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
        int anz_mannschaft;
        bool mannschaft;
        string timeformat;
        int bibo;
        public string TimeFormat
        {
            set { timeformat = value; }
            get { return timeformat; }
        }
        public int Bibo
        {
            set{bibo = value;}
            get { return bibo; }
        }
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
        public bool Mannschaftwertung
    {
            set
        {
            mannschaft = value;
        }
            get
        {
            return mannschaft;
        }
    }
        public int Anzahl_Mannschaft
    {
            set
        {
            anz_mannschaft = value;
        }
            get
        {
            return anz_mannschaft;
        }
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
