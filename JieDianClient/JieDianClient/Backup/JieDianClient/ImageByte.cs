using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;

namespace CoapClient
{
    class ImageByte
    {
        //FileStream fsByte = new FileStream(@"e:\a.jpg",FileMode.Open);
                                //byte[] imbyte = new byte[fsByte.Length];
                                //for (int i = 0; i < fsByte.Length; i++)
                                //    imbyte[i] = (byte)fsByte.ReadByte();                                                              
        //private MemoryStream mStream;// = new MemoryStream(response.Payload);
        //private Image image;

        //public static byte[] ImageToByte(Image im) {
        //    FileStream fsByte = new FileStream(im,FileMode.Open);    
        //    byte[] by=new byte[im.];
        //    return;
        //}
        public static byte[] ImageToByte(string str)
        {
            FileStream fsByte = new FileStream(str, FileMode.Open);
            byte[] imbyte = new byte[fsByte.Length];
            for (int i = 0; i < fsByte.Length; i++)imbyte[i] = (byte)fsByte.ReadByte();
            return imbyte;
        }    
        public static Image ByteToImage(byte[] by) {
            MemoryStream mStream= new MemoryStream(by);
            return Image.FromStream(mStream);      
        }
    }
}
