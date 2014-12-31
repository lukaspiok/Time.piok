using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Data.OleDb;
using System.Data;
using System.Globalization;

namespace Time.piok
{
    /*************     Time.piok   ****************************************
     * ***********Programmiert von Lukas Piok******************************
     * ***********      Version 1.0             ***************************
     */
    public partial class MainWindow : Window
    {
        DispatcherTimer timer;
        ObservableCollection<Teilnehmer> liste = new ObservableCollection<Teilnehmer>();
        ObservableCollection<Kategorien> listek = new ObservableCollection<Kategorien>();
        ICollectionView ansicht;
        XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<Teilnehmer>));
        XmlSerializer xsk = new XmlSerializer(typeof(ObservableCollection<Kategorien>));
        Socket sock;
        Device dev = new Device();
        Teilnehmer tt = new Teilnehmer();
        bool keepalive = true;
        Bewerbe bewerb = new Bewerbe();
        SerialPort mySerialPort;
        string state;
        public MainWindow()
        {
            InitializeComponent();
            btn_starttiming.Header = "Zeitnehmung starten";
            state = btn_starttiming.Header.ToString();
            Bewerbe b = new Bewerbe(); 
           load_competition k2 = new load_competition(b);
           bool? result1 = k2.ShowDialog();
           if (result1 == true)
           {
               MessageBox.Show(b.Name + " wurde erfolgreich geladen");
               bewerb.Name = b.Name;

           }
           else
           {
               MessageBox.Show("Kein Bewerb gewählt, Programm wird geschlossen!");
               this.Close();
           }
            if (File.Exists(@"C:\Time.piok\" + b.Name + "\\competitors.xml"))
            {
                if (new FileInfo(@"C:\Time.piok\" + b.Name + "\\competitors.xml").Length != 0)
                {
                    XmlTextReader read = new XmlTextReader(@"C:\Time.piok\" + b.Name + "\\competitors.xml");
                    liste = xs.Deserialize(read) as ObservableCollection<Teilnehmer>;
                    read.Close();
                }
            }
            else
            {
                if (!Directory.Exists(@"C:\Time.piok"))
                    Directory.CreateDirectory(@"C:\Time.piok");
                File.Create(@"C:\Time.piok\" + b.Name + "\\competitors.xml");
            }
            if (File.Exists(@"C:\Time.piok\" + b.Name + "\\categories.xml"))
            {
                XmlTextReader read = new XmlTextReader(@"C:\Time.piok\" + b.Name + "\\categories.xml");
                listek = xsk.Deserialize(read) as ObservableCollection<Kategorien>;
                read.Close();
            }            
            listview.ItemsSource = liste;   
                    CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listview.ItemsSource);
                        PropertyGroupDescription groupDescription = new PropertyGroupDescription("Klasse");
                        view.GroupDescriptions.Add(groupDescription);
            ansicht = CollectionViewSource.GetDefaultView(liste);
            SortView();
            Rang_zuweisen();
            if(listek.Count == 0)
            {
                Kategorien kkk = new Kategorien();
                kkk.Anfangsjahr = 0;
                kkk.Endjahr = DateTime.Today.Year;
                kkk.Name = "Standart";
                kkk.Geschlecht = "Männlich und Weiblich";
                listek.Add(kkk);
                using (StreamWriter wr = new StreamWriter(@"C:\Time.piok\" + bewerb.Name + "\\categories.xml"))
                {
                    xsk.Serialize(wr, listek);
                    wr.Close();
                }
            }
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Start();       
        }

        private void Rang_zuweisen()
        {
            foreach (CollectionViewGroup g in ansicht.Groups)
            {
                for (int i = 0; i < g.Items.Count; i++)
                {
                    Teilnehmer t = g.Items[i] as Teilnehmer;
                    if (i + 1 < g.Items.Count)
                    {
                        Teilnehmer t1 = g.Items[i + 1] as Teilnehmer;
                        t.Rang = i + 1;
                        if (t.Endzeit == t1.Endzeit)
                        {
                            t1.Rang = t.Rang;
                            i++;
                        }
                    }
                    else
                        t.Rang = i + 1;
                }
                for(int i = 0;i<liste.Count;i++)
                {
                    if (liste[i].Status != "OK")
                        liste[i].Rang = 0;
                }

            }
        }

        private void btn_add_Click(object sender, RoutedEventArgs e)
        {
            Teilnehmer t = new Teilnehmer();
            t.Loaded = bewerb.Name;
            Teinehmerhin w2 = new Teinehmerhin(t);
            bool? result = w2.ShowDialog();
            tt.Startnummer = t.Startnummer;
            int position = liste.IndexOf(tt);
            if (position == -1)
            {
                if (result == true)
                {
                    liste.Add(t);
                    writelist();

                }
            }
            else
            {
                result = false;
                liste.Remove(t);
                MessageBox.Show("Startnummer bereits in Verwendung");
            }
        }

        private void btn_remove_Click(object sender, RoutedEventArgs e)
        {
            liste.Remove(listview.SelectedItem as Teilnehmer);
            writelist();
        }

        private void writelist()
        {
            using (StreamWriter wr = new StreamWriter(@"C:\Time.piok\" + bewerb.Name + "\\competitors.xml"))
            {
                xs.Serialize(wr, liste);
                wr.Close();
            }
        }

        private void btn_categories_Click(object sender, RoutedEventArgs e)
        {
            Kategorien k = new Kategorien();
            k.Loaded = bewerb.Name;
            Kategoriehin k2 = new Kategoriehin(k);
            bool? result1 = k2.ShowDialog();
            if (result1 == true)
            {
                XmlTextReader read = new XmlTextReader(@"C:\Time.piok\" + bewerb.Name + "\\categories.xml");
                listek = xsk.Deserialize(read) as ObservableCollection<Kategorien>;
                read.Close();
            }
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            if (state != "Zeitnehmung starten")
            {
                if(dev.Com == true)
                    mySerialPort.Close();
                
                else if(dev.Ethernet==true)
                  sock.Close();
            }
            this.Close();
        }

        private void btn_dsq_Click(object sender, RoutedEventArgs e)
        {
            Teilnehmer te = new Teilnehmer();
            DSQ dq = new DSQ(te);
            bool? result1 = dq.ShowDialog();
            if (result1 == true)
            {
                int position = liste.IndexOf(te);
                if (position !=-1)
                {
                    liste[position].Status = "DSQ";
                    liste[position].Startzeit = DateTime.Parse("00:00:00.00000");
                    liste[position].Zielzeit = DateTime.Parse("00:00:00.00000");
                    liste[position].Endzeit = liste[position].Startzeit - liste[position].Zielzeit;
                }
            }
        }

        private void btn_starttiming_Click(object sender, RoutedEventArgs e)
        {
            state = btn_starttiming.Header.ToString();
            if (state == "Zeitnehmung starten")
            {
                if (dev.Ethernet == true)
                {
                    sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    sock.Connect(new IPEndPoint(IPAddress.Parse(dev.IP), 7000));
                    Thread clientService = new Thread(new ThreadStart(read));
                    clientService.Start();
                    btn_starttiming.Header = "Zeitnehmung stoppen";
                    state = "Zeitnehmung stoppen";
                }
                else if (dev.Com == true)
                {
                    btn_starttiming.Header = "Zeitnehmung stoppen";
                    state = "Zeitnehmung stoppen";
                    mySerialPort = new SerialPort(dev.ComPort);
                    mySerialPort.BaudRate = dev.ComBaud;
                    mySerialPort.Parity = Parity.None;
                    mySerialPort.StopBits = StopBits.One;
                    mySerialPort.DataBits = 8;
                    mySerialPort.Handshake = Handshake.None;
                    mySerialPort.Open();
                    mySerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                }
            }
            else
            {
                if (dev.Ethernet == true)
                {
                    keepalive = false;
                    sock.Close();
                    btn_starttiming.Header = "Zeitnehmung starten";
                    state = "Zeitnehmung starten";
                }
                else if (dev.Com == true)
                {
                    mySerialPort.Close();
                    btn_starttiming.Header = "Zeitnehmung starten";
                    state = "Zeitnehmung starten";
                }
            }
        }
        private delegate void readHandler(string s);

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            System.Threading.Thread.Sleep(80);
            string indata = sp.ReadExisting();
            Dispatcher.Invoke(new readHandler(serialread), indata);
           
        }
        private void serialread(string s)
        {
            if (dev.Type == "Alge TdC 8001")
                AlgeProtocol(s);
            
            else if (dev.Type == "Tag Heuer CP545")
                TagHeuerProtocol(s);
            
            else if (dev.Type == "Microgate Rei2")
            {
                int iCount = 0, iLength = 0; ;
                string strCutString = "", strCommand = "";
                iCount = 0;
                strCutString = s;
                do
                {
                    iCount++;
                    iLength = strCutString.Length;

                    if (iLength >= 52)
                    {
                        strCommand = strCutString;
                        if (iLength > 52)
                            strCommand = strCommand.Remove(52, strCommand.Length - 52);

                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Time.piok\" + bewerb.Name + "\\timing_log.txt", true))
                            file.WriteLine(strCommand);

                        MicrogateProtocol(strCommand);
                        strCutString = strCutString.Remove(0, 52);
                    }
                    else
                        break;

                    if (iCount > 50)
                    {
                        MessageBox.Show("More than 50 commands send!", "Error",MessageBoxButton.OK);
                        break;
                    }
                } while (iLength != 52);
            }
        }


        private void TagHeuerProtocol(string s)
        {
            if (s.Contains("TN") || s.Contains("T-") || s.Contains("!N") || s.Contains("T+") || s.Contains("T="))
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Time.piok\" + bewerb.Name + "\\timing_log.txt", true))
                {
                    file.WriteLine(s);
                    string[] Teile = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    Teilnehmer tt = new Teilnehmer();
                    Teilnehmer t0 = new Teilnehmer();
                    if (Teile[0] == "TN")
                    {
                        tt.Startnummer = int.Parse(Teile[1]);
                        int position = liste.IndexOf(tt);
                        if (position != -1)
                        {
                            if (Teile[4].Length != 14)
                            {
                                for (int i = 0; Teile[4].Length < 14; i++)
                                {
                                    if (Teile[4].Length == 5)
                                    {
                                        Teile[4] = "." + Teile[4];
                                    }
                                    if (Teile[4].Length == 8 || Teile[4].Length == 11)
                                    {
                                        Teile[4] = ":" + Teile[4];
                                    }
                                    Teile[4] = "0" + Teile[4];
                                }

                            }
                            if (Teile[3] == "M4" || Teile[3] == "4")
                            {
                                DateTime zielzeit;
                                if (!DateTime.TryParse(Teile[4], out zielzeit))
                                {
                                    int sec;
                                    zielzeit = new DateTime();
                                    if (int.TryParse(Teile[4], out sec))
                                        zielzeit.AddSeconds(sec);
                                }
                                Zielzeit_zuweisen(Teile[4], position);
                            }
                            else if (Teile[3] == "1" || Teile[3] == "M1")
                            {
                                DateTime startzeit;
                                if (!DateTime.TryParse(Teile[4], out startzeit))
                                {
                                    int sec;
                                    startzeit = new DateTime();
                                    if (int.TryParse(Teile[4], out sec))
                                        startzeit.AddSeconds(sec);
                                }

                                Startzeit_zuweisen(Teile[4], position);
                            }
                        }
                    }
                    else if (Teile[0].Contains("T-"))
                    {
                        tt.Startnummer = int.Parse(Teile[1]);
                        int position = liste.IndexOf(tt);
                        if (position != -1)
                        {
                            if (Teile[3] == "1" || Teile[3] == "M1")
                            {
                                ClearStart(position);
                            }
                            else if (Teile[3] == "4" || Teile[3] == "M4")
                            {
                                ClearZiel(position);
                            }
                        }
                    }

                    else if (Teile[0].Contains("T+") || Teile[0].Contains("T="))
                    {
                        tt.Startnummer = int.Parse(Teile[1]);
                        int position = liste.IndexOf(tt);
                        if (position != -1)
                        {
                            if (Teile[4].Length != 14)
                            {
                                for (int i = 0; Teile[4].Length < 14; i++)
                                {
                                    if (Teile[4].Length == 5)
                                    {
                                        Teile[4] = "." + Teile[4];
                                    }
                                    if (Teile[4].Length == 8 || Teile[4].Length == 11)
                                    {
                                        Teile[4] = ":" + Teile[4];
                                    }
                                    Teile[4] = "0" + Teile[4];
                                }

                            }
                            if (Teile[3] == "M4" || Teile[3] == "4")
                            {
                                Zielzeit_zuweisen(Teile[4], position);
                            }
                            else if (Teile[3] == "M1" || Teile[3] == "1")
                            {
                                Startzeit_zuweisen(Teile[4], position);

                            }
                        }
                    }

                }

            }
        }

        private void ClearZiel(int position)
        {
            liste[position].Zielzeit = DateTime.Parse("00:00:00.00000");
            liste[position].Endzeit = TimeSpan.Parse("00:00:00.00000");
            liste[position].Status = "DNF";
        }

        private void ClearStart(int position)
        {
            liste[position].Startzeit = DateTime.Parse("00:00:00.00000");
            liste[position].Zielzeit = DateTime.Parse("00:00:00.00000");
            liste[position].Endzeit = liste[position].Startzeit - liste[position].Zielzeit;
            liste[position].Status = "DNS";
        }

        private void MicrogateProtocol(string strCommand)
        {
            char[] zeichen = strCommand.ToCharArray();
            string mode = zeichen[4].ToString() + zeichen[5].ToString();
            if (mode == "SO")
            {
                string startnummer = zeichen[12].ToString() + zeichen[13].ToString() + zeichen[14].ToString() + zeichen[15].ToString() + zeichen[16].ToString();
                string kanal = zeichen[23].ToString() + zeichen[24].ToString() + zeichen[25].ToString();
                char info = zeichen[29];
                string uhrzeit = zeichen[30].ToString() + zeichen[31].ToString() + ":" + zeichen[32].ToString() + zeichen[33].ToString() + ":" + zeichen[34].ToString() + zeichen[35].ToString() + "." + zeichen[36].ToString() + zeichen[37].ToString() + zeichen[38].ToString() + zeichen[39].ToString();
                tt.Startnummer = int.Parse(startnummer);
                int position = liste.IndexOf(tt);
                if (position != -1)
                {
                    if (info == '0')
                    {
                        if (kanal == "000" || kanal == "100")
                        {
                            Startzeit_zuweisen(uhrzeit, position);
                        }
                        if (kanal == "015" || kanal == "115")
                        {
                            Zielzeit_zuweisen(uhrzeit, position);
                        }
                    }
                    else if (info == 'a')
                    {
                        if (kanal == "015" || kanal == "115")
                        {
                            ClearZiel(position);
                        }
                        else if (kanal == "000" || kanal == "100")
                        {
                            ClearStart(position);
                        }
                    }
                    else if (info == 'Q')
                    {
                        DSQ_COMP(position);
                    }
                    else if (info == 'K')
                    {
                        if(kanal == "315")
                        {
                            Zielzeit_zuweisen(uhrzeit, position);
                        }
                        else if(kanal == "300")
                        {
                            Startzeit_zuweisen(uhrzeit, position);
                        }
                    }
                }
            }
        }

        private void DSQ_COMP(int position)
        {
            liste[position].Status = "DSQ";
            liste[position].Startzeit = DateTime.Parse("00:00:00.00000");
            liste[position].Zielzeit = DateTime.Parse("00:00:00.00000");
            liste[position].Endzeit = TimeSpan.Parse("00:00:00.000000");
        }
        private void Abstand_ber()
        {
            Teilnehmer th = new Teilnehmer();
            th.Rang = 1;
            for (int i = 0; i < listek.Count; i++)
            {
                th.Klasse = listek[i].Name;
                var q = liste.IndexOf(liste.Where(teil => teil.Rang == 1).FirstOrDefault());
                int position = q;
                if(position!=-1)
                {
                    for(int x = 0;x<liste.Count;x++)
                    {
                        if(liste[x].Klasse == th.Klasse)
                        {
                            if(liste[x].Status == "OK")
                                liste[x].Abstand = liste[position].Endzeit - liste[x].Endzeit;
                        }
                    }
                }
            }
        }
        
        private void AlgeProtocol(string s)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Time.piok\" + bewerb.Name + "\\timing_log.txt", true))
                file.WriteLine(s);

            SplitLine(s);
        }

        private bool IsLineValid(string s)
        {
            bool result;
            Regex myPattern = new Regex("[0-9]{4} [A-Z]");

            result = myPattern.IsMatch(s);
            return result;
        }

        private void SplitLine(string s)
        {
            string strLine;
            string strTmp;
            int index;

            strTmp = s;

            index = strTmp.IndexOf("\r");
            if (s.Length < 5 || index <= 0)
                return;

            strLine = strTmp.Remove(index, strTmp.Length - index);

            if (IsLineValid(strLine))
                AlgeProtkoll_auswertung(strLine);

            s = s.Remove(0, s.IndexOf("\r")+1);

            SplitLine(s);
        }

        private void AlgeProtkoll_auswertung(string s)
        {

            char[] zeichen = s.ToCharArray();

            string startnummer = zeichen[1].ToString() + zeichen[2].ToString() + zeichen[3].ToString() + zeichen[4].ToString();
            if (zeichen[0] == ' ' || zeichen[0] == 'i')
            {
                string kanal = zeichen[6].ToString() + zeichen[7].ToString() + zeichen[8].ToString();
                string uhrzeit = "";
                for (int i = 10; i <= 22; i++)
                {
                    uhrzeit = uhrzeit + zeichen[i].ToString();
                }
                tt.Startnummer = int.Parse(startnummer);
                int position = liste.IndexOf(tt);
                if (position != -1)
                {
                    if(kanal == "RT " || kanal == "RTM")
                    {
                        liste[position].Status = "OK";
                        liste[position].Endzeit = TimeSpan.Parse(uhrzeit);
                    }
                    if (kanal == "C0 " || kanal == "C0M")
                    {
                        Startzeit_zuweisen(uhrzeit, position);
                    }
                    else if (kanal == "C1 " || kanal == "C1M")
                    {
                        Zielzeit_zuweisen(uhrzeit, position);
                    }
                }
            }
            else if (zeichen[0] == 'c')
            {
                string kanal = zeichen[6].ToString() + zeichen[7].ToString() + zeichen[8].ToString();
                tt.Startnummer = int.Parse(startnummer);
                int position = liste.IndexOf(tt);
                if (position != -1)
                {
                    if (kanal == "C0" || kanal == "C0M")
                    {
                        ClearStart(position);
                    }
                    else if (kanal == "C1" || kanal == "C1M")
                    {
                        ClearZiel(position);
                    }
                }
            }
            else if (zeichen[0] == 'd')
            {
                tt.Startnummer = int.Parse(startnummer);
                int position = liste.IndexOf(tt);
                if (position != -1)
                {
                    DSQ_COMP(position);
                }
            }
        }

        private void Startzeit_zuweisen(string uhrzeit, int position)
        {
            liste[position].Startzeit = DateTime.Parse(uhrzeit);
            liste[position].Status = "DNF";
        }

        private void Zielzeit_zuweisen(string uhrzeit, int position)
        {
            liste[position].Zielzeit = DateTime.Parse(uhrzeit);
            liste[position].Status = "OK";
            liste[position].Endzeit = liste[position].Startzeit - liste[position].Zielzeit;
        }

        private void read()
        {
            keepalive = true;
            while (keepalive)
            {
                byte[] buffer = new byte[255];
                int rec = sock.Receive(buffer);
                Array.Resize(ref buffer, rec);
                string s = Encoding.Default.GetString(buffer);
                Dispatcher.Invoke(new readHandler(receive), s);
            }
        }
        private void receive(string s)
        {
            if (dev.Type == "Tag Heuer CP545")
                TagHeuerProtocol(s);
        }
        private void btn_settings_Click(object sender, RoutedEventArgs e)
        {
            Device d = new Device();
            Settings s1 = new Settings(d);
            bool? result1 = s1.ShowDialog();
            if (result1 == true)
            {
                dev.Ethernet = d.Ethernet;
                dev.IP = d.IP;
                dev.Com = d.Com;
                dev.Type = d.Type;
                dev.ComBaud = d.ComBaud;
                dev.ComPort = d.ComPort;
            }
        }

        private void listview_Initialized(object sender, EventArgs e)
        {

        }
        public  void  timer_Tick(object sender, EventArgs e)
        {
            SortView();
            Rang_zuweisen();
            Abstand_ber();
        }

        private void SortView()
        {
            using (ansicht.DeferRefresh())
            {
                ansicht.SortDescriptions.Clear();
                ansicht.SortDescriptions.Add(new SortDescription("Status", ListSortDirection.Descending));
                ansicht.SortDescriptions.Add(new SortDescription("Endzeit", ListSortDirection.Descending));
                ansicht.SortDescriptions.Add(new SortDescription("Startnummer", ListSortDirection.Ascending));
                ansicht.SortDescriptions.Add(new SortDescription("Klasse", ListSortDirection.Ascending));
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            using (StreamWriter wr = new StreamWriter(@"C:\Time.piok\" + bewerb.Name + "\\competitors.xml"))
            {
                xs.Serialize(wr, liste);
                wr.Close();
            }
        }
        private void btnstartauslos_Click(object sender, RoutedEventArgs e)
        {
            int frei = 5;
            MessageBoxResult dlR = MessageBox.Show("5 Startnummern freilassen zwischen den Klassen?", "Frage", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(dlR == MessageBoxResult.No)
            {
                frei = 0;
            }


        }

        private void btnexcel_Click(object sender, RoutedEventArgs e)
        {
            bool readWithStartNumber = false;
            MessageBox.Show("Die CSV muss folgendes Format haben: \n 1.Spalte: Startnummer \n 2.Spalte: Vorname \n 3.Spalte: Nachname \n 4.Spalte: Geschlecht \n 5.Spalte: Jahrgang");
            MessageBoxResult dlR = MessageBox.Show("Mit Startnummer einlesen?", "Frage", MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (dlR == MessageBoxResult.Yes)
            {
                readWithStartNumber = true;
            }

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Exceldateien (.csv)|*.csv;"; 
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                String[] values = File.ReadAllText(dlg.FileName,Encoding.Default).Split('\n');
                string[] strTmp;
                int i = 0;
                foreach(string str in values)
                {
                    i++;
                    Teilnehmer t = new Teilnehmer();

                    strTmp = str.Split(';');

                    if (strTmp[0] == "")
                        break;
                    try
                    {
                        if (readWithStartNumber)
                            t.Startnummer = Convert.ToInt16(strTmp[0]);
                        else
                            t.Startnummer = i;

                        t.Vorname = strTmp[1];
                        t.Nachname = strTmp[2];
                        t.Geschlecht = strTmp[3];
                        t.Geburtsjahr = Convert.ToInt16(strTmp[4]);
                        t.Klasse = "Standart";
                        t.Status = "DNS";
                    }
                    catch (Exception ex)
                    {

                    }
                    tt.Startnummer = t.Startnummer;
                    int position = liste.IndexOf(tt);
                    if (position == -1)
                    {
                        liste.Add(t);

                        using (StreamWriter wr = new StreamWriter(@"C:\Time.piok\" + bewerb.Name + "\\competitors.xml"))
                        {
                            xs.Serialize(wr, liste);
                            wr.Close();
                        }
                    }
                }
            }
        }
        private void btnklasszu_Click(object sender,RoutedEventArgs e)
        {
            for (int x = 0; x < listek.Count; x++)
            {
                for (int i = 0; i < liste.Count; i++)
                {
                    if(listek[x].Geschlecht == liste[i].Geschlecht)
                    {
                        if (liste[i].Geburtsjahr <= listek[x].Endjahr && liste[i].Geburtsjahr >= listek[x].Anfangsjahr)
                            liste[i].Klasse = listek[x].Name;
                    }
                }
            }
            MessageBox.Show("Alle Teilnehmer wurden den entsprechenden Klassen zugewiesen!");
        }
        
    }
}
