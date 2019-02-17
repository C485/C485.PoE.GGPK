using C485.PoE.GGPK.Base.Model;
using C485.PoE.GGPK.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

//Here you can find most non optimized code in whole universe :D
namespace C485.PoE.GGPK.Playground
{
    internal class Program
    {
        public static StringBuilder sb = new StringBuilder(1024 * 24);
        private static FileManipulator kk = new FileManipulator("E:\\POE\\Content.ggpk");

        public static void FilesCountByExtension()
        {
            var files = kk.Files.Values.Where(x => x.Name.Contains('.')).GroupBy(x => x.Name.ToLower().Split('.').Last()).Select(x => new
            {
                key = x.Key,
                filesCount = x.Count()
            }).OrderByDescending(x => x.filesCount).ToList();

            foreach (var filePointer in files)
            {
                Console.WriteLine($"{filePointer.key} - {filePointer.filesCount}");
            }
        }

        public static void PrintTree(FileDirectoryTree root, int dep)
        {
            sb.AppendFormat("{0}>{1}\n", new string('\t', dep), root.DirectoryName);
            foreach (FilePointer filePointer in root.Files)
            {
                sb.AppendFormat("{0}{1}\n", new string('\t', dep + 1), filePointer.Name);
            }
            foreach (FileDirectoryTree item in root.Directories)
            {
                PrintTree(item, dep + 1);
            }
        }

        public static void UnpackAll(FileDirectoryTree root, string path)
        {
            foreach (FilePointer filePointer in root.Files)
            {
                byte[] buf = kk.GetFileContentBytes(filePointer);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                File.WriteAllBytes(path + "/" + filePointer.Name, buf);
            }
            foreach (FileDirectoryTree item in root.Directories)
            {
                UnpackAll(item, path + "/" + item.DirectoryName);
            }
        }

        public static void UnpackAllDDSFiles(string path)
        {
            List<FilePointer> files = kk.Files.Values.Where(x => x.FileType == FileType.DdsCompressed || x.FileType == FileType.DdsUncompressed).ToList();
            int i = 0;

            foreach (FilePointer pointer in files)
            {
                byte[] buf = kk.GetFileContentBytes(pointer, pointer.FileType == FileType.DdsCompressed);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                if (File.Exists(path + "/" + pointer.Name))
                    File.WriteAllBytes(path + "/" + (i++) + pointer.Name, buf);
                else
                    File.WriteAllBytes(path + "/" + pointer.Name, buf);
            }
        }

        public static void UnpackFirst10OfAnyExtension(string path)
        {
            var files = kk.Files.Values.Where(x => x.Name.Contains('.') && x.FileType == FileType.Unknown).GroupBy(x => x.Name.ToLower().Split('.').Last()).Select(x => new
            {
                key = x.Key,
                files = x.GroupBy(p => p.Name).Select(p => p.First()).Take(100).ToList()
            }).ToList();

            foreach (var filePointer in files)
            {
                foreach (FilePointer pointer in filePointer.files)
                {
                    byte[] buf = kk.GetFileContentBytes(pointer, pointer.FileType == FileType.DdsCompressed);
                    if (!Directory.Exists(path + "/" + filePointer.key))
                        Directory.CreateDirectory(path + "/" + filePointer.key);
                    File.WriteAllBytes(path + "/" + filePointer.key + "/" + pointer.Name, buf);
                }
            }
        }

        private static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();

            sw.Start();
            Console.WriteLine("File parse start");
            kk.MapFile();

            sw.Stop();

            Console.WriteLine("File parse stop, Elapsed={0}", sw.Elapsed);

            PrintTree(kk.Root, 0);
            //System.IO.File.AppendAllText("E:\\lol.txt", sb.ToString());
            UnpackFirst10OfAnyExtension("E:/PoEUnpacked3_ext");
            //FilesCountByExtension();
            //UnpackAllDDSFiles("E:/PoEUnpacked4_ext");
        }
    }
}