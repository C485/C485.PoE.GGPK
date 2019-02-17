using System.Linq;

namespace C485.PoE.GGPK.Base.Model
{
    //AST - https://wiki.multimedia.cx/index.php/FutureVision_audio_formats
    //BK2 - Bink2 video (100.0%)
    //DICT - shadercache file
    //FMT - https://wiki.multimedia.cx/index.php/FutureVision_audio_formats
    //GM - 	PrintFox/Pagefox bitmap (var. G) (100.0%), unknown file format - TODO
    //lnk - shortcut to some folders WTF, they are using SVN in 2019
    //MB - Maya Binary Scene (32bit) (100.0%), unknown file format, machine generated script with binary data - TODO
    //MTP - unknown
    //PJD - unknown file format - TODO
    //PSG - https://github.com/OmegaK2/PyPoE/blob/dev/PyPoE/poe/file/psg.py
    //SMD - unknown file format - TODO
    //TDT - unknown file format, have some file names in it - TODO/
    //TGM - unknown file format - TODO
    //TMD - unknown file format - TODO

    public enum FileType
    {
        Txt,
        ExcelFile,
        SpriteFont, //Direct3D 11 implementation of a bitmap font renderer
        Dat,
        Font,
        Png, //Image
        Jpg, //Image
        DdsCompressed, //Compressed dds file
        DdsUncompressed, //Uncompressed dds file
        DdsLookup, //Dds lookup file, simple txt file with paths to other dds
        Dat64,
        Ogg, //Audio
        Riff, //Resource Interchange File Format, initial release August 1991!! https://www.menasoft.com/blog/?p=34
        Unknown
    }

    public class FilePointer
    {
        public long FileDataOffset;
        public long FilePointerDataOffset;
        public long FilePointerOffset;

        public uint FileSize;
        public FileType FileType;
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
                switch (split.Last().ToLower())
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