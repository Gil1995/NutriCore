using MySql.Data.MySqlClient;
using NutriCore.src;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace NutriCore
{
    /// <summary>
    /// Interaction logic for EditFoodWindow.xaml
    /// </summary>
    public partial class EditFoodWindow : Window
    {

        private TrackedData _toEdit;
        public TrackedData ToEdit { get => _toEdit; set => _toEdit = value; }
        public EditFoodWindow(TrackedData _toEdit)
        {
            InitializeComponent();
            this.ToEdit = _toEdit;
            this.DataContext = this;
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

        //Löschen Button ClickBefehl
        private void DeleteFood_Click(object sender, RoutedEventArgs e)
        {
            int id = ToEdit.TableID;

            string sql = "DELETE FROM tracked_data " +
                         "WHERE id = @id";

            using (MySqlConnection connection = new MySqlConnection(DBConnection.ServerConnection))
            {
                MySqlCommand cmd = new MySqlCommand(sql, connection);

                cmd.Parameters.AddWithValue("@id", id);

                connection.Open();
                cmd.ExecuteNonQuery();
            }

            this.DialogResult = true;
        }

        //MengeÄndern Button ClickBefehl
        private void ChangeQuant_Click(object sender, RoutedEventArgs e)
        {     
            int id = ToEdit.TableID;
            double quant = ToEdit.QuantityTracked;

            string sql = "UPDATE tracked_data " +
                         "SET menge_in_g = @quant " +
                         "WHERE id = @id";

            using (MySqlConnection connection = new MySqlConnection(DBConnection.ServerConnection))
            {
                MySqlCommand cmd = new MySqlCommand(sql, connection);

                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@quant", quant);

                connection.Open();
                cmd.ExecuteNonQuery();
            }

            this.DialogResult = true;
        }   
    }
}