using System.Collections.Generic;

namespace C485.PoE.GGPK.Base.Model
{
    public class FileDirectoryTree
    {
        public List<FileDirectoryTree> Directories;
        public string DirectoryName;
        public List<FilePointer> Files;
    }
}