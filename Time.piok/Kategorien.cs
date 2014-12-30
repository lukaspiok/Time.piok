using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Time.piok
{
    public class Kategorien
    {
        string name;
        int jahrganganfang;
        int jahrgangende;
        string geschlecht;
        string loaded;
public string Name
        {
    set
            { name = value; }
            get { return name; }
        }
    public int Anfangsjahr
{
    set { jahrganganfang=value;}
    get { return jahrganganfang; }
}
        public int Endjahr
    {
        set { jahrgangende = value; }
            get {return jahrgangende;}
    }
        public string Geschlecht
        {
            set { geschlecht = value;}
            get {return geschlecht;}
        }
        public string Loaded
        {
            set { loaded = value; }
            get { return loaded; }
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
