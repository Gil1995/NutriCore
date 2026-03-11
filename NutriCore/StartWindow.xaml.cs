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

        //Falls Startfenster angeclickt wird, verschwindet es vor dem TimerEnde
        private void TitelImage_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            timer.Stop();

            ShowMainWindow();
        }

        //Hilfsmethode für einen Timer -> Startfenster verschwindet automatisch nach 15 Sekunden
        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();

            ShowMainWindow();
            
        }

        //Hilfsmethode zum Übergang von Startfenster -> Hauptfenster
        private void ShowMainWindow()
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }

        //Fensterleiste Design
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
