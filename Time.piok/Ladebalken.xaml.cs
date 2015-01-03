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
    /// Interaktionslogik für Ladebalken.xaml
    /// </summary>
    public partial class Ladebalken : Window
    {
        public Ladebalken()
        {
            InitializeComponent();
            
        }
        public void Prog(int wert, int max)
        {
           /**/ int tmp = 100/max;
            prgbar.Value++;// = wert * tmp;
            
        }
    }
}
