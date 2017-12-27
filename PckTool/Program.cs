using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PckTool
{
    struct Entry
    {
        public byte[] Dummy;
        public byte Flags;
        public int Offset;
        public int Size;
        public int OriginalSize;
        public int Less;

        public string Filename;
        public byte[] Data;

        public int DataSize => Size - Less;
    }

    static class Program
    {
        public static readonly Yappy Yappy = new Yappy();

        private static readonly byte[] DCKey =
        {
            0x37, 0xea, 0x79, 0x85, 0x86, 0x29, 0xec, 0x94,
            0x85, 0x20, 0x7c, 0x1a, 0x62, 0xc3, 0x72, 0x4f,
            0x72, 0x75, 0x25, 0x0b, 0x99, 0x99, 0xbd, 0x7f,
            0x0b, 0x24, 0x9a, 0x8d, 0x85, 0x38, 0x0e, 0x39
        };
        private static readonly Aes AES = new AesManaged
        {
            Key = DCKey,
            Mode = CipherMode.ECB,
            Padding = PaddingMode.Zeros
        };

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string file;
            if (args.Length > 0 && (file = args.Where(f => f.EndsWith(".pck", StringComparison.OrdinalIgnoreCase)).FirstOrDefault()) != null)
            {
                var dir = file.Substring(0, file.LastIndexOf(".pck", StringComparison.OrdinalIgnoreCase));
                if (Directory.Exists(dir))
                {
                    if (MessageBox.Show("Directory exists, overwrite?", "Extract", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    {
                        return;
                    }
                }
                else
                {
                    Directory.CreateDirectory(dir);
                }
                ExtractFiles(file, dir);
                MessageBox.Show("Extract successfully.", "Extract", MessageBoxButtons.OK, MessageBoxIcon.Information);

                //var info = new ProcessStartInfo();
                //info.FileName = "explorer.exe";
                //info.Arguments = string.Format("/select, \"{0}\"", dir);
                //Process.Start(info);
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FormMain());
            }
        }

        #region - Extension -
        public static int ReadInt(this Stream stream)
        {
            var buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            return BitConverter.ToInt32(buffer, 0);
        }

        public static void WriteInt(this Stream stream, int value)
        {
            var buffer = BitConverter.GetBytes(value);
            stream.Write(buffer, 0, 4);
        }
        #endregion

        private static void ExtractFiles(string pckfile, string directory)
        {
            using (var fsin = File.OpenRead(pckfile))
            {
                fsin.Seek(8, SeekOrigin.Begin);

                var filecount = fsin.ReadInt();
                var entries = new Entry[filecount];
                for (var i = 0; i < filecount; i++)
                {
                    var entry = new Entry
                    {
                        Dummy = new byte[8]
                    };
                    fsin.Read(entry.Dummy, 0, 8);
                    entry.Flags = (byte)fsin.ReadByte();
                    entry.Offset = fsin.ReadInt();
                    entry.Size = fsin.ReadInt();
                    entry.OriginalSize = fsin.ReadInt();
                    entry.Less = fsin.ReadInt();

                    entries[i] = entry;
                }

                // extract
                for (var i = 0; i < filecount; i++)
                {
                    var entry = entries[i];
                    var outname = string.Format("{0:0000}", i); //, Convert.ToBase64String(entry.Dummy).Replace('/', '-'));
                    fsin.Seek(entry.Offset, SeekOrigin.Begin);
                    var data = new byte[entry.Size];
                    fsin.Read(data, 0, data.Length);
                    if ((entry.Flags & 2) == 2)
                    {
                        // decrypt
                        data = DecryptData(data);
                    }
                    if ((entry.Flags & 1) == 1)
                    {
                        // decompress
                        data = Yappy.Uncompress(data, entry.DataSize, entry.OriginalSize);
                    }
                    outname += GetExtName(data);
                    File.WriteAllBytes(Path.Combine(directory, outname), data);
                }
            }
        }

        private static byte[] DecryptData(byte[] data)
        {
            using (var transform = AES.CreateDecryptor())
            {
                var result = transform.TransformFinalBlock(data, 0, data.Length);
                //AES.Clear();
                return result;
            }
        }

        private static byte[] EncryptData(byte[] data)
        {
            using (var transform = AES.CreateEncryptor())
            {
                var result = transform.TransformFinalBlock(data, 0, data.Length);
                AES.Clear();
                return result;
            }
        }

        private static bool MatchID(byte[] data, string id)
        {
            if (data.Length < id.Length)
            {
                return false;
            }
            for (var i = 0; i < id.Length; i++)
            {
                if (data[i] != id[i])
                {
                    return false;
                }
            }
            return true;
        }

        private static string GetExtName(byte[] data)
        {
            if (data.Length > 0)
            {
                if (MatchID(data, "moc\x0a") || MatchID(data, "moc\x09"))
                {
                    return ".moc";
                }
                else if (MatchID(data, "\x89PNG"))
                {
                    return ".png";
                }
                else if (MatchID(data, "\xef\xbb\xbf"))
                {
                    return ".txt";
                }
                else if (MatchID(data, "# Live2D Animator Motion Data"))
                {
                    return ".mtn";
                }
                else if (MatchID(data, "{\n") || MatchID(data, "{\r"))
                {
                    return ".json";
                }
            }
            return ".dat";
        }
    }
}
