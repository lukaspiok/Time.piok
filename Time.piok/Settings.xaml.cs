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
            }
            cb_type.SelectedIndex = 0;
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
    }
}
