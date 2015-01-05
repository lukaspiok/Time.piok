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
    /// Interaktionslogik für Add_Kategorie.xaml
    /// </summary>
    public partial class Add_Kategorie : Window
    {
        Kategorien kategorien;
        int countID;
        public Add_Kategorie(Kategorien kategorien, int count)
        {
            InitializeComponent();
            this.kategorien = kategorien;
            cb.SelectedIndex = 0;
            this.countID = count;
        }

        private void btn_add_Click(object sender, RoutedEventArgs e)
        {
            if (txt_bis.Text != "" && txt_von.Text != "" && kname_txt.Text != "")
            {
                kategorien.Anfangsjahr = int.Parse(txt_von.Text);
                kategorien.Endjahr = int.Parse(txt_bis.Text);
                ComboBoxItem typeItem = (ComboBoxItem)cb.SelectedItem;
                kategorien.Geschlecht = typeItem.Content.ToString();
                kategorien.ID = Convert.ToInt16(txt_ID.Text);
                kategorien.Name = kname_txt.Text;
                this.DialogResult = true;
            }
            else
                MessageBox.Show("Alle Felder müssen ausgefüllt sein!");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txt_ID.Text = countID.ToString();
        }
    }
}
