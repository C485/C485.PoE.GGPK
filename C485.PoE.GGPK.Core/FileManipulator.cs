using BrotliSharpLib;
using C485.PoE.GGPK.Base.Enums;
using C485.PoE.GGPK.Base.Model;
using C485.PoE.GGPK.Core.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace C485.PoE.GGPK.Core
{
    public class FileManipulator
    {
        private const int HashSizeInBytes = 32;
        private const int SizeOfHeader = sizeof(int) + SizeOfTag;
        private const int SizeOfTag = 4;
        private const int WideZeroTerminatorSizeInBytes = 2;
        private readonly Dictionary<long, FileDirectory> _directories;
        private readonly long _fileLength;
        private readonly Dictionary<long, FilePointer> _files;
        private readonly FileStream _fileStream;
        private long? _emptyNameFolderOffset;
        private FileDirectoryTree _root;

        public FileManipulator(string filePath)
        {
            _fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.RandomAccess);
            _directories = new Dictionary<long, FileDirectory>(50000);
            _files = new Dictionary<long, FilePointer>(500000);
            _fileLength = _fileStream.Length;
        }

        ~FileManipulator()
        {
            _fileStream.Dispose();
        }

        public IReadOnlyDictionary<long, FileDirectory> Directories
        {
            get
            {
                if (_directories == null || _directories.Count == 0)
                    throw new Exception("MapFile was not called!");
                return _directories;
            }
        }

        public IReadOnlyDictionary<long, FilePointer> Files
        {
            get
            {
                if (_files == null || _files.Count == 0)
                    throw new Exception("MapFile was not called!");
                return _files;
            }
        }

        public FileDirectoryTree Root
        {
            get
            {
                if (_root == null)
                    throw new Exception("MapFile was not called!");
                return _root;
            }
        }

        public byte[] GetFileContentBytes(FilePointer filePointer, bool brotliDecompress = false)
        {
            //This is kind of dangerous, we can reach limit of ram
            byte[] bytes = new byte[filePointer.FileSize];
            long totalReadBytes = 0;
            long bytesToRead = filePointer.FileSize;
            using (BinaryReader binReader = new BinaryReader(_fileStream, new UTF8Encoding(), true))
            {
                binReader.BaseStream.Seek(filePointer.FileDataOffset, SeekOrigin.Begin);
                while (bytesToRead > 0)
                {
                    int chunk = unchecked((int)bytesToRead);
                    if (bytesToRead > int.MaxValue)
                        chunk = int.MaxValue;
                    bytesToRead -= chunk;
                    byte[] readBytes = binReader.ReadBytes(chunk);

                    Array.Copy(readBytes, 0, bytes, totalReadBytes, chunk);
                    totalReadBytes += chunk;
                }
            }
            return brotliDecompress ? Brotli.DecompressBuffer(bytes, 4, bytes.Length - 4) : bytes;
        }

        public void MapFile()
        {
            _directories.Clear();
            _files.Clear();

            using (BinaryReader binReader = new BinaryReader(_fileStream, new UTF8Encoding(), true))
            {
                binReader.BaseStream.Seek(0, SeekOrigin.Begin);
                while (_fileLength != binReader.BaseStream.Position)
                {
                    long fileOffset = binReader.BaseStream.Position;
                    long dataOffset = fileOffset + SizeOfHeader;
                    ParseHeaderData(binReader, SizeOfHeader, out PackType packType, out uint dataLength);

                    switch (packType)
                    {
                        case PackType.PDir:
                            ParsePDir(binReader, dataLength, fileOffset, dataOffset);
                            break;

                        case PackType.File:
                            ParseFilePointer(binReader, dataLength, fileOffset, dataOffset);
                            break;

                        default:
                            binReader.SkipBytes(dataLength - SizeOfHeader);
                            break;
                    }
                }
            }
            _root = GetTreeWithRoot();
        }

        private static void CheckForDds(BinaryReader binReader, FilePointer fp, uint fileDataSize)
        {
            if (fp.FileType == FileType.DdsCompressed)
            {
                byte[] cntBytes = binReader.ReadBytes(4);
                if (cntBytes[0] == '*')
                    fp.FileType = FileType.DdsLookup;
                else if (cntBytes[0] == 'D' && cntBytes[1] == 'D' && cntBytes[2] == 'S' && cntBytes[3] == ' ') //MagicNumber 0x20534444 ("DDS ")
                    fp.FileType = FileType.DdsUncompressed;
                binReader.SkipBytes(fileDataSize - 4);
            }
            else
            {
                binReader.SkipBytes(fileDataSize);
            }
        }

        private static PackType GetPackType(byte[] data)
        {
            if (data[4] == 'F' && data[5] == 'I' && data[6] == 'L' && data[7] == 'E')
                return PackType.File;
            if (data[4] == 'G' && data[5] == 'G' && data[6] == 'P' && data[7] == 'K')
                return PackType.Ggpk;
            if (data[4] == 'F' && data[5] == 'R' && data[6] == 'E' && data[7] == 'E')
                return PackType.Free;
            if (data[4] == 'P' && data[5] == 'D' && data[6] == 'I' && data[7] == 'R')
                return PackType.PDir;
            throw new Exception($"Unknown PackType[{Encoding.ASCII.GetString(data, 4, 4)}]");
        }

        private static void ParseHeaderData(BinaryReader binReader, int sizeOfHeader, out PackType packType, out uint dataLength)
        {
            byte[] headerData = binReader.ReadBytes(sizeOfHeader);
            dataLength = BitConverter.ToUInt32(headerData);
            packType = GetPackType(headerData);
        }

        private List<FilePointer> GetFiles(List<long> fileOffsets)
        {
            List<FilePointer> files = new List<FilePointer>();
            foreach (long fileOffset in fileOffsets)
            {
                bool it = _files.ContainsKey(fileOffset);
                if (!it)
                    continue;
                files.Add(_files[fileOffset]);
            }

            return files;
        }

        private FileDirectory GetRootDirectory()
        {
            if (_emptyNameFolderOffset != null && _directories.ContainsKey(_emptyNameFolderOffset.Value))
                return _directories[_emptyNameFolderOffset.Value];
            throw new Exception("Root directory not found!");
        }

        private List<FileDirectoryTree> GetTree(FileDirectory root)
        {
            List<FileDirectoryTree> tree = new List<FileDirectoryTree>();
            foreach (long item in root.FilesOffset)
            {
                bool it = _directories.ContainsKey(item);
                if (!it)
                    continue;
                FileDirectory directory = _directories[item];
                tree.Add(new FileDirectoryTree
                {
                    Directories = GetTree(directory),
                    DirectoryName = directory.DirectoryName,
                    Files = GetFiles(directory.FilesOffset)
                });
            }

            return tree;
        }

        private FileDirectoryTree GetTreeWithRoot()
        {
            FileDirectory root = GetRootDirectory();
            return new FileDirectoryTree
            {
                Directories = GetTree(root),
                DirectoryName = "ROOT",
                Files = GetFiles(root.FilesOffset)
            };
        }

        private void ParseFilePointer(BinaryReader binReader, uint dataLength, long fileOffset, long dataOffset)
        {
            int nameLength = binReader.ReadInt32();
            byte[] data = binReader.ReadBytes(HashSizeInBytes + 2 * nameLength);
            long fileDataOffset = binReader.BaseStream.Position;
            using (MemoryStream stream = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    reader.SkipBytes(HashSizeInBytes);
                    string name = Encoding.Unicode.GetString(reader.ReadBytes(2 * (nameLength - 1)));
                    reader.SkipBytes(WideZeroTerminatorSizeInBytes);
                    uint fileDataSize = dataLength - HashSizeInBytes - (uint)(2 * nameLength) - sizeof(int) - SizeOfHeader;
                    FilePointer fp = new FilePointer
                    {
                        Name = name,
                        FilePointerDataOffset = dataOffset,
                        FilePointerOffset = fileOffset,
                        FileSize = fileDataSize,
                        FileDataOffset = fileDataOffset
                    };
                    CheckForDds(binReader, fp, fileDataSize);
                    _files.Add(fileOffset, fp);
                }
            }
        }

        private void ParsePDir(BinaryReader binReader, uint dataLength, long fileOffset, long dataOffset)
        {
            byte[] data = binReader.ReadBytes((int)dataLength - SizeOfHeader);

            using (MemoryStream stream = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    int nameLength = reader.ReadInt32();
                    int filesCount = reader.ReadInt32();
                    reader.SkipBytes(HashSizeInBytes);
                    string name = Encoding.Unicode.GetString(reader.ReadBytes(2 * (nameLength - 1)));
                    reader.SkipBytes(WideZeroTerminatorSizeInBytes);
                    FileDirectory res = new FileDirectory
                    {
                        DirectoryName = name,
                        DataOffset = dataOffset,
                        FileOffset = fileOffset
                    };
                    for (int i = 0; i < filesCount; i++)
                    {
                        reader.SkipBytes(sizeof(int));//entity hash name
                        long offset = reader.ReadInt64();
                        res.FilesOffset.Add(offset);
                    }
                    _directories.Add(fileOffset, res);
                    if (!_emptyNameFolderOffset.HasValue && nameLength == 1)
                        _emptyNameFolderOffset = fileOffset;
                    else if (_emptyNameFolderOffset.HasValue && nameLength == 1)
                        throw new Exception("Found another empty name directory, unable to find root!");
                    if (reader.BaseStream.Length != reader.BaseStream.Position)
                        throw new Exception($"ParsePDir, some data left, new data format?");
                }
            }
        }
    }
}