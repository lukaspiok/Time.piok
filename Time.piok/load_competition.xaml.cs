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
    /// Interaktionslogik für load_competition.xaml
    /// </summary>
    public partial class load_competition : Window
    {
        Bewerbe bewerb;
        ObservableCollection<Bewerbe> liste = new ObservableCollection<Bewerbe>();
        XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<Bewerbe>));
        public load_competition(Bewerbe bewerb)
        {
            InitializeComponent();
            this.bewerb = bewerb;
            if (File.Exists(@"C:\Time.piok\competitions.xml"))
            {
                if (new FileInfo(@"C:\Time.piok\competitions.xml").Length != 0)
                {
                    XmlTextReader read = new XmlTextReader(@"C:\Time.piok\competitions.xml");
                    liste = xs.Deserialize(read) as ObservableCollection<Bewerbe>;
                    read.Close();
                }
            }
            else
            {
                if (!Directory.Exists(@"C:\Time.piok"))
                    Directory.CreateDirectory(@"C:\Time.piok");
                File.Create(@"C:\Time.piok\competitions.xml");
            }
            listview.ItemsSource = liste;
        }

        private void btn_add_competition_Click(object sender, RoutedEventArgs e)
        {
            Bewerbe be = new Bewerbe();
            Add_competition k2 = new Add_competition(be);
            bool? result1 = k2.ShowDialog();
            if (result1 == true)
            {
                liste.Add(be);
                bewerb.Name = be.Name;
                bewerb.Anzlauf = be.Anzlauf;
                using (StreamWriter wr = new StreamWriter(@"C:\Time.piok\competitions.xml"))
                {
                    xs.Serialize(wr, liste);
                    wr.Close();
                }
               
            }
        }

        private void btn_load_competition_Click(object sender, RoutedEventArgs e)
        {
           
             if (listview.SelectedIndex == -1)
                MessageBox.Show("Kein Bewerb ausgewählt!");
            else
            {
                bewerb.Anzlauf = liste[listview.SelectedIndex].Anzlauf;
                bewerb.Name = liste[listview.SelectedIndex].Name;
                this.DialogResult = true;
            }
        }

        private void btn_remove_cat_Click(object sender, RoutedEventArgs e)
        {
            Directory.Delete(@"C:\Time.piok\" + liste[listview.SelectedIndex].Name,true);
            liste.Remove(listview.SelectedItem as Bewerbe);
            using (StreamWriter wr = new StreamWriter(@"C:\Time.piok\competitions.xml"))
            {
                xs.Serialize(wr, liste);
                wr.Close();
            }

        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
