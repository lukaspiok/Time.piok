using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Time.piok
{

    public class Teilnehmer : IEquatable<Teilnehmer>, INotifyPropertyChanged
    {
        string vname;
        string nname;
        int startnummer;
        string klasse;
        string geschlecht;
        DateTime startzeit;
        DateTime zielzeit;
        int rang;
        int geburtsjahr;
        TimeSpan endzeit;
        TimeSpan abstand;
        string loaded;
        string status;
        public string Vorname
        {
            set
            {
                vname = value;
            }
            get
            {
                return vname;
            }
        }
        public int Geburtsjahr
        {
            set
            {
                geburtsjahr = value;
            }
            get
            {
                return geburtsjahr;
            }
        }
        public string Nachname
        {
            set
            {
                nname = value;
            }
            get
            {
                return nname;
            }

        }
        public TimeSpan Abstand
        {
            set
            {
                abstand = value;
            }
            get
            {
                return abstand;
            }
        }
        public int Startnummer
        {
            set
            {
                startnummer = value;
            }
            get
            {
                return startnummer;
            }
        }
        public string Klasse
        {
            set
            {
                klasse = value;
                OnPropertyChanged("Klasse");
            }
            get
            {
                return klasse;
            }
        }

        [XmlIgnore]
        public TimeSpan Endzeit
        {
            set
            {
                endzeit = value;
                OnPropertyChanged("Endzeit");
            }
            get
            {
                return endzeit;
            }
        }
        [XmlAttribute(AttributeName = "Endzeit", DataType = "duration")]
        public string XmlEndzeit
        {
            get { return XmlConvert.ToString(endzeit); }
            set { endzeit = XmlConvert.ToTimeSpan(value); }
        }
        public string Geschlecht
        {
            set { geschlecht = value; }
            get { return geschlecht; }
        }
        public string Loaded
        {
            set { loaded = value; }
            get { return loaded; }
        }
        public string Status
        {
            set
            {
                status = value;
                OnPropertyChanged("Status");
            }
            get { return status; }
        }
        public DateTime Startzeit
        {
            set { startzeit = value; }
            get { return startzeit; }
        }
        public DateTime Zielzeit
        {
            set { zielzeit = value; }
            get { return zielzeit; }
        }
        public int Rang
        {
            set
            {
                rang = value;
                OnPropertyChanged("Rang");
            }
            get { return rang; }
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

        public bool Equals(Teilnehmer other)
        {
            return Startnummer.Equals(other.Startnummer);
        }
        
    }
}
