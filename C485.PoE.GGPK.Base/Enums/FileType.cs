namespace C485.PoE.GGPK.Base.Enums
{
    //AST - https://wiki.multimedia.cx/index.php/FutureVision_audio_formats
    //BK2 - Bink2 video (100.0%)
    //DICT - shadercache file
    //FMT - https://wiki.multimedia.cx/index.php/FutureVision_audio_formats
    //GM - 	PrintFox/Pagefox bitmap (var. G) (100.0%), unknown file format - TODO
    //lnk - shortcut to some folders WTF, they are using SVN in 2019
    //MB - Maya Binary Scene (32bit) (100.0%), unknown file format, machine generated script with binary data - TODO
    //MTP - unknown
    //PJD - unknown file format - TODO - PassiveJewelDistanceList.pjd
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
}