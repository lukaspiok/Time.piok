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
    public partial class DSQ : Window
    {
        Teilnehmer teilnehmer;
        public DSQ(Teilnehmer teilnehmer)
        {
            InitializeComponent();
            this.teilnehmer = teilnehmer;
        }
        private void btn_dsq_Click(object sender, RoutedEventArgs e)
        {
            if(txt_stnr.Text!="")
            {
                teilnehmer.Startnummer = int.Parse(txt_stnr.Text);
                this.DialogResult = true;
            }
          
        }
    }
}
