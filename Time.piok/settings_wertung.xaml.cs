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
    /// Interaktionslogik für settings_wertung.xaml
    /// </summary>
    public partial class settings_wertung : Window
    {
        Bewerbe bewerb;
        public settings_wertung(Bewerbe bewerb)
        {
            InitializeComponent();
            this.bewerb = bewerb;
            if (bewerb.Anzlauf == 2)
                txt_bibo.IsEnabled = true;
        }

        private void btn_ok_Click(object sender, RoutedEventArgs e)
        {
            bewerb.Mannschaftwertung = cb_mannschaft.IsChecked.Value;
            if (txt_anzlaeuf.Text != "")
                bewerb.Anzahl_Mannschaft = int.Parse(txt_anzlaeuf.Text);
            else
                MessageBox.Show("Hacken Mannschaftswertung entfernen, ansonsten Textbox ausfüllen");
        }
    }
}
