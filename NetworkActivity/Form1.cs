using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Management;

namespace NetworkActivity
{
    public partial class Form1 : Form
    {
        NotifyIcon packetsRecivedIcon;
        Thread worker;

        public Form1()
        {
            InitializeComponent();

            packetsRecivedIcon = new NotifyIcon();
            packetsRecivedIcon.Visible = true;

            MenuItem quitMenuItem = new MenuItem("Quit");
            MenuItem programeNameMenuItem = new MenuItem("Network Activity v0.1");
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add(programeNameMenuItem);
            contextMenu.MenuItems.Add(quitMenuItem);
            packetsRecivedIcon.ContextMenu = contextMenu;

            quitMenuItem.Click += QuitMenuItem_Click;

            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;

            worker = new Thread(new ThreadStart(HardDriveActivityThread));
            worker.Start();
        }

        private void QuitMenuItem_Click(object sender, System.EventArgs e)
        {
            worker.Abort();
            packetsRecivedIcon.Dispose();
            this.Close();
        }

        private void HardDriveActivityThread()
        {
            var driveDataClass = new ManagementClass("Win32_PerfFormattedData_PerfNet_Redirector");
            try
            {
                while (true)
                {
                    var driveDataClassCollection = driveDataClass.GetInstances();
                    foreach (var item in driveDataClassCollection)
                    {
                        string value = Convert.ToUInt64(item["PacketsReceivedPersec"]).ToString();
                        CreateTextIcon(value);
                    }
                    Thread.Sleep(100);
                }
            }
            catch (ThreadAbortException)
            {
                driveDataClass.Dispose();
               // MessageBox.Show(ex.Message);
            }
        }

        private void CreateTextIcon(string str)
        {
            Font fontToUse = new Font("Microsoft Sans Serif", 16, FontStyle.Regular, GraphicsUnit.Pixel);
            Brush brushToUse = new SolidBrush(Color.White);
            Bitmap bitmapText = new Bitmap(16, 16);
            Graphics g = System.Drawing.Graphics.FromImage(bitmapText);

            IntPtr hIcon;

            g.Clear(Color.Transparent);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            g.DrawString(str, fontToUse, brushToUse, -4, -2);
            hIcon = (bitmapText.GetHicon());
            packetsRecivedIcon.Icon = System.Drawing.Icon.FromHandle(hIcon);
        }
    }
}
