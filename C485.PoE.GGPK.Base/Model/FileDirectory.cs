using System.Collections.Generic;

namespace C485.PoE.GGPK.Base.Model
{
    public class FileDirectory
    {
        public long DataOffset;
        public string DirectoryName;
        public long FileOffset;
        public List<long> FilesOffset;

        public FileDirectory()
        {
            FilesOffset = new List<long>();
        }
    }
}