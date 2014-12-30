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
    /// <summary>
    /// Interaktionslogik für Kategoriehin.xaml
    /// </summary>
    public partial class Kategoriehin : Window
    {
        Kategorien Kategorien;
        ObservableCollection<Kategorien> listek = new ObservableCollection<Kategorien>();
        XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<Kategorien>));
        public Kategoriehin(Kategorien Kategorien)
        {
            InitializeComponent();
           this.Kategorien = Kategorien;
            if (File.Exists(@"C:\Time.piok\" + Kategorien.Loaded +"\\categories.xml"))
            {
                if (new FileInfo(@"C:\Time.piok\" + Kategorien.Loaded +"\\categories.xml").Length != 0)
                {
                    XmlTextReader read = new XmlTextReader(@"C:\Time.piok\" + Kategorien.Loaded +"\\categories.xml");
                    listek = xs.Deserialize(read) as ObservableCollection<Kategorien>;
                    read.Close();
                }
            }
            else
            {
                if (!Directory.Exists(@"C:\Time.piok"))
                    Directory.CreateDirectory(@"C:\Time.piok");
                File.Create(@"C:\Time.piok\" + Kategorien.Loaded +"\\categories.xml");
            }
            listview.ItemsSource = listek;
        }

        private void btn_remove_cat_Click(object sender, RoutedEventArgs e)
        {
            listek.Remove(listview.SelectedItem as Kategorien);
            using (StreamWriter wr = new StreamWriter(@"C:\Time.piok\" + Kategorien.Loaded +"\\categories.xml"))
            {
                xs.Serialize(wr, listek);
                wr.Close();
            }
        }

        private void btn_add_cat_Click(object sender, RoutedEventArgs e)
        {
            Kategorien k = new Kategorien();
            Add_Kategorie k2 = new Add_Kategorie(k);
            bool? result1 = k2.ShowDialog();
            if (result1 == true)
            {
                listek.Add(k);
                using (StreamWriter wr = new StreamWriter(@"C:\Time.piok\" + Kategorien.Loaded +"\\categories.xml"))
                {
                    xs.Serialize(wr, listek);
                    wr.Close();
                }
            }
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}