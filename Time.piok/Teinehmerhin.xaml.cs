using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Xml;
using System.Xml.Serialization;

namespace Time.piok
{

    public partial class Teinehmerhin : Window
    {
        Teilnehmer Teilnehmer;
        ObservableCollection<Kategorien> listek = new ObservableCollection<Kategorien>();
        XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<Kategorien>));
        public Teinehmerhin(Teilnehmer Teilnehmer)
        {
            InitializeComponent();
            this.Teilnehmer = Teilnehmer;
            cb_ges.SelectedIndex = 0;
           if (File.Exists(@"C:\Time.piok\" + Teilnehmer.Loaded +"\\categories.xml"))
            {
                if (new FileInfo(@"C:\Time.piok\" + Teilnehmer.Loaded +"\\categories.xml").Length != 0)
                {
                    XmlTextReader read = new XmlTextReader(@"C:\Time.piok\" + Teilnehmer.Loaded +"\\categories.xml");
                    listek = xs.Deserialize(read) as ObservableCollection<Kategorien>;
                    read.Close();
                }
            }
            for(int i=0;i<listek.Count;i++)
            {
                cb_cat.Items.Add(listek[i].Name);
            }
            cb_cat.SelectedIndex = 0;
        }

        private void btn_add_Click(object sender, RoutedEventArgs e)
        {
          
            Teilnehmer.Nachname = name_txt.Text;
            Teilnehmer.Vorname = vname_txt.Text;
            Teilnehmer.Startnummer = int.Parse(stnr_txt.Text);
            Teilnehmer.Geburtsjahr = int.Parse(txt_jahr.Text);
            ComboBoxItem typeItem = (ComboBoxItem)cb_ges.SelectedItem;
            Teilnehmer.Geschlecht = typeItem.Content.ToString();
            Teilnehmer.Klasse = cb_cat.SelectedItem.ToString(); ;
            Teilnehmer.Status = "DNS";
            this.DialogResult = true;
        }
    }
}
