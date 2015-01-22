using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO.Ports;


namespace Time.piok
{
    
    public partial class Settings : Window
    {
        Device device;
        public Settings(Device device)
        {
            InitializeComponent();
            this.device = device;
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                cb_com.Items.Add(port);
                cb_com_board.Items.Add(port);
            }
            cb_type.SelectedIndex = 0;
            cb_type_board.SelectedIndex = 1;
            if (cb_com.Items.Count == 1)
            {
                rb_com.IsEnabled = false;
                cb_com.IsEnabled = false;
                cb_baud.IsEnabled = false;
                
            }
            else
            {
                cb_com.SelectedIndex = 1;
                cb_baud.SelectedIndex = 3;
                cb_com.Items.Remove(cb_com.SelectedItem = 0);
            }
            if (cb_com_board.Items.Count > 0)
                cb_board.IsEnabled = true;
        }

        private void btn_set_Click(object sender, RoutedEventArgs e)
        {
            device.Ethernet = rb_ethernet.IsChecked.Value;
            device.Com = rb_com.IsChecked.Value;
            device.IP = txt_IP.Text;
            ComboBoxItem typeItem = (ComboBoxItem)cb_type.SelectedItem;
            device.Type = typeItem.Content.ToString();
           if(device.Com==true)
           { 
            ComboBoxItem Item = (ComboBoxItem)cb_baud.SelectedItem;
            device.ComBaud = int.Parse(Item.Content.ToString());
            device.ComPort = cb_com.SelectedItem.ToString();  
            }
            if(cb_board.IsChecked.HasValue && cb_board.IsChecked.Value)
            {
                device.Board = true;
                ComboBoxItem boardtype = (ComboBoxItem)cb_type_board.SelectedItem;
                device.BoardType = boardtype.Content.ToString();
                device.ComBoard = cb_com_board.SelectedItem.ToString();
                ComboBoxItem boardbaud = (ComboBoxItem)cb_baud_board.SelectedItem;
                device.BaudBoard = int.Parse(boardbaud.Content.ToString());
            }
            this.DialogResult = true;
        }

        private void cb_com_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
                    if(cb_type.SelectedIndex != 0)
                    {
                     rb_ethernet.IsEnabled = false;
                     txt_IP.IsEnabled = false;
                    }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            cb_baud_board.IsEnabled = true;
            cb_type_board.IsEnabled = true;
            cb_com_board.IsEnabled = true;
            cb_baud_board.SelectedIndex = 1;
            cb_com_board.SelectedIndex = 0;
            cb_type_board.SelectedIndex = 0;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            cb_baud_board.IsEnabled = false;
            cb_type_board.IsEnabled = false;
            cb_com_board.IsEnabled = false;
        }
    }
}
