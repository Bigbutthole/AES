using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace 凯撒密码CaesarDecrypt
{
    public partial class Form1 : Form
    {
        // 声明 Windows API 函数
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        // 声明 EM_GETFIRSTVISIBLELINE 消息常量
        private const int EM_GETFIRSTVISIBLELINE = 0x00CE;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //加密文本
            //textBox2.Text = EncryptStr(textBox1.Text);

            //加密或解密文本
            textBox2.Text = "加密文本:" + EncryptStr(textBox1.Text)+ "解密文本:" + DecryptStr(textBox1.Text);

        }


        //设置AES加密解密参数
        private RijndaelManaged Setting()
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes("codecodekedocode"), //加密密钥,自己设置，长度必须为16字节的倍数
                IV = Encoding.UTF8.GetBytes("1234567812345678"),  //加密的iv偏移量,长度必须为16字节的倍数
                Mode = CipherMode.CBC,       //加密模式，ECB、CBC、CFB等
                Padding = PaddingMode.PKCS7, //待加密的明文长度不满足条件时使用的填充模式，PKCS7是python中默认的填充模式
                BlockSize = 128              //加密操作的块大小
            };
            return rijndaelCipher;
        }

        //加密字符串，并保存为base64编码格式的字符串
        public string EncryptStr(string encryptStr)
        {
            string decryptStr = string.Empty;
            try
            {
                //将待加密的明文字符串转为加密所需的字节数组格式
                byte[] plainText = Encoding.UTF8.GetBytes(encryptStr);

                //设定加密参数
                RijndaelManaged rijndaelCipher = Setting();

                //加密字符串
                ICryptoTransform transform = rijndaelCipher.CreateEncryptor();
                byte[] cipherBytes = transform.TransformFinalBlock(plainText, 0, plainText.Length);

                //将加密后的字节数组转为base64格式字符串
                decryptStr = Convert.ToBase64String(cipherBytes);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
            return decryptStr;
        }


        //解密base64编码格式的字符串
        public string DecryptStr(string decryptStr)
        {
            string encryptStr = string.Empty;
            try
            {
                //将待解密的base64格式字符串解码为加密所需的字节数组格式
                byte[] plainText = Convert.FromBase64String(decryptStr);

                //设定解密参数
                RijndaelManaged rijndaelCipher = Setting();

                //解密字符串
                ICryptoTransform transform = rijndaelCipher.CreateDecryptor();
                byte[] cipherBytes = transform.TransformFinalBlock(plainText, 0, plainText.Length);

                //将解密后的字节数组转为字符串
                encryptStr = Encoding.UTF8.GetString(cipherBytes);
            }
            catch (Exception ex)
            {
                //MessageBox.Show();
            }
            return encryptStr;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.Opacity >= 0.025)
            {
                this.Opacity -= 0.025;
            }
            else
            {
                timer1.Stop();
                this.Close();
            }
        }

        private void userControl11_Scroll(object sender, ScrollEventArgs e)
        {
            // 根据滚动事件的值计算 TextBox 的滚动位置
            int scrollPos = e.NewValue;

            // 更新 TextBox 的滚动位置
            textBox1.SelectionStart = scrollPos;
            textBox1.ScrollToCaret();

            // 更新 ScrollBar 的值
            //userControl11.Value = scrollPos;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.textBox1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.Panel_MouseWheel);
            this.textBox2.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.Panel2_MouseWheel);
        }

        private void Panel_MouseWheel(object sender, MouseEventArgs e)
        {
            // 如果当前有选中文本，则不滚动
            if (textBox1.SelectionLength > 0)
            {
                return;
            }

            // 计算滚动的行数
            int numberOfTextLinesToMove = e.Delta * SystemInformation.MouseWheelScrollLines / 120;

            // 获取当前行的第一个字符的索引
            int index = textBox1.GetFirstCharIndexOfCurrentLine();

            // 如果滚动的行数大于 0，则向下滚动，否则向上滚动
            if (numberOfTextLinesToMove > 0)
            {
                int lineCount = textBox1.GetLineFromCharIndex(textBox1.Text.Length) - textBox1.GetLineFromCharIndex(index);
                if (lineCount <= numberOfTextLinesToMove)
                {
                    index = textBox1.GetFirstCharIndexFromLine(textBox1.GetLineFromCharIndex(index) + lineCount);
                }
                else
                {
                    index = textBox1.GetFirstCharIndexFromLine(textBox1.GetLineFromCharIndex(index) + numberOfTextLinesToMove);
                }
            }
            else if (numberOfTextLinesToMove < 0)
            {
                int lineCount = textBox1.GetLineFromCharIndex(index);
                if (lineCount <= -numberOfTextLinesToMove)
                {
                    index = 0;
                }
                else
                {
                    index = textBox1.GetFirstCharIndexFromLine(textBox1.GetLineFromCharIndex(index) + numberOfTextLinesToMove);
                }
            }

            // 移动光标并滚动文本框
            textBox1.Select(index, 0);
            textBox1.ScrollToCaret();
        }

        private void Panel2_MouseWheel(object sender, MouseEventArgs e)
        {
            // 如果当前有选中文本，则不滚动
            if (textBox2.SelectionLength > 0)
            {
                return;
            }

            // 计算滚动的行数
            int numberOfTextLinesToMove = e.Delta * SystemInformation.MouseWheelScrollLines / 120;

            // 获取当前行的第一个字符的索引
            int index = textBox2.GetFirstCharIndexOfCurrentLine();

            // 如果滚动的行数大于 0，则向下滚动，否则向上滚动
            if (numberOfTextLinesToMove > 0)
            {
                int lineCount = textBox2.GetLineFromCharIndex(textBox2.Text.Length) - textBox2.GetLineFromCharIndex(index);
                if (lineCount <= numberOfTextLinesToMove)
                {
                    index = textBox2.GetFirstCharIndexFromLine(textBox2.GetLineFromCharIndex(index) + lineCount);
                }
                else
                {
                    index = textBox2.GetFirstCharIndexFromLine(textBox2.GetLineFromCharIndex(index) + numberOfTextLinesToMove);
                }
            }
            else if (numberOfTextLinesToMove < 0)
            {
                int lineCount = textBox2.GetLineFromCharIndex(index);
                if (lineCount <= -numberOfTextLinesToMove)
                {
                    index = 0;
                }
                else
                {
                    index = textBox2.GetFirstCharIndexFromLine(textBox2.GetLineFromCharIndex(index) + numberOfTextLinesToMove);
                }
            }

            // 移动光标并滚动文本框
            textBox2.Select(index, 0);
            textBox2.ScrollToCaret();
        }
    }
}
