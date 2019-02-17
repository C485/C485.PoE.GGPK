using C485.PoE.GGPK.Base.Enums;
using System.Linq;

namespace C485.PoE.GGPK.Base.Model
{
    public class FilePointer
    {
        public long FileDataOffset;
        public long FilePointerDataOffset;
        public long FilePointerOffset;

        public uint FileSize;
        public FileType FileType;
        public int FileExtensionHash;
        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                string[] split = value.Split('.');
                if (split.Length < 2)
                    return;
                string fileExtension = split.Last().ToLower();
                FileExtensionHash = fileExtension.GetHashCode();
                switch (fileExtension)
                {
                    case "tgt":
                    case "tgr":
                    case "xml":
                    case "ui":
                    case "gt":
                    case "fgp":
                    case "ffx":
                    case "edp":
                    case "ecf":
                    case "dlp":
                    case "ddt":
                    case "env":
                    case "et":
                    case "epk":
                    case "dgr":
                    case "mtd":
                    case "clt":
                    case "dct":
                    case "cht":
                    case "cfg":
                    case "filter":
                    case "fxgraph":
                    case "hideout":
                    case "gft":
                    case "sm":
                    case "mat":
                    case "ot":
                    case "hlsl"://Shader code
                    case "pet":
                    case "otc":
                    case "rs":
                    case "ao":
                    case "aoc":
                    case "ais":
                    case "arm":
                    case "amd":
                    case "slt":
                    case "act":
                    case "atlas":
                    case "properties":
                    case "red":
                    case "tst":
                    case "tsi":
                    case "trl":
                    case "json":
                    case "txt":
                        FileType = FileType.Txt;
                        break;

                    case "ogg":
                        FileType = FileType.Ogg;
                        break;

                    case "dat":
                        FileType = FileType.Dat;
                        break;

                    case "bank":
                        FileType = FileType.Riff;
                        break;

                    case "dds":
                        FileType = FileType.DdsCompressed;
                        break;

                    case "xls":
                        FileType = FileType.ExcelFile;
                        break;

                    case "png":
                        FileType = FileType.Png;
                        break;

                    case "dat64":
                        FileType = FileType.Dat64;
                        break;

                    case "spritefont":
                        FileType = FileType.SpriteFont;
                        break;

                    case "ttf":
                        FileType = FileType.Font;
                        break;

                    case "jpg":
                        FileType = FileType.Jpg;
                        break;

                    default:
                        FileType = FileType.Unknown;
                        break;
                }
            }
        }
    }
}