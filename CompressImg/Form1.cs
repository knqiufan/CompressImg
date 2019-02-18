using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Threading;
//Download by http://www.codefans.net
namespace CompressImg
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        FileSystemInfo[] fsi=null;
        string ImgPath = "";
        ArrayList al = new ArrayList();
        string ImgSavePath = "";
        string strSourcePath = "";
        Thread td;
        Image ig = null;
        string strSavePath = "";
        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
        }

        /// <summary>
        /// 无损图片缩放
        /// </summary>
        /// <param name="sFile">图片的原始路径</param>
        /// <param name="dFile">缩放后图片的保存路径</param>
        /// <param name="compRatio">图片压缩比例</param>
        /// <returns></returns>

        public static bool GetPicThumbnail(string sFile, string dFile, long compRatio)
        {
            Image img = Image.FromFile(sFile);
            //以下代码为保存图片时，设置压缩质量
            EncoderParameters ep = new EncoderParameters();
            long[] qy = new long[1];
            qy[0] = compRatio;//设置压缩的比例1-100 
            EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
            ep.Param[0] = eParam;
            try
            {
                ImageCodecInfo jpegICIinfo = GetEncoderInfo("JPG");
                if (jpegICIinfo != null)
                {
                    img.Save(dFile, jpegICIinfo, ep);//dFile是压缩后的新路径
                }
                else
                {
                    img.Save(dFile, System.Drawing.Imaging.ImageFormat.Jpeg);
                }

            }
            catch (Exception)
            {
                img.Save(dFile, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            return true;
        }
        public static ImageCodecInfo GetEncoderInfo(String fileExtenUper)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].FilenameExtension.Contains(fileExtenUper))
                {
                    return encoders[j];
                }
            }
            return null;
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                fsi = null;
                al.Clear();
                txtPicPath.Text = folderBrowserDialog1.SelectedPath;
                ImgPath = txtPicPath.Text.Trim();
                DirectoryInfo di=new DirectoryInfo(txtPicPath.Text);
                fsi = di.GetFileSystemInfos();
                for (int i = 0; i < fsi.Length; i++)
                {
                    string ofile=fsi[i].ToString();
                    string fileType = ofile.Substring(ofile.LastIndexOf(".") + 1, ofile.Length - ofile.LastIndexOf(".")-1);
                    fileType = fileType.ToLower();
                    if (fileType == "jpeg" || fileType == "jpg" || fileType == "bmp" || fileType == "png")
                    {
                        al.Add(ofile);
                    }
                }
                lblPicNum.Text = al.Count.ToString();
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog2.ShowDialog() == DialogResult.OK)
            {
                txtSavePath.Text = folderBrowserDialog2.SelectedPath;
                ImgSavePath = txtSavePath.Text.Trim();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void rbPercent_Enter(object sender, EventArgs e)
        {
            groupBox1.Focus();
        }

        private void rbResolving_Enter(object sender, EventArgs e)
        {
            groupBox1.Focus();
        }

        private void CompleteIMG()
        {
            progressBar1.Maximum = al.Count;
            progressBar1.Minimum = 1;
            if (ImgPath.Length == 3)
                strSourcePath = ImgPath;
            else
                strSourcePath = ImgPath + "\\";

            if (ImgSavePath.Length == 3)
                strSavePath = ImgSavePath;
            else
                strSavePath = ImgSavePath + "\\";
            for (int i = 0; i < al.Count; i++)
            {
                ig = Image.FromFile(strSourcePath + al[i].ToString());
                if (rbPercent.Checked)
                {
                    GetPicThumbnail(strSourcePath + al[i].ToString(), strSavePath + al[i].ToString(), Convert.ToInt64(numericUpDown1.Value));
                }
                ig.Dispose();
                progressBar1.Value = i+1;
                lblComplete.Text = Convert.ToString(i+1);
            }
            if (lblComplete.Text == al.Count.ToString())
            {
                MessageBox.Show("压缩成功","提示",MessageBoxButtons.OK,MessageBoxIcon.Information);
                progressBar1.Value = 1;
                pictureBox1.Enabled = true;
                pictureBox2.Enabled = true;
                rbPercent.Enabled = true;
                lblPicNum.Text = "0";
                lblComplete.Text = "0";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtPicPath.Text.Trim() != "" && txtSavePath.Text.Trim() != ""&&lblPicNum.Text!="0")
            {
                pictureBox1.Enabled = false;
                pictureBox2.Enabled = false;
                rbPercent.Enabled = false;
                td = new Thread(new ThreadStart(this.CompleteIMG));
                td.Start();
            }
            else
            {
                if (txtPicPath.Text.Trim() == "" && txtSavePath.Text.Trim() == "")
                {
                    MessageBox.Show("警告：请选择待处理的图片目录及保存位置", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (txtPicPath.Text.Trim() == "")
                        MessageBox.Show("警告：请选择待处理的图片", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (txtSavePath.Text.Trim() == "")
                        MessageBox.Show("警告：请选择保存路径", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (td != null)
            {
                td.Abort();
            }
        }
    }
}
