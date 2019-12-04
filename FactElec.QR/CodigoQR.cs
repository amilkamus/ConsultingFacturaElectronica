
using com.barcodelib.barcode;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactElec.QR
{
    public class CodigoQR
    {

        public string ExtraerCertificado(string firma)
        {
            string codigoQR = "";
            string[] arregloCodigoBarra;
            arregloCodigoBarra = firma.Split('|');

            if (arregloCodigoBarra.Length >= 8)
            {
                for (int posicion = 0; posicion <= 9; posicion++)
                {
                    if (posicion <= 8)
                        codigoQR += arregloCodigoBarra[posicion] + "|";
                    else
                        codigoQR += arregloCodigoBarra[posicion];
                }
            }

            firma = codigoQR;
            return firma;
        }

        private Bitmap GetImageFromByteArray(byte[] arregloBytes)
        {
            ImageConverter imageConverter = new ImageConverter();
            Bitmap bm = (Bitmap)imageConverter.ConvertFrom(arregloBytes);

            if (bm != null && (bm.HorizontalResolution != (int)bm.HorizontalResolution || bm.VerticalResolution != (int)bm.VerticalResolution))
            {
                bm.SetResolution((int)(bm.HorizontalResolution + 0.5F), (int)(bm.VerticalResolution + 0.5F));
            }

            return bm;
        }

        public byte[] GenerarCodigoQR(string certificado, int escala = 1)
        {
            try
            {
                byte[] ArregloCodigoBarras;
                string initialString = certificado;
                QRCode barcode = new QRCode();
                barcode.setData(initialString);
                barcode.setDataMode(QRCode.MODE_AUTO);
                barcode.setVersion(10);
                barcode.setEcl(QRCode.ECL_Q);
                barcode.setProcessTilde(true);
                barcode.setUOM(QRCode.UOM_INCH);
                barcode.setLeftMargin((float)0.874);
                barcode.setRightMargin((float)0.875);
                barcode.setTopMargin((float)0.05);
                barcode.setBottomMargin((float)0.05);
                barcode.setResolution(72);
                barcode.setModuleSize((float)0.04);
                ArregloCodigoBarras = barcode.renderBarcodeToBytes();

                MemoryStream memoria = new MemoryStream();
                Bitmap newBitmap = GetImageFromByteArray(ArregloCodigoBarras);
                newBitmap.Save(memoria, System.Drawing.Imaging.ImageFormat.Bmp);

                return memoria.ToArray();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
