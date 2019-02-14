using System.IO;

namespace C485.PoE.GGPK.Core.Extension
{
    public static class BinaryReaderExtension
    {
        public static void SkipBytes(this BinaryReader binReader, long bytes)
        {
            binReader.BaseStream.Seek(bytes, SeekOrigin.Current);
        }
    }
}