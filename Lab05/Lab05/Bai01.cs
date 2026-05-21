using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab05
{
    public partial class Bai01 : Form
    {
        public Bai01()
        {
            InitializeComponent();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SmtpClient smtpClient = new SmtpClient("127.0.0.1", 25))
            {
                string mailfrom = textBox1.Text.ToString().Trim();
                string mailto = textBox2.Text.ToString().Trim();
                string password = textBox3.Text.ToString().Trim();

                smtpClient.EnableSsl = false;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(mailfrom, password);

                using (MailMessage message = new MailMessage())
                {
                    MailAddress fromAddress = new MailAddress(mailfrom);

                    message.From = fromAddress;
                    message.Subject = textBox4.Text.ToString().Trim();
                    message.IsBodyHtml = true;
                    message.Body = richTextBox1.Text.ToString();
                    message.To.Add(mailto);

                    try
                    {
                        smtpClient.Send(message);
                        MessageBox.Show("Gửi thư thành công.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
        }
    }
}
