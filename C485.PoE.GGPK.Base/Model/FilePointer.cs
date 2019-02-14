namespace C485.PoE.GGPK.Base.Model
{
    public class FilePointer
    {
        public long FilePointerDataOffset;
        public long FilePointerOffset;
        public string Name;
        public long FileDataOffset { get; set; }
        public uint FileSize { get; set; }
    }
}