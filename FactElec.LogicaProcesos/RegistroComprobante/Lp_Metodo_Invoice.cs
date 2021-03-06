﻿using FactElec.CapaEntidad.ComprobanteElectronico.Invoice;
using FactElec.CapaEntidad.RegistroComprobante;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace FactElec.LogicaProceso.RegistroComprobante
{
    public class Lp_Metodo_Invoice
    {
        readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Lp_Metodo_Invoice));
        public En_Respuesta RegistroComprobante(En_ComprobanteElectronico Comprobante)
        {
            log.Info("Invocación al método RegistroComprobante");
            En_Respuesta oRespuesta = new En_Respuesta();
            string rutaXML = "";

            try
            {
                InvoiceType invoice = new InvoiceType();
                LlenarCabecera(Comprobante, ref invoice);
                LlenarEmisor(Comprobante.Emisor, ref invoice);
                LlenarReceptor(Comprobante.Receptor, ref invoice);
                LlenarDescuentoCargo(Comprobante, ref invoice);
                LlenarMontosIGV(Comprobante, ref invoice);
                LlenarMontosTotales(Comprobante, ref invoice);
                LlenarDetalle(Comprobante, ref invoice);

                string codigoHASH = "", nombreXML = "", firma = "";
                byte[] archivoXML = null;
                nombreXML = string.Format("{0}-{1}-{2}.xml", Comprobante.Emisor.NumeroDocumentoIdentidad, Comprobante.TipoComprobante, Comprobante.SerieNumero);
                CrearXML(ref invoice, Comprobante, ref codigoHASH, ref archivoXML, ref firma);

                string mensajeRetorno = "";
                Lp_Comprobante lpComprobante = new Lp_Comprobante();
                bool resultado = lpComprobante.InsertarComprobante(Comprobante, nombreXML, archivoXML, codigoHASH, firma, ref mensajeRetorno);

                oRespuesta.Codigo = (resultado) ? "0" : "99";
                oRespuesta.Descripcion = mensajeRetorno;

                string carpetaTemp = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temporal");
                rutaXML = Path.Combine(carpetaTemp, nombreXML);
                if (File.Exists(rutaXML)) File.Delete(rutaXML);
            }
            catch (Exception ex)
            {
                oRespuesta.Codigo = "99";
                oRespuesta.Descripcion = "Ocurrió un error general, mensaje: " + ex.Message.ToString();
                if (rutaXML != "" && File.Exists(rutaXML)) File.Delete(rutaXML);
            }
            return oRespuesta;
        }


        TaxSubtotalType LlenarSubTotalDetalle(decimal MontoBase, decimal MontoTotalImpuesto, string Moneda, decimal PorcentajeImpuesto, string CodigoInternacionalTributo, string NombreTributo, string CodigoTributo, string AfectacionIGV)
        {
            TaxSubtotalType oSubtotal = new TaxSubtotalType
            {
                TaxableAmount = new TaxableAmountType
                {
                    Value = MontoBase,
                    currencyID = Moneda
                },
                TaxAmount = new TaxAmountType
                {
                    Value = MontoTotalImpuesto,
                    currencyID = Moneda
                },
                TaxCategory = new TaxCategoryType
                {
                    Percent = new PercentType1
                    {
                        Value = PorcentajeImpuesto
                    },
                    TaxExemptionReasonCode = new TaxExemptionReasonCodeType
                    {
                        listAgencyName = "PE:SUNAT",
                        listName = "Afectacion del IGV",
                        listURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo07",
                        Value = AfectacionIGV
                    },
                    TaxScheme = new TaxSchemeType
                    {
                        ID = new IDType
                        {
                            schemeAgencyName = "PE:SUNAT",
                            schemeName = "Codigo de tributos",
                            schemeURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo05",
                            Value = CodigoInternacionalTributo
                        },
                        Name = new NameType1
                        {
                            Value = NombreTributo
                        },
                        TaxTypeCode = new TaxTypeCodeType
                        {
                            Value = CodigoTributo
                        }
                    }
                }
            };
            return oSubtotal;

        }

        void LlenarDetalle(En_ComprobanteElectronico Comprobante, ref InvoiceType invoice)
        {
            List<InvoiceLineType> oListaDetalle = new List<InvoiceLineType>();

            foreach (En_ComprobanteDetalle oDet in Comprobante.ComprobanteDetalle)
            {
                List<DescriptionType> oListaDescripcion = new List<DescriptionType>();

                DescriptionType oDescripcion = new DescriptionType();
                oDescripcion.Value = oDet.Descripcion;
                oListaDescripcion.Add(oDescripcion);

                foreach (string oDes in oDet.MultiDescripcion)
                {
                    DescriptionType oDescrip = new DescriptionType();
                    oDescrip.Value = oDes.ToString();
                    oListaDescripcion.Add(oDescrip);
                }



                List<TaxSubtotalType> oListaSubtotal = new List<TaxSubtotalType>();

                foreach (En_ComprobanteDetalleImpuestos odetImpuesto in oDet.ComprobanteDetalleImpuestos)
                {
                    TaxSubtotalType oSubTotal = new TaxSubtotalType();
                    oSubTotal = LlenarSubTotalDetalle(odetImpuesto.MontoBase, odetImpuesto.MontoTotalImpuesto, Comprobante.Moneda.Trim(), odetImpuesto.Porcentaje, odetImpuesto.CodigoInternacionalTributo, odetImpuesto.NombreTributo, odetImpuesto.CodigoTributo, odetImpuesto.AfectacionIGV);
                    oListaSubtotal.Add(oSubTotal);
                }

                InvoiceLineType oInvoiceLine = new InvoiceLineType
                {
                    ID = new IDType
                    {
                        Value = oDet.Item.ToString()
                    },
                    InvoicedQuantity = new InvoicedQuantityType
                    {
                        unitCode = oDet.UnidadMedida.Trim().ToUpper(),
                        unitCodeListAgencyName = "United Nations Economic Commission for Europe",
                        unitCodeListID = "UN/ECE rec 20",
                        Value = oDet.Cantidad
                    },
                    LineExtensionAmount = new LineExtensionAmountType
                    {
                        Value = oDet.Total,
                        currencyID = Comprobante.Moneda.Trim()
                    },
                    PricingReference = new PricingReferenceType
                    {
                        AlternativeConditionPrice = new PriceType[] {
                           new PriceType {
                               PriceAmount=new PriceAmountType{
                                   Value =oDet.ValorVentaUnitarioIncIgv,
                                   currencyID=Comprobante.Moneda .Trim ()
                               },
                               PriceTypeCode=new PriceTypeCodeType{
                                   Value =oDet.CodigoTipoPrecio,
                                   listAgencyName="PE:SUNAT" ,
                                   listName ="Tipo de Precio" ,
                                   listURI ="urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo16"
                               }

                           }
                       }
                    },
                    TaxTotal = new TaxTotalType[] {
                        new TaxTotalType{
                            TaxAmount =new TaxAmountType{
                                Value =oDet.ImpuestoTotal ,
                                currencyID =Comprobante.Moneda.Trim ()
                            },
                            TaxSubtotal =oListaSubtotal.ToArray()

                            }
                        },
                    Price = new PriceType
                    {
                        PriceAmount = new PriceAmountType
                        {
                            Value = oDet.ValorVentaUnitario,
                            currencyID = Comprobante.Moneda.Trim()
                        }
                    },
                    Item = new ItemType
                    {
                        Description = oListaDescripcion.ToArray(),

                        SellersItemIdentification = new ItemIdentificationType
                        {
                            ID = new IDType
                            {
                                Value = oDet.Codigo
                            }
                        },
                        CommodityClassification = new CommodityClassificationType[]
                        {
                            new CommodityClassificationType{
                                CommodityCode =new CommodityCodeType
                                {
                                        listAgencyName="GS1 US",
                                        listID ="UNSPSC" ,
                                        listName ="Item Classification",
                                        Value =oDet.CodigoSunat
                                }
                            }

                        }
                    }

                };
                oListaDetalle.Add(oInvoiceLine);


            };
            invoice.InvoiceLine = oListaDetalle.ToArray();

        }

        void LlenarDescuentoCargo(En_ComprobanteElectronico Comprobante, ref InvoiceType invoice)
        {
            List<AllowanceChargeType> oListaDescuentoCargo = new List<AllowanceChargeType>();

            if (Comprobante.DescuentoCargo != null)
            {
                foreach (En_DescuentoCargo oDescar in Comprobante.DescuentoCargo)
                {
                    AllowanceChargeType oDescuentoCargo = new AllowanceChargeType
                    {
                        AllowanceChargeReason = new AllowanceChargeReasonType[]
                        {
                        new AllowanceChargeReasonType{ Value =oDescar.Motivo }
                        },
                        ChargeIndicator = new ChargeIndicatorType
                        {
                            Value = oDescar.Indicador
                        },

                        AllowanceChargeReasonCode = new AllowanceChargeReasonCodeType
                        {
                            listAgencyName = "PE:SUNAT",
                            listName = "Cargo/descuento",
                            listURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo53",
                            Value = oDescar.CodigoMotivo
                        },
                        MultiplierFactorNumeric = new MultiplierFactorNumericType
                        {
                            Value = oDescar.Factor
                        },
                        Amount = new AmountType2
                        {
                            Value = oDescar.MontoTotal,
                            currencyID = Comprobante.Moneda.Trim()
                        },
                        BaseAmount = new BaseAmountType
                        {
                            Value = oDescar.MontoBase,
                            currencyID = Comprobante.Moneda.Trim()
                        }

                    };
                    oListaDescuentoCargo.Add(oDescuentoCargo);
                }
                invoice.AllowanceCharge = oListaDescuentoCargo.ToArray();
            }
        }

        void LlenarMontosTotales(En_ComprobanteElectronico Comprobante, ref InvoiceType invoice)
        {
            MonetaryTotalType oTotal = new MonetaryTotalType
            {
                LineExtensionAmount = new LineExtensionAmountType
                {
                    Value = Comprobante.TotalValorVenta,
                    currencyID = Comprobante.Moneda.Trim()
                },
                TaxInclusiveAmount = new TaxInclusiveAmountType
                {
                    Value = Comprobante.TotalPrecioVenta,
                    currencyID = Comprobante.Moneda.Trim()
                },
                PayableAmount = new PayableAmountType
                {
                    Value = Comprobante.ImporteTotal,
                    currencyID = Comprobante.Moneda.Trim()
                }
            };

            if (Comprobante.TotalCargo > 0)
            {
                ChargeTotalAmountType oChargeTotalAmount = new ChargeTotalAmountType
                {
                    Value = Comprobante.TotalCargo,
                    currencyID = Comprobante.Moneda.Trim()
                };
                oTotal.ChargeTotalAmount = oChargeTotalAmount;
            }

            if (Comprobante.TotalDescuento > 0)
            {
                AllowanceTotalAmountType oAllowanceTotalAmount = new AllowanceTotalAmountType
                {
                    Value = Comprobante.TotalDescuento,
                    currencyID = Comprobante.Moneda.Trim()
                };
                oTotal.AllowanceTotalAmount = oAllowanceTotalAmount;
            }

            invoice.LegalMonetaryTotal = oTotal;

        }

        void LlenarMontosIGV(En_ComprobanteElectronico Comprobante, ref InvoiceType invoice)
        {
            List<TaxSubtotalType> oListaSubtotal = new List<TaxSubtotalType>();

            if (Comprobante.MontoTotales != null)
            {
                if (Comprobante.MontoTotales.Gravado != null)
                {
                    if (Comprobante.MontoTotales.Gravado.GravadoIGV != null)
                    {
                        TaxSubtotalType oTotalGravado = LlenarSubTotalCabecera(Comprobante.MontoTotales.Gravado.GravadoIGV.MontoBase, Comprobante.MontoTotales.Gravado.GravadoIGV.MontoTotalImpuesto, Comprobante.Moneda, Comprobante.MontoTotales.Gravado.GravadoIGV.Porcentaje, "1000", "IGV", "VAT");
                        oListaSubtotal.Add(oTotalGravado);
                    }

                }
            }


            TaxTotalType oTaxTotal = new TaxTotalType
            {
                TaxAmount = new TaxAmountType
                {
                    Value = Comprobante.TotalImpuesto,
                    currencyID = Comprobante.Moneda
                },
                TaxSubtotal = oListaSubtotal.ToArray()
            };

            invoice.TaxTotal = new TaxTotalType[] { oTaxTotal };

        }

        TaxSubtotalType LlenarSubTotalCabecera(decimal MontoOperaciones, decimal MontoTotalImpuesto, string Moneda, decimal PorcentajeImpuesto, string CodigoInternacionalTributo, string NombreTributo, string CodigoTributo)
        {
            TaxSubtotalType oSubtotal = new TaxSubtotalType
            {
                TaxableAmount = new TaxableAmountType
                {
                    Value = MontoOperaciones,
                    currencyID = Moneda
                },
                TaxAmount = new TaxAmountType
                {
                    Value = MontoTotalImpuesto,
                    currencyID = Moneda
                },
                Percent = new PercentType1
                {
                    Value = PorcentajeImpuesto
                },
                TaxCategory = new TaxCategoryType
                {
                    TaxScheme = new TaxSchemeType
                    {
                        ID = new IDType
                        {
                            schemeAgencyName = "PE:SUNAT",
                            schemeName = "Codigo de tributos",
                            schemeURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo05",
                            Value = CodigoInternacionalTributo
                        },
                        Name = new NameType1
                        {
                            Value = NombreTributo
                        },
                        TaxTypeCode = new TaxTypeCodeType
                        {
                            Value = CodigoTributo
                        }
                    }
                }
            };
            return oSubtotal;

        }

        void CrearXML(ref InvoiceType invoice, En_ComprobanteElectronico Comprobante, ref string codigoHASH, ref byte[] archivoXML, ref string firma)
        {
            XmlSerializer oxmlSerializer = new XmlSerializer(typeof(InvoiceType));
            var xmlNameSpaceNom = new XmlSerializerNamespaces();
            xmlNameSpaceNom.Add("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
            xmlNameSpaceNom.Add("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
            xmlNameSpaceNom.Add("ds", "http://www.w3.org/2000/09/xmldsig#");
            xmlNameSpaceNom.Add("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");
            xmlNameSpaceNom.Add("qdt", "urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2");
            xmlNameSpaceNom.Add("sac", "urn:sunat:names:specification:ubl:peru:schema:xsd:SunatAggregateComponents-1");
            xmlNameSpaceNom.Add("udt", "urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2");

            string carpetaTemp = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temporal");
            string nombreXml = string.Format("{0}-{1}-{2}.xml", Comprobante.Emisor.NumeroDocumentoIdentidad, Comprobante.TipoComprobante, Comprobante.SerieNumero);
            string ruta = Path.Combine(carpetaTemp, nombreXml);

            string sxml = "";
            Encoding utf8noBOM = new UTF8Encoding(false);
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = utf8noBOM
            };

            // Se ha creado una nueva clase llamada StringWriterWithEncoding que grabaen formato UTF-8
            // Por defecto la clase StringWriter graba con UTF-16 y no se podía firmar, con UTF-8 ya se puede
            using (var sw = new StringWriterWithEncoding())
            {
                using (XmlWriter writter = XmlWriter.Create(sw, settings))
                {
                    oxmlSerializer.Serialize(writter, invoice, xmlNameSpaceNom);
                    sxml = sw.ToString();
                }
            }

            // Se elimina el tag temporal creado dentro de ext:ExtensionContent y en su lugar se colocará la firma
            string cadena = "<Borrar xmlns=\"\" />";
            int tamanio = cadena.Length;
            int indice = sxml.IndexOf(cadena);
            sxml = sxml.Remove(indice, tamanio);

            // Se sigue grabando el archivo como siempre 
            File.WriteAllText(ruta, sxml, Encoding.UTF8);

            // Firma del comprobante
            var objFirma = new Firma.FirmaComprobante();
            var document = new XmlDocument();
            document.Load(ruta);
            // Enviamos el RUC de la empresa, para ello el certificado debe estar registrado
            objFirma.FirmarXml(document, Comprobante.Emisor.NumeroDocumentoIdentidad, ref codigoHASH, ref firma);
            document.Save(ruta);

            archivoXML = File.ReadAllBytes(ruta);
        }

        void LlenarCabecera(En_ComprobanteElectronico Comprobante, ref InvoiceType invoice)
        {
            UBLExtensionType uBLExtensionType = new UBLExtensionType()
            {
                // Se crea un tag temporal llamado "Borrar", esto porque no he conseguido crear el tag
                // ext:ExtensionContent con un valor vacío
                ExtensionContent = new XmlDocument().CreateElement("Borrar")
            };

            UBLExtensionType[] ublExtensions = { uBLExtensionType };
            invoice.UBLExtensions = ublExtensions;
            //Serie y Numero          
            invoice.ID = new IDType
            {
                Value = Comprobante.SerieNumero.Trim()
            };

            invoice.UBLVersionID = new UBLVersionIDType
            {
                Value = "2.1"
            };

            if (!string.IsNullOrEmpty(Comprobante.FechaVencimiento))
            {
                invoice.DueDate = new DueDateType
                {
                    Value = Comprobante.FechaVencimiento
                };
            }

            invoice.IssueDate = new IssueDateType
            {
                Value = Comprobante.FechaEmision
            };
            invoice.IssueTime = new IssueTimeType
            {
                Value = Comprobante.HoraEmision
            };

            invoice.DocumentCurrencyCode = new DocumentCurrencyCodeType
            {
                listAgencyName = "United Nations Economic Commission for Europe",
                listID = "ISO 4217 Alpha",
                listName = "Currency",
                Value = Comprobante.Moneda.Trim()
            };

            List<NoteType> oListaNota = new List<NoteType>();
            foreach (En_Leyenda oNote in Comprobante.Leyenda)
            {
                NoteType oNota = new NoteType
                {
                    Value = oNote.Valor,
                    languageLocaleID = oNote.Codigo
                };
                oListaNota.Add(oNota);
            };

            invoice.Note = oListaNota.ToArray();

            invoice.CustomizationID = new CustomizationIDType
            {
                Value = "2.0"
            };

            invoice.InvoiceTypeCode = new InvoiceTypeCodeType
            {
                listAgencyName = "PE:SUNAT",
                listID = Comprobante.TipoOperacion.Trim(),
                listName = "Tipo de Documento",
                listSchemeURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo51",
                listURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo01",
                name = "Tipo de Operacion",
                Value = Comprobante.TipoComprobante.Trim()
            };


            List<DocumentReferenceType> oListadocumento = new List<DocumentReferenceType>();

            if (Comprobante.DocumentoReferenciaDespacho != null)
            {
                foreach (En_DocumentoReferencia oreferen in Comprobante.DocumentoReferenciaDespacho)
                {
                    DocumentReferenceType odocumento = new DocumentReferenceType
                    {
                        ID = new IDType
                        {
                            Value = oreferen.SerieNumero.Trim()
                        },
                        IssueDate = new IssueDateType
                        {
                            Value = oreferen.Fecha.Trim()
                        },
                        DocumentTypeCode = new DocumentTypeCodeType
                        {
                            Value = oreferen.TipoDocumento.Trim(),
                            listAgencyName = "PE:SUNAT",
                            listName = "SUNAT:Identificador de guía relacionada",
                            listURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo12"
                        }
                    };
                    oListadocumento.Add(odocumento);
                };
                invoice.DespatchDocumentReference = oListadocumento.ToArray();
            }
        }

        void LlenarEmisor(En_Emisor Emisor, ref InvoiceType invoice)
        {

            WebsiteURIType EmisorPaginaWeb = new WebsiteURIType
            {
                Value = Emisor.PaginaWeb.Trim()
            };

            List<PartyNameType> oListaNombreComercial = new List<PartyNameType>();
            PartyNameType PartyName = new PartyNameType
            {
                Name = new NameType1
                {
                    Value = Emisor.NombreComercial.Trim()
                }
            };
            oListaNombreComercial.Add(PartyName);


            PartyIdentificationType EmisorIdentificacion = new PartyIdentificationType();
            List<PartyIdentificationType> EmisorListaIdentificacion = new List<PartyIdentificationType>();
            EmisorIdentificacion.ID = new IDType
            {
                Value = Emisor.NumeroDocumentoIdentidad.Trim(),
                schemeAgencyID = "PE:SUNAT",
                schemeID = Emisor.TipoDocumentoIdentidad.Trim(),
                schemeName = "Documento de Identidad",
                schemeURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06"
            };
            EmisorListaIdentificacion.Add(EmisorIdentificacion);

            //Razon social
            PartyLegalEntityType oRazonSocial = new PartyLegalEntityType();

            if (!string.IsNullOrEmpty(Emisor.RazonSocial) || !string.IsNullOrWhiteSpace(Emisor.RazonSocial))
            {
                oRazonSocial.RegistrationName = new RegistrationNameType();
                oRazonSocial.RegistrationName.Value = Emisor.RazonSocial;
            }

            oRazonSocial.RegistrationAddress = new AddressType();


            if (!string.IsNullOrEmpty(Emisor.CodigoUbigeo) && !string.IsNullOrWhiteSpace(Emisor.CodigoUbigeo))
            {
                oRazonSocial.RegistrationAddress.ID = new IDType();
                oRazonSocial.RegistrationAddress.ID.Value = Emisor.CodigoUbigeo;
                oRazonSocial.RegistrationAddress.ID.schemeAgencyName = "PE:INEI";
                oRazonSocial.RegistrationAddress.ID.schemeName = "Ubigeos";
            }

            if (!string.IsNullOrEmpty(Emisor.CodigoDomicilioFiscal) || !string.IsNullOrWhiteSpace(Emisor.CodigoDomicilioFiscal))
            {
                oRazonSocial.RegistrationAddress.AddressTypeCode = new AddressTypeCodeType();
                oRazonSocial.RegistrationAddress.AddressTypeCode.Value = Emisor.CodigoDomicilioFiscal;
                oRazonSocial.RegistrationAddress.AddressTypeCode.listAgencyName = "PE:SUNAT";
                oRazonSocial.RegistrationAddress.AddressTypeCode.listName = "Establecimientos anexos";
            }

            if (!string.IsNullOrEmpty(Emisor.Urbanizacion) || !string.IsNullOrWhiteSpace(Emisor.Urbanizacion))
            {
                oRazonSocial.RegistrationAddress.CitySubdivisionName = new CitySubdivisionNameType();
                oRazonSocial.RegistrationAddress.CitySubdivisionName.Value = Emisor.Urbanizacion;
            }

            if (!string.IsNullOrEmpty(Emisor.Provincia) || !string.IsNullOrWhiteSpace(Emisor.Provincia))
            {
                oRazonSocial.RegistrationAddress.CityName = new CityNameType();
                oRazonSocial.RegistrationAddress.CityName.Value = Emisor.Provincia;
            }

            if (!string.IsNullOrEmpty(Emisor.Departamento) || !string.IsNullOrWhiteSpace(Emisor.Departamento))
            {
                oRazonSocial.RegistrationAddress.CountrySubentity = new CountrySubentityType();
                oRazonSocial.RegistrationAddress.CountrySubentity.Value = Emisor.Departamento;
            }

            if (!string.IsNullOrEmpty(Emisor.Distrito) || !string.IsNullOrWhiteSpace(Emisor.Distrito))
            {
                oRazonSocial.RegistrationAddress.District = new DistrictType();
                oRazonSocial.RegistrationAddress.District.Value = Emisor.Distrito;
            }

            if (!string.IsNullOrEmpty(Emisor.Direccion) || !string.IsNullOrWhiteSpace(Emisor.Direccion))
            {
                oRazonSocial.RegistrationAddress.AddressLine = new AddressLineType[1];
                oRazonSocial.RegistrationAddress.AddressLine[0] = new AddressLineType();
                oRazonSocial.RegistrationAddress.AddressLine[0].Line = new LineType();
                oRazonSocial.RegistrationAddress.AddressLine[0].Line.Value = Emisor.Direccion;
            }

            if (!string.IsNullOrEmpty(Emisor.CodigoPais) || !string.IsNullOrWhiteSpace(Emisor.CodigoPais))
            {
                oRazonSocial.RegistrationAddress.Country = new CountryType();
                oRazonSocial.RegistrationAddress.Country.IdentificationCode = new IdentificationCodeType();
                oRazonSocial.RegistrationAddress.Country.IdentificationCode.listAgencyName = "United Nations Economic Commission for Europe";
                oRazonSocial.RegistrationAddress.Country.IdentificationCode.listID = "ISO 3166-1";
                oRazonSocial.RegistrationAddress.Country.IdentificationCode.listName = "Country";
                oRazonSocial.RegistrationAddress.Country.IdentificationCode.Value = Emisor.CodigoPais;
            }

            ContactType oContacto = new ContactType
            {
                ElectronicMail = new ElectronicMailType()
                {
                    Value = Emisor.Contacto.Correo.Trim()
                },
                Name = new NameType1
                {
                    Value = Emisor.Contacto.Nombre.Trim()
                },
                Telephone = new TelephoneType
                {
                    Value = Emisor.Contacto.Telefono.Trim()
                }
            };

            SupplierPartyType oEmisor = new SupplierPartyType
            {
                Party = new PartyType
                {
                    WebsiteURI = EmisorPaginaWeb,
                    PartyIdentification = EmisorListaIdentificacion.ToArray(),
                    PartyName = oListaNombreComercial.ToArray(),
                    PartyLegalEntity = new PartyLegalEntityType[] { oRazonSocial },
                    Contact = oContacto

                }
            };

            invoice.AccountingSupplierParty = oEmisor;


        }

        void LlenarReceptor(En_Receptor Receptor, ref InvoiceType invoice)
        {

            WebsiteURIType EmisorPaginaWeb = new WebsiteURIType
            {
                Value = Receptor.PaginaWeb.Trim()
            };

            List<PartyNameType> oListaNombreComercial = new List<PartyNameType>();
            PartyNameType PartyName = new PartyNameType
            {
                Name = new NameType1
                {
                    Value = Receptor.NombreComercial.Trim()
                }
            };
            oListaNombreComercial.Add(PartyName);


            PartyIdentificationType EmisorIdentificacion = new PartyIdentificationType();
            List<PartyIdentificationType> EmisorListaIdentificacion = new List<PartyIdentificationType>();
            EmisorIdentificacion.ID = new IDType
            {
                Value = Receptor.NumeroDocumentoIdentidad.Trim(),
                schemeAgencyID = "PE:SUNAT",
                schemeID = Receptor.TipoDocumentoIdentidad.Trim(),
                schemeName = "Documento de Identidad",
                schemeURI = "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06"
            };
            EmisorListaIdentificacion.Add(EmisorIdentificacion);

            //Razon social
            PartyLegalEntityType oRazonSocial = new PartyLegalEntityType
            {
                RegistrationName = new RegistrationNameType
                {
                    Value = Receptor.RazonSocial,
                },

                RegistrationAddress = new AddressType
                {
                    ID = new IDType
                    {
                        Value = Receptor.CodigoUbigeo,
                        schemeAgencyName = "PE:INEI",
                        schemeName = "Ubigeos"
                    },
                    AddressTypeCode = new AddressTypeCodeType
                    {
                        Value = Receptor.CodigoDomicilioFiscal,
                        listAgencyName = "PE:SUNAT",
                        listName = "Establecimientos anexos"
                    },
                    CitySubdivisionName = new CitySubdivisionNameType
                    {
                        Value = Receptor.Urbanizacion
                    },
                    CityName = new CityNameType
                    {
                        Value = Receptor.Provincia
                    },
                    CountrySubentity = new CountrySubentityType
                    {
                        Value = Receptor.Departamento
                    },
                    District = new DistrictType
                    {
                        Value = Receptor.Distrito
                    },
                    AddressLine = new AddressLineType[] {
                        new AddressLineType {
                            Line =new LineType{
                                Value =Receptor.Direccion
                            }
                        }
                    },
                    Country = new CountryType
                    {
                        IdentificationCode = new IdentificationCodeType
                        {
                            listAgencyName = "United Nations Economic Commission for Europe",
                            listID = "ISO 3166-1",
                            listName = "Country",
                            Value = Receptor.CodigoPais
                        }
                    }
                }
            };

            ContactType oContacto = new ContactType
            {
                ElectronicMail = new ElectronicMailType()
                {
                    Value = Receptor.Contacto.Correo.Trim()
                },
                Name = new NameType1
                {
                    Value = Receptor.Contacto.Nombre.Trim()
                },
                Telephone = new TelephoneType
                {
                    Value = Receptor.Contacto.Telefono.Trim()
                }
            };

            CustomerPartyType oReceptor = new CustomerPartyType
            {
                Party = new PartyType
                {
                    WebsiteURI = EmisorPaginaWeb,
                    PartyIdentification = EmisorListaIdentificacion.ToArray(),
                    PartyName = oListaNombreComercial.ToArray(),
                    PartyLegalEntity = new PartyLegalEntityType[] { oRazonSocial },
                    Contact = oContacto

                }
            };

            invoice.AccountingCustomerParty = oReceptor;


        }
    }

    public sealed class StringWriterWithEncoding : StringWriter
    {
        private readonly Encoding encoding;

        public StringWriterWithEncoding() : this(Encoding.UTF8) { }

        public StringWriterWithEncoding(Encoding encoding)
        {
            this.encoding = encoding;
        }

        public override Encoding Encoding
        {
            get { return encoding; }
        }
    }
}
