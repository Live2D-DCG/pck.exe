using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PckTool
{
    public partial class FormMain : Form
    {
        const string LIST_FILE = "PckTool.list";

        private string CurrentFile { get; set; }

        public FormMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            imageList.Images.Add("pck", Properties.Resources.all);

            if (File.Exists(LIST_FILE))
            {
                var lines = File.ReadAllLines(LIST_FILE);
                if (lines.Length > 0)
                {
                    CurrentFile = lines[0];
                    if (!string.IsNullOrWhiteSpace(CurrentFile))
                    {
                        listViewPck.Items.Add(CurrentFile.Substring(CurrentFile.LastIndexOf(Path.DirectorySeparatorChar) + 1), "pck");
                    }
                    comboBoxRes.Items.AddRange(lines.Skip(1).ToArray());
                    comboBoxRes.SelectedIndex = comboBoxRes.Items.Count - 1;
                }
            }
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            int index = comboBoxRes.SelectedIndex;
            var text = comboBoxRes.Text;

            var list = new List<string>();
            list.Add(CurrentFile ?? "");
            for (int i = 0; i < comboBoxRes.Items.Count; i++)
            {
                if (i == index)
                {
                    continue;
                }
                list.Add((string)comboBoxRes.Items[i]);
            }
            if (!string.IsNullOrWhiteSpace(text))
            {
                list.Add(text);
            }
            if (list.Count > 0)
            {
                File.WriteAllLines(LIST_FILE, list.ToArray());
            }
        }

        string GetDropFile(DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            return files.Where(name => name.EndsWith(".pck", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }
        string GetDropDirectory(DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            return files.Where(name => Directory.Exists(name)).FirstOrDefault();
        }

        private void listViewPck_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var file = GetDropFile(e);
                e.Effect = file == null ? DragDropEffects.None : DragDropEffects.Copy;
            }
        }

        private void listViewPck_DragDrop(object sender, DragEventArgs e)
        {
            var file = GetDropFile(e);
            CurrentFile = file;
            listViewPck.Items.Clear();
            listViewPck.Items.Add(file.Substring(file.LastIndexOf(Path.DirectorySeparatorChar) + 1), "pck");
        }

        private void comboBoxRes_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var dir = GetDropDirectory(e);
                e.Effect = dir == null ? DragDropEffects.None : DragDropEffects.Copy;
            }
        }

        private void comboBoxRes_DragDrop(object sender, DragEventArgs e)
        {
            var dir = GetDropDirectory(e);
            comboBoxRes.Text = dir;
        }

        private void buttonPack_Click(object sender, EventArgs e)
        {
            string resDir;
            if (string.IsNullOrEmpty(CurrentFile))
            {
                MessageBox.Show(this, "Please select the original PCK file first.", "Packing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (string.IsNullOrWhiteSpace(resDir = comboBoxRes.Text))
            {
                MessageBox.Show(this, "Please select the res directory first.", "Packing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (!Directory.Exists(resDir))
            {
                MessageBox.Show(this, "Res directory does not exists.", "Packing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                var buffer = new byte[8];
                int count;

                var files = Directory.GetFiles(resDir);
                Array.Sort(files);
                var output = CurrentFile.Substring(0, CurrentFile.LastIndexOf(".pck", StringComparison.OrdinalIgnoreCase)) + ".replace.pck";

                using (var fsout = File.Open(output, FileMode.Create))
                {
                    Entry[] entries;

                    // header
                    using (var fsin = File.OpenRead(CurrentFile))
                    {
                        count = fsin.Read(buffer, 0, 8);
                        fsout.Write(buffer, 0, count);

                        var filecount = fsin.ReadInt();
                        if (filecount != files.Length)
                        {
                            throw new Exception("file counts error");
                        }
                        fsout.WriteInt(filecount);

                        entries = new Entry[filecount];
                        var offset = 12 + filecount * 25;
                        for (var i = 0; i < filecount; i++)
                        {
                            var entry = new Entry
                            {
                                Dummy = new byte[8],
                                Offset = offset,
                                Filename = files[i]
                            };
                            entry.OriginalSize = (int)new FileInfo(entry.Filename).Length;

                            var ext = entry.Filename.Substring(entry.Filename.LastIndexOf('.')).ToLower();
                            if (ext == ".txt" || ext == ".json" || ext == ".cpp" || ext == ".mtn")
                            {
                                entry.Data = Program.Yappy.Compress(File.ReadAllBytes(entry.Filename), 100);
                                entry.Size = entry.Data.Length;
                                entry.Flags = 1;
                            }
                            else
                            {
                                entry.Size = entry.OriginalSize;
                                entry.Flags = 0;
                            }
                            offset += entry.Size;
                            fsin.Read(entry.Dummy, 0, 8);
                            fsin.Seek(17, SeekOrigin.Current);

                            entries[i] = entry;
                        }
                    }

                    // write table-indexes
                    foreach (var entry in entries)
                    {
                        fsout.Write(entry.Dummy, 0, 8);
                        fsout.WriteByte(entry.Flags);
                        fsout.WriteInt(entry.Offset);
                        fsout.WriteInt(entry.Size);
                        fsout.WriteInt(entry.OriginalSize);
                        fsout.WriteInt(0);
                    }

                    // write content
                    const int DATA_SIZE = 4096;
                    var data = new byte[DATA_SIZE];
                    foreach (var entry in entries)
                    {
                        if (entry.Data != null)
                        {
                            fsout.Write(entry.Data, 0, entry.Size);
                        }
                        else
                        {
                            using (var fsin = File.OpenRead(entry.Filename))
                            {
                                while ((count = fsin.Read(data, 0, DATA_SIZE)) > 0)
                                {
                                    fsout.Write(data, 0, count);
                                }
                            }
                        }
                    }
                }

                MessageBox.Show(this, "Packaged successfully.", "Packed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
