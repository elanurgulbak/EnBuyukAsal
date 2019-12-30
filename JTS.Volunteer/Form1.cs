using JTS.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JTS.Volunteer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Minimize();
            
        }
        private string GetMACAddress()
        {
            return ConfigurationManager.AppSettings["MAC"] ?? NetworkInterface.GetAllNetworkInterfaces().Where(c => c.OperationalStatus == OperationalStatus.Up).Select(c => c.GetPhysicalAddress().ToString()).FirstOrDefault();
        }
        /// <summary>
         /// (2^p - 1) değerinin asal sayı olup olmadığına bakılıyor.
         /// </summary>
        public bool IsPrimeNumber(int p)
        {
            if (p < 2)
            {
                throw new ArgumentException(nameof(p));
            }

            // Eğer p değeri çift ise, sadece 2 değeri için Mersenne asalı olur.
            if (p % 2 == 0)
            {
                return p == 2;
            }

            // (2^p - 1) sayısının asal olması için, p değerinin de asal olması gerekiyor.
            // Tüm tek sayılara göre mod alma işlemi uygulanarak, p sayısının asal olup olmadığına bakılıyor.
            // Döngünün p^(1/2) ye kadar çalışması yeterli. Hiçbir sayı, karekök değerinden büyük bir sayıya tam olarak bölünemez.
            for (int i = 3; i <= (int)Math.Sqrt(p); i += 2)
            {
                if (p % i == 0)
                {
                    // Tam olarak bölünüyor, asal değil.
                    return false;
                }
            }

            // Değer çok büyük olabileceği için BigInteger kullanılmıştır.
            var mode = BigInteger.Pow(2, p) - 1;

            // S fonksiyonu başlangıç değeri
            var S = new BigInteger(4);

            // S(p-2) değeri bulunacak
            for (int n = 1; n <= p - 2; n++)
            {
                // S(n) = (S(n-1)^2 - 2) mod (2p-1)
                S = (BigInteger.Pow(S, 2) - 2) % mode;
            }

            // S(p-2) değeri 0 ise, (2^p - 1) değeri asaldır.
            return S == 0;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                var address = new EndpointAddress("http://localhost:62210/JobService.svc");
                var binding = new BasicHttpBinding();
                var channel = ChannelFactory<IJobService>.CreateChannel(binding, address);
                var mac = this.GetMACAddress();
                var n = channel.GetNewJob(mac);

                this.label1.Text = "GetNewJob " + n;

                var isPrime = this.IsPrimeNumber((int)n);

                this.label1.Text = n + (isPrime ? " is prime" : " is not prime");

                notifyIcon1.Text = this.label1.Text;

                channel.SaveJobResult(mac, n, isPrime);
            }
            catch
            {

            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
            {
                Minimize();
            }
        }
        private void Minimize()
        {
            Hide();
            notifyIcon1.Visible = true;
            notifyIcon1.Text = "NotifyIcon Denemesi";
            notifyIcon1.BalloonTipTitle = "Program Çalışıyor";
            notifyIcon1.BalloonTipText = "Program sağ alt köşede konumlandı.";
            notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon1.ShowBalloonTip(10000);
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            Show();
            notifyIcon1.Visible = false;
            this.WindowState = FormWindowState.Normal;
        }
    }
}
