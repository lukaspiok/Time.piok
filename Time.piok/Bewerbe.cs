using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Time.piok
{
    public class Bewerbe
    {
        int anz_laufe;
        string date;
        string bez;
        public int Anzlauf
        {
            set { anz_laufe = value; }
            get { return anz_laufe; }
        }
    public string Datum
        {
            set { date = value; }
            get { return date; }
        }
    public string Name
    {
        set { bez = value; }
        get { return bez; }
    }
    
    public event PropertyChangedEventHandler PropertyChanged;


    protected void OnPropertyChanged(string name)
    {
        PropertyChangedEventHandler handler = PropertyChanged;
        if (handler != null)
        {
            handler(this, new PropertyChangedEventArgs(name));
        }
    }
    }
}
