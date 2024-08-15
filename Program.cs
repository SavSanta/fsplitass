using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace fsplitass
{
    class Program
    {
        private const int BUFFER_SIZE = 10 * 1024 * 1024;
        private const string APPVERSION = "1.0.2408.1";

        static void Main(string[] args)
        {

            if (args.Length < 3)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                PrintVersion();
                Console.Write("Syntax error: fsplitass.exe <target_filename> <destination_folder> <max_bytes_per_split>\n" +
                    " Example:\n # Split file into approx 25 megabytes subfiles \nfsplitass.exe AliceNightly.img Alice_Split 25000000\n" +
                    " Example:\n # Split file into approx 5 gigabyte subfiles \nfsplitass.exe ServerDataBakup.vdisk D:\\ServerChunks 5000000000");
                Console.ResetColor();
                return;
            }

            string targFile = args[0];
            string outDir = args[1];
            string targSize = args[2];

            targSize = targSize.Replace(",", "").Replace(".", "").Trim();

            if (!File.Exists(targFile))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERR: File Not Found: [{0}]", targFile);
                Console.ResetColor();
                return;
            }

            long bytesSplitSize = 0;
            if (!Int64.TryParse(targSize, out bytesSplitSize))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERR: Could not obtain a splittable integer value: [{0}]", targSize);
                Console.ResetColor();
                return;
            }

            // 10,000,000,000 bytes is 10 GB. Das quick maffs, bruv.
            if (bytesSplitSize > 1.0000E10)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("NOTE: Operator chose split files into rather large chunks (greater than 10 GB). Verify a good size for transfer.", targSize);
                Console.ResetColor();
            }

            Console.WriteLine();
            try
            {
                SplitFile(targFile, outDir, bytesSplitSize);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("FATALERR: {0}", ex.Message);
                Console.ResetColor();
                return;
            }
        }


        public static void PrintVersion()
        {
            Console.WriteLine("fsplitass.exe {0}", APPVERSION);
        }

        public static void SplitFile(string srcFileName, string outDirPath, long subFileSize)
        {
            if (string.IsNullOrEmpty(srcFileName)
               || string.IsNullOrEmpty(outDirPath)
               || !File.Exists(srcFileName)
               || subFileSize <= 0)
            {
                throw new Exception("File Split parameters wrong!");
            }

            if (!Directory.Exists(outDirPath))
            {
                Console.WriteLine("[{0}] does not exist. Trying to create it!", outDirPath);
                try
                {
                    Directory.CreateDirectory(outDirPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Could not create folder [{0}], error with: " + ex.Message, outDirPath);
                    throw new Exception("File Split parameters wrong!");
                }
            }

            Console.WriteLine("Spliting the file [{0}] to [{1}] with file size [{2}] bytes.", srcFileName, outDirPath, subFileSize);

            IList<String> lstSubFiles = GetSubFileName(srcFileName, outDirPath, subFileSize);

            Console.WriteLine("Spliting to [{0}] sub files.", lstSubFiles.Count);

            FileStream orgFile = new FileStream(srcFileName, FileMode.Open, FileAccess.Read);

            long remainingFileSize = orgFile.Length;

            foreach (String subFile in lstSubFiles)
            {
                Console.WriteLine("Splitting [{0}].", subFile);

                if (subFileSize > remainingFileSize)
                {
                    subFileSize = remainingFileSize;
                }

                CopyToSubFile(orgFile, subFile, subFileSize, BUFFER_SIZE);

                remainingFileSize -= subFileSize;
            }

            Console.WriteLine("Split Routine Complete.");
        }


        public static IList<String> GetSubFileName(String orgFileName, String outPath, long subFileSize)
        {
            IList<String> lstSubFileName = new List<String>();

            // This looks wrong in terms of the process checks and bail.
            // Not sure how'd I will want to rewrite in the future but it definitely should crash under these conditions
            if (string.IsNullOrEmpty(orgFileName)
                || string.IsNullOrEmpty(outPath)
                || !File.Exists(orgFileName)
                || !Directory.Exists(outPath)
                || subFileSize <= 0)
            {
                return lstSubFileName;
            }

            FileInfo orgFileInfo = new FileInfo(orgFileName);

            long orgFileSize = orgFileInfo.Length;

            // Get possible number of sub files
            int numSubFile = (int)(orgFileSize / subFileSize);
            if (orgFileSize % subFileSize > 0)
            {
                numSubFile++;
            }

            // Sub file extention pattern
            /* // Original Method Which Works Fine For Adding Leading Zeros if The Total SplitFiles Will be 10 or greater. Though, less than 10 it doesnt produce a leading zero per OPSEC request of an opertator
            int subFileExtLen = numSubFile.ToString().Length;
            String subFileExt = "";
            for (int ind = 0; ind < subFileExtLen; ind++)
            {
                subFileExt += "0";
            }
            */

            // List of sub files
            outPath += "\\" + orgFileInfo.Name + ".";

            for (int ind = 1; ind <= numSubFile; ind++)
            {
                //String subFileName = outPath + ind.ToString(subFileExt);
                String subFileName = outPath + ind.ToString("00");
                lstSubFileName.Add(subFileName);
            }

            return lstSubFileName;
        }


        public static void CopyToSubFile(FileStream fileStreamIn, String subFileName, long totalSubFileSize, int bufferSize)
        {
            if (null == fileStreamIn
                || string.IsNullOrEmpty(subFileName)
                || totalSubFileSize <= 0
                || bufferSize <= 0)
            {
                return;
            }

            FileStream fileStreamOut = File.Create(subFileName);

            fileStreamOut.SetLength(totalSubFileSize);

            long remainingSubFileSize = totalSubFileSize;
            int byteRead = -1;

            byte[] buffer = new byte[bufferSize];
            // Copy to sub file
            while (remainingSubFileSize > 0 || byteRead == 0)
            {
                if (bufferSize > remainingSubFileSize)
                {
                    bufferSize = (int)remainingSubFileSize;
                }

                byteRead = fileStreamIn.Read(buffer, 0, bufferSize);

                fileStreamOut.Write(buffer, 0, byteRead);

                remainingSubFileSize -= byteRead;
            }

            fileStreamOut.Flush();
            fileStreamOut.Close();
        }

        public static void JoinSubFiles()
        {
            throw new NotImplementedException();
            /* // Operator can manually run the following to re-join the files.
            Console.WriteLine("Options");
            Console.Write("\n\n1.[pshw] Get-Content -Raw file1, file2 | Set-Content -NoNewline destination\n2.[mscmd]copy file1 /b + file2 /b + file3 /b destfile");
            */
        }

    }
}
