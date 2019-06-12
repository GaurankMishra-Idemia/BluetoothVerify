
using System;
using System.Drawing;
using ZXing;
using ZXing.PDF417;


namespace PDF417_CS
{
    class Program
    {
        static void Main(string[] args)
        {
            
            // create a barcode reader instance
            var barcodeReader = new BarcodeReader();
            BarcodeFormat format = BarcodeFormat.PDF_417;
            //barcodeReader.Options.PossibleFormats = format;
            
            // create an in memory bitmap            
            var barcodeBitmap = (Bitmap)Bitmap.FromFile("pdf417_edit.bmp");   // pdf4, pdf8, pdf417_edit is working
            //var barcodeBitmap = (Bitmap)Bitmap.FromFile("pdf8.bmp");
            //var barcodeBitmap = (Bitmap)Bitmap.FromFile("qr.bmp");

            // decode the barcode from the in memory bitmap
            var barcodeResult = barcodeReader.Decode(barcodeBitmap);

            // output results to console
            Console.WriteLine($"Decoded barcode text: {barcodeResult?.Text}");
            Console.WriteLine($"\n\nBarcode format: {barcodeResult?.BarcodeFormat}");                        

        }
    }
}
