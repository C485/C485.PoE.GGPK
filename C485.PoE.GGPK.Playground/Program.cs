using C485.PoE.GGPK.Base.Model;
using C485.PoE.GGPK.Core;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace C485.PoE.GGPK.Playground
{
    internal class Program
    {
        public static StringBuilder sb = new StringBuilder(1024 * 24);
        private static FileManipulator kk = new FileManipulator("E:\\POE\\Content.ggpk");

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
            UnpackAll(kk.Root, "E:/PoEUnpacked2");
        }

        public static void UnpackAll(FileDirectoryTree root, string path)
        {
            foreach (FilePointer filePointer in root.Files)
            {
                if(!filePointer.Name.EndsWith(".dat"))
                    continue;
                
                var buf = kk.GetFileContentBytes(filePointer);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                File.WriteAllBytes(path + "/" + filePointer.Name, buf);
            }
            foreach (FileDirectoryTree item in root.Directories)
            {
                UnpackAll(item, path + "/" + item.DirectoryName);
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
    }
}