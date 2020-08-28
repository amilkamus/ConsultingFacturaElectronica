using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.XPath;
using FactElec.CapaEntidad.SincronizarComprobante;
using ICSharpCode.SharpZipLib.Zip;

namespace FactElec.LogicaProceso
{
    public class Lp_Utilitario
    {
        public En_Respuesta LeerRespuestaXml(string nombreArchivoDescomprimido, bool esExcepcion)
        {
            string cadenaXML = "";
            En_Respuesta oRespuesta = new En_Respuesta();
            StreamReader strreader = new StreamReader(nombreArchivoDescomprimido, System.Text.Encoding.UTF8);
            cadenaXML = strreader.ReadToEnd();

            XmlDocument xmlRespuesta = new XmlDocument();
            xmlRespuesta.LoadXml(cadenaXML);

            strreader.Dispose();
            XPathNavigator nav = xmlRespuesta.CreateNavigator();
            XmlNamespaceManager ns = ObtenerXmlNamespaces(nav);

            if (!esExcepcion)
            {
                foreach (XPathNavigator nodoXML in nav.Select("*/cac:DocumentResponse/cac:Response", ns))
                {
                    oRespuesta.Codigo = NodeValue(nodoXML.SelectSingleNode("cbc:ResponseCode", ns), "");
                    oRespuesta.Descripcion = NodeValue(nodoXML.SelectSingleNode("cbc:Description", ns), "");
                }

            List<string> listaMensaje = new List<string>();
            foreach (XPathNavigator nodoXML in nav.Select("*/cbc:Note", ns))
            {
                string mensaje = NodeValue(nodoXML.SelectSingleNode("cbc:ResponseCode", ns), "");
                if (mensaje.Trim().Length > 0)
                {
                    listaMensaje.Add(mensaje);
                }
            }

            if (listaMensaje.Count > 0)
            {
                oRespuesta.Detalle = listaMensaje.ToArray();
            }

                oRespuesta.FecharespuestaSunat = NodeValue(nav.SelectSingleNode("*/cbc:ResponseDate", ns), "");
                oRespuesta.HoraRespuestaSunat = NodeValue(nav.SelectSingleNode("*/cbc:ResponseTime", ns), "");
            }
            else
            {
                oRespuesta.Codigo = NodeValue(nav.SelectSingleNode("*/codigo", ns), "");
                oRespuesta.Descripcion = NodeValue(nav.SelectSingleNode("*/mensaje", ns), "");
                oRespuesta.FecharespuestaSunat = DateTime.Now.ToString("yyyy-MM-dd");
                oRespuesta.HoraRespuestaSunat = DateTime.Now.ToString("HH:mm:ss");
            }

            return oRespuesta;
        }
        public static XmlNamespaceManager ObtenerXmlNamespaces(XPathNavigator nav)
        {
            var ns = new XmlNamespaceManager(nav.NameTable);
            var nodes = nav.Select("/*/namespace::cac");
            while (nodes.MoveNext())
            {
                var nsis = nodes.Current.GetNamespacesInScope(XmlNamespaceScope.Local);
                foreach (var nsi in nsis)
                {
                    var prf = nsi.Key == string.Empty ? "global" : nsi.Key;
                    ns.AddNamespace(prf, nsi.Value);
                }
            }

            return ns;
        }
        public static string NodeValue(XPathItem node, string defaultValue)
        {
            if (node != null)
                return node.Value ?? defaultValue;
            return defaultValue;
        }
        public string Descomprimir(string directorio, string zipFic, ref bool excepcion)
        {
            string RutaArchivo = string.Empty;
            ZipInputStream z = null;
            try
            {

                if (!zipFic.ToLower().EndsWith(".zip"))
                    zipFic = Directory.GetFiles(zipFic, "*.zip")[0];
                if (directorio == "")
                    directorio = ".";
                z = new ZipInputStream(File.OpenRead(directorio + @"\" + zipFic));
                ZipEntry theEntry;
                do
                {
                    theEntry = z.GetNextEntry();
                    if (theEntry != null)
                    {
                        string fileName = directorio + @"\" + Path.GetFileName(theEntry.Name);
                        if (!Directory.Exists(fileName))
                        {
                            if (Path.GetExtension(fileName).ToString().ToUpper() == ".XML")
                            {
                                RutaArchivo = fileName;
                                FileStream streamWriter;
                                try
                                {
                                    streamWriter = File.Create(fileName);
                                }
                                catch //(DirectoryNotFoundException ex)
                                {
                                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                                    streamWriter = File.Create(fileName);
                                }
                                // 
                                int size;
                                byte[] data = new byte[2049];
                                do
                                {
                                    size = z.Read(data, 0, data.Length);
                                    if ((size > 0))
                                        streamWriter.Write(data, 0, size);
                                    else
                                        break;
                                }
                                while (true);
                                streamWriter.Close();
                            }
                        }
                    }
                    else
                        break;
                }
                while (true);
                z.Close();
                excepcion = false;
                return RutaArchivo;
            }
            catch
            {
                if (z != null)
                    z.Close();

                var archivo = directorio + @"\" + zipFic;
                File.Move(archivo, Path.ChangeExtension(archivo, ".xml"));
                excepcion = true;
                return Path.ChangeExtension(archivo, ".xml");
            }
        }
    }
}
