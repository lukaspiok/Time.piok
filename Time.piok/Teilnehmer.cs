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
        int idKlasse;
        string geschlecht;
        DateTime startzeit;
        DateTime zielzeit;
        int rang;
        string mannschaft;
        int geburtsjahr;
        TimeSpan endzeit;
        TimeSpan abstand;
        string loaded;
        string status;
        int randStartnummer;
        string status2;
        TimeSpan lauf1;
        TimeSpan lauf2;

        public string Status2
    {
        set { status2 = value; }
        get { return status2; }
    }
        [XmlIgnore]
        public TimeSpan Lauf1
        {
            set { lauf1 = value;
            OnPropertyChanged("Lauf1");
            }
            get { return lauf1; }
        }
        [XmlIgnore]
        public TimeSpan Lauf2
        {
            set { lauf2 = value;
            OnPropertyChanged("Lauf2");
            }
            get { return lauf2; }
        }
        [XmlAttribute(AttributeName = "Lauf1", DataType = "duration")]
        public string XmlLauf1
        {
            get { return XmlConvert.ToString(lauf1); }
            set { lauf1 = XmlConvert.ToTimeSpan(value); }
        }
        [XmlAttribute(AttributeName = "Lauf2", DataType = "duration")]
        public string XmlEndzeit
        {
            get { return XmlConvert.ToString(lauf2); }
            set { lauf2 = XmlConvert.ToTimeSpan(value); }
        }
        public string Mannschaft
        { set { mannschaft = value; }
          get { return mannschaft; }
        }
        [XmlIgnore]
        public int RandomStartnummer
        {
            set { randStartnummer = value; }
            get { return randStartnummer; }
        }
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
        public string XmlLauf2
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
        public int ID
        {
            set { idKlasse = value; }
            get { return idKlasse; }
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
