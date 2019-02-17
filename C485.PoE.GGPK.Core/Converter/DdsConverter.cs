using Pfim;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;

namespace C485.PoE.GGPK.Core.Converter
{
    public static class DdsConverter
    {
        //Legacy format not supported, to view them use https://www.nvidia.pl/object/windows_texture_viewer.html
        //Nothing interesting there
        public static byte[] DdsToPng(byte[] bytes)
        {
            if (bytes[87] == 52) //We can simply change this bit from D3DFMT_DXT4 to D3DFMT_DXT5, they are the same
                bytes[87] = 53;
            if (bytes[87] == 50) //We can simply change this bit from D3DFMT_DXT2 to D3DFMT_DXT3, they are the same
                bytes[87] = 51;

            Dds dds = Pfim.Dds.Create(bytes, new PfimConfig());
            if (dds.Format == Pfim.ImageFormat.Rgba32)
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                    Image<Bgra32> image = Image.LoadPixelData<Bgra32>(
                        dds.Data, dds.Width, dds.Height);
                    image.SaveAsPng(memStream);
                    return memStream.GetBuffer();
                }
            }

            if (dds.Format == Pfim.ImageFormat.Rgb24)
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                    Image<Bgr24> image = Image.LoadPixelData<Bgr24>(
                        dds.Data, dds.Width, dds.Height);
                    image.SaveAsPng(memStream);
                    return memStream.GetBuffer();
                }
            }
            if (dds.Format == Pfim.ImageFormat.R5g5b5a1)
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                    Image<Bgra5551> image = Image.LoadPixelData<Bgra5551>(
                        dds.Data, dds.Width, dds.Height);
                    image.SaveAsPng(memStream);
                    return memStream.GetBuffer();
                }
            }

            throw new Exception("Unsupported pixel format (" + dds.Format + ")");
        }
    }
}