using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NutriCore
{
    /// <summary>
    /// Interaction logic for StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        DispatcherTimer timer;
        public StartWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(15);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void TitelImage_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            timer.Stop();

            ShowMainWindow();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();

            ShowMainWindow();
            
        }

        private void ShowMainWindow()
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }

        [DllImport("dwmapi.dll")]
        static extern int DwmSetWindowAttribute(
        IntPtr hwnd,
        int dwAttribute,
        ref int pvAttribute,
        int cbAttribute);

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;

            // 35 = DWMWA_CAPTION_COLOR
            int DWMWA_CAPTION_COLOR = 35;

            // Farbe im BGR Format (nicht RGB!)
            int color = 0x00302D2D;

            DwmSetWindowAttribute(hwnd, DWMWA_CAPTION_COLOR, ref color, sizeof(int));
        }   
    }
}
