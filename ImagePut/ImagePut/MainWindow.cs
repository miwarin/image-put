using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace ImagePut
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();

            SetDefaultSource();
            SetDefaultDestination();
            SetDefaultSFTP();
        }

        private void txtSource_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            txtSource.Text = files[0];
        }

        private void txtDestination_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            txtDestination.Text = files[0];
        }

        private void txtSource_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void txtDestination_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void Output(String message)
        {
            txtOutput.AppendText(message + "\n");
        }

        // 転送
        private void btnTransfer_Click(object sender, EventArgs e)
        {
            Transfer();
        }

        private void Transfer()
        {
            try
            {
                ImageResize();
                FilenameToLower();
                Put();
                String tag = BuildTag();
                SetClipboard(tag);
            }
            catch (Exception e)
            {
                txtOutput.AppendText(e.ToString());
                Console.WriteLine("Error: {0}", e);
            }
        }

        private Boolean SetDefaultSource()
        {
            txtHostName.Text = Config.HostName;
            txtUserName.Text = Config.UserName;
            txtPassword.Text = Config.Password;
            txtFingerPrint.Text = Config.FingerPrint;
            return true;
        }

        private Boolean SetDefaultDestination()
        {
            String dfl_dir = "/home/rin/public_html/images";
            String today = Today();
            String dst_path = String.Format("{0}/{1}", dfl_dir, today);
            txtDestination.Text = dst_path;
            return true;
        }

        private Boolean SetDefaultSFTP()
        {
            txtSource.Text = Config.Source;
            return true;
        }

        private String Today()
        {
            DateTime d = DateTime.Today;
            String today = d.ToString("yyyy/MM/dd");
            return today;
        }

        private Boolean ImageResize()
        {
            String src_dir = txtSource.Text;
            String dst_dir = Path.Combine(src_dir, "s");
            List<String> files = CollectFiles(src_dir);
            foreach (String f in files)
            {
                Bitmap bmp = new Bitmap(f);
                Bitmap resizeBmp = ResizeImage(bmp, 400, 400);
                String src_file = Path.GetFileName(f);
                String dst_path = Path.Combine(dst_dir, src_file);
                Directory.CreateDirectory(dst_dir);
                resizeBmp.Save(dst_path);
                
                // Dipose もしないとファイルをロックしてしまう。後ほど Move できない
                bmp.Dispose();
                resizeBmp.Dispose();

            }
            return true;
        }

        private Boolean FilenameToLower()
        {
            String src_dir = txtSource.Text;
            string[] files = Directory.GetFiles(src_dir, "*", System.IO.SearchOption.AllDirectories);
            foreach (String file in files)
            {
                String finename = Path.GetFileName(file);
                String lowfile = finename.ToLower();
                String srcdir = Path.GetDirectoryName(file);
                String lowpath = Path.Combine(srcdir, lowfile);
                File.Move(file, lowpath);
            }
            return true;
        }

        private Boolean Put()
        {
            String hostname = txtHostName.Text;
            String username = txtUserName.Text;
            String password = txtPassword.Text;
            String finger = txtFingerPrint.Text;
            String src = txtSource.Text;
            String dst = txtDestination.Text;

            SCP scp = new SCP(hostname, username, password, finger);
            scp.MessageHandler += (sender, e) => { Output(e.Message); };
            scp.Put(src, dst);
            return true;
        }

        // {{'<a href="../images/2013/09/12/k03.png"><img src="../images/2013/09/12/s/k03.png"></a>'}}
        private String BuildTag()
        {
            String dst_dir = txtDestination.Text;
            String dst_dir2 = dst_dir.Replace("/home/rin/public_html", "..");
            String src_dir = txtSource.Text;
            List<String> files = CollectFiles(src_dir);
            List<String> tags = new List<String>();
            StringBuilder sb = new StringBuilder();

            foreach (String file in files)
            {
                String f = Path.GetFileName(file);
                String t = String.Format("{{{{'<a href=\"{0}/{1}\"><img src=\"{0}/s/{1}\"></a>'}}}}", dst_dir2, f);
                sb.AppendLine(t);
                sb.AppendLine("\n");
            }

            String tag = sb.ToString();
            return tag;
        }

        private Boolean SetClipboard(String text)
        {
            Clipboard.SetDataObject(text, true);
            return true;
        }

        private List<String> CollectFiles(String directory)
        {
            List<String> files = new List<String>();
            foreach (String f in Directory.GetFiles(directory, "*.*"))
            {
                files.Add(f);
            }
            return files;
        }

        public static Bitmap ResizeImage(Bitmap image, double dw, double dh)
        {
            double hi;
            double imagew = image.Width;
            double imageh = image.Height;

            if ((dh / dw) <= (imageh / imagew))
            {
                hi = dh / imageh;
            }
            else
            {
                hi = dw / imagew;
            }
            int w = (int)(imagew * hi);
            int h = (int)(imageh * hi);

            Bitmap result = new Bitmap(w, h);
            Graphics g = Graphics.FromImage(result);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(image, 0, 0, result.Width, result.Height);
            g.Dispose();

            return result;
        }

        private void txtSource_TextChanged(object sender, EventArgs e)
        {
            Config.Source = txtSource.Text;
        }

        private void txtDestination_TextChanged(object sender, EventArgs e)
        {
            Config.Destination = txtDestination.Text;
        }

        private void txtHostName_TextChanged(object sender, EventArgs e)
        {
            Config.HostName = txtHostName.Text;
        }

        private void txtUserName_TextChanged(object sender, EventArgs e)
        {
            Config.UserName = txtUserName.Text;
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            Config.Password = txtPassword.Text;
        }

        private void txtFingerPrint_TextChanged(object sender, EventArgs e)
        {
            Config.FingerPrint = txtFingerPrint.Text;
        }
    }
}
