using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaktionslogik für Add_competition.xaml
    /// </summary>
    public partial class Add_competition : Window
    {
        Bewerbe bewerbe;
        public Add_competition(Bewerbe bewerbe)
        {
            InitializeComponent();
            this.bewerbe = bewerbe;
        }

        private void btn_add_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(@"C:\time.piok\" + txt_bez.Text))
            {
                Directory.CreateDirectory(@"C:\time.piok\" + txt_bez.Text);
                bewerbe.Name = txt_bez.Text;
                if (rb_1.IsChecked == true)
                    bewerbe.Anzlauf = 1;
                else
                    bewerbe.Anzlauf = 2;
                bewerbe.Datum = DateTime.Today.Day.ToString() + "." + DateTime.Today.Month.ToString() + "." + DateTime.Today.Year.ToString();
                this.DialogResult = true;
            }
            else
                MessageBox.Show("Bewerb existiert schon");
        }
    }
}
