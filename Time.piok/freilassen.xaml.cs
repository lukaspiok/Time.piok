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

namespace Time.piok
{
    /// <summary>
    /// Interaktionslogik für freilassen.xaml
    /// </summary>
    public partial class freilassen : Window
    {
        int frei;
        public freilassen(int frei)
        {
            InitializeComponent();
            this.frei = frei;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (txt_anz.Text != "")
                frei = int.Parse(txt_anz.Text);
            else
                frei = 0;
            this.DialogResult = true;
        }

        private void cb_frei_Checked(object sender, RoutedEventArgs e)
        {
            txt_anz.IsEnabled = true;
        }

        private void cb_frei_Unchecked(object sender, RoutedEventArgs e)
        {
            txt_anz.IsEnabled = false;
        }
    }
}
