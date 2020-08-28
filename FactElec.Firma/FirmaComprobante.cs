using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using System.Configuration;

namespace FactElec.Firma
{
    public class FirmaComprobante
    {
        private readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(FirmaComprobante));
        public XmlDocument FirmarXml(XmlDocument xmlDoc, string ruc, ref string codigoHash, ref string firma)
        {
            XmlDocument xmlDocument = null;
            string carpetaCertificado = "";
            string nombreArchivoCertificado = "";
            string total = "";
            try
            {
                if (xmlDoc == null)
                {
                    ArgumentException nex = new ArgumentException("El documento XML generado es nulo.");
                    throw nex;
                }

                SignedXml signedXml = new SignedXml(xmlDoc);
                Reference reference = new Reference()
                {
                    Uri = ""
                };
                reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
                signedXml.AddReference(reference);
                carpetaCertificado = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Certificado");
                nombreArchivoCertificado = string.Format("{0}.pfx", ruc);
                byte[] bytesCertificado = File.ReadAllBytes(Path.Combine(carpetaCertificado, nombreArchivoCertificado));
                total = Path.Combine(carpetaCertificado, nombreArchivoCertificado);
                log.Info("total:"+ total);
                //X509Certificate2 certificado = new X509Certificate2(bytesCertificado, ruc); //DevuelveCertificado(ruc);

                //X509Certificate2 certificado = new X509Certificate2(bytesCertificado, ConfigurationManager.AppSettings[ruc].ToString());
                X509Certificate2 certificado = new X509Certificate2(total, ConfigurationManager.AppSettings[ruc].ToString(), X509KeyStorageFlags.MachineKeySet);
                //X509Certificate2 certificado = new X509Certificate2(@"h:\root\home\crouillon-001\www\quypay\factelect\certificado\20602034675.pfx", ConfigurationManager.AppSettings[ruc].ToString());
                string subjectName = certificado.SubjectName.Name;

                log.Info("subjectName");

                KeyInfo keyInfo = new KeyInfo();
                try
                {
                    signedXml.SigningKey = certificado.PrivateKey;
                }
                catch
                {
                    Exception nex = new Exception("m_safeCertContext es un controlador no válido.");
                    throw nex;
                }
                KeyInfoX509Data kData = new KeyInfoX509Data(certificado);
                kData.AddSubjectName(subjectName);
                keyInfo.AddClause(kData);
                signedXml.KeyInfo = keyInfo;
                signedXml.Signature.Id = "signatureKG";
                signedXml.ComputeSignature();
                XmlElement xmlDigitalSignature = signedXml.GetXml();
                var insertoFirma = false;
                InsertaFirma(xmlDoc.ChildNodes, xmlDigitalSignature, ref insertoFirma);
                if (!insertoFirma)
                {
                    throw new Exception("No se ha insertado la firma.");
                }
                else
                {
                    log.Info("Se firmó satisfactoriamente el comprobante.");
                }
                codigoHash = xmlDigitalSignature.ChildNodes[0].ChildNodes[2].ChildNodes[2].InnerText;
                firma = xmlDigitalSignature.ChildNodes[1].InnerText;
                keyInfo = null;
                reference = null;
                signedXml = null;
                xmlDigitalSignature = null;
                xmlDocument = xmlDoc;
                xmlDocument.PreserveWhitespace = true;
            }
            catch (Exception exception)
            {
                log.Error(exception.Message + " carpetaCertificado:" + carpetaCertificado +"." + " nombreArchivoCertificado:"+ nombreArchivoCertificado + ".", exception);
                throw;
            }
            return xmlDocument;
        }

        private void InsertaFirma(XmlNodeList l_nodos, XmlElement element, ref bool aplico)
        {
            try
            {
                foreach (XmlNode nodo in l_nodos)
                {
                    if (nodo.LocalName.Equals("ExtensionContent"))
                    {
                        if (!nodo.HasChildNodes)
                        {
                            nodo.AppendChild(nodo.OwnerDocument.ImportNode(element, true));
                            aplico = true;
                            break;
                        }
                    }
                    if (nodo.HasChildNodes)
                    {
                        InsertaFirma(nodo.ChildNodes, element, ref aplico);
                    }
                }
            }
            catch (Exception exception)
            {
                Exception ex = exception;
                log.Error(ex.Message, ex);
                throw new Exception("Error al Insertar Firma en XML");
            }
        }

        private X509Certificate2 DevuelveCertificado(string ruc)
        {
            X509Certificate2 certificado = new X509Certificate2();
            try
            {
                Exception nex;
                string nombre_certificado = ruc;
                if (nombre_certificado.Equals(""))
                {
                    nex = new Exception("No se ha enviado el RUC del certificado.");
                    throw nex;
                }

                X509Store objStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                objStore.Open(OpenFlags.ReadOnly);
                bool existeFirma = false;
                X509Certificate2Enumerator enumerator = objStore.Certificates.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    X509Certificate2 cert = enumerator.Current;
                    if (cert.FriendlyName.Equals(nombre_certificado))
                    {
                        certificado = cert;
                        existeFirma = true;
                        break;
                    }
                }
                objStore.Close();
                if (!existeFirma)
                {
                    nex = new Exception(string.Concat("La firma enviada (", nombre_certificado, ") no existe en el Servidor."));
                    throw nex;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }

            return certificado;
        }
    }
}
