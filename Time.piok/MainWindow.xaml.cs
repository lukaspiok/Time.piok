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
        int wait;
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
            if (result1.HasValue && result1.Value)
            {
                bewerb.Name = b.Name;
                lbl_geladen.Content = b.Name;
            }
            else
            {
                MessageBox.Show("Kein Bewerb gewählt, Programm wird geschlossen!");
                this.Close();
            }
            if (b.Name != null)
            {
                try
                {
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
                    Abstand_ber();
                    if (listek.Count == 0)
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
                    timer.Interval = TimeSpan.FromMilliseconds(100);
                    timer.Start();
                    MessageBox.Show(b.Name + " wurde erfolgreich geladen");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            cb_lauf.Items.Add("1.Lauf");
            if (b.Anzlauf == 2)
                cb_lauf.Items.Add("2.Lauf");
            cb_lauf.SelectedIndex = 0;
        }

        private void btn_settings_wertungen_Click(object sender, RoutedEventArgs e)
        {
            settings_wertung sw = new settings_wertung(bewerb);
            bool? result = sw.ShowDialog();
            if(result.HasValue && result.Value)
            {

            }

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
                for (int i = 0; i < liste.Count; i++)
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
            System.Threading.Thread.Sleep(500);
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
            if (result1.HasValue && result1.Value)
            {
                XmlTextReader read = new XmlTextReader(@"C:\Time.piok\" + bewerb.Name + "\\categories.xml");
                listek = xsk.Deserialize(read) as ObservableCollection<Kategorien>;
                read.Close();
            }
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            keepalive = false;
            if (state != "Zeitnehmung starten")
            {
                if (dev.Com == true)
                    mySerialPort.Close();

                else if (dev.Ethernet == true)
                    sock.Close();
            }
            this.Close();
        }

        private void btn_starttiming_Click(object sender, RoutedEventArgs e)
        {
            state = btn_starttiming.Header.ToString();
            if (state == "Zeitnehmung starten")
            {
                if (dev.Ethernet == true)
                {
                    sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    try
                    {
                        sock.Connect(new IPEndPoint(IPAddress.Parse(dev.IP), 7000));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return;
                    }
                    Thread clientService = new Thread(new ThreadStart(read));
                    clientService.Start();
                    btn_starttiming.Header = "Zeitnehmung stoppen";
                    state = "Zeitnehmung stoppen";
                    lbl_verbunden.Content = "Verbunden: " + dev.Type + "," + dev.IP;
                }
                else if (dev.Com == true)
                {
                    if (dev.Type == "Alge TdC 8001")
                        btn_cont8001.IsEnabled = true;
                    mySerialPort = new SerialPort(dev.ComPort, dev.ComBaud, Parity.None, 8, StopBits.One);
                    mySerialPort.Handshake = Handshake.None;
                    try { mySerialPort.Open(); }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return;
                    }
                    btn_starttiming.Header = "Zeitnehmung stoppen";
                    state = "Zeitnehmung stoppen";
                    lbl_verbunden.Content = "Verbunden: " + dev.Type + "," + dev.ComPort + "," + dev.ComBaud.ToString();
                    mySerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                }
                else
                    MessageBox.Show("Keine Einstellungen gewählt!!!");
            }
            else
            {
                if (dev.Ethernet == true)
                {
                    keepalive = false;
                    System.Threading.Thread.Sleep(100);
                    sock.Close();
                    lbl_verbunden.Content = "Nicht verbunden";
                    btn_starttiming.Header = "Zeitnehmung starten";

                }
                else if (dev.Com == true)
                {
                    btn_cont8001.IsEnabled = false;
                    keepalive = false;
                    mySerialPort.Close();
                    lbl_verbunden.Content = "Nicht verbunden";
                    btn_starttiming.Header = "Zeitnehmung starten";
                }
            }
        }
        private delegate void readHandler(string s);
        private delegate void statelb(int wert, int max);
        private delegate void resetlb();
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            int length = 0;
            string indata;
            char[] bytesread = new char[16348];
            System.Threading.Thread.Sleep(200);
            while (length < mySerialPort.BytesToRead + length)
            {
                bytesread[length] = Convert.ToChar(mySerialPort.ReadByte());
                Dispatcher.Invoke(new statelb(state_lb), length, mySerialPort.BytesToRead + length);
                System.Threading.Thread.Sleep(wait);
                length++;
            }
            Dispatcher.Invoke(new resetlb(reset_lb));
            indata = new string(bytesread, 0, length);
            mySerialPort.DiscardInBuffer();
            Dispatcher.Invoke(new readHandler(serialread), indata);

        }
        private void state_lb(int wert, int max)
        {
            prgMain.Value = wert * (100 / Convert.ToDouble(max));
        }
        private void reset_lb()
        {
            prgMain.Value = 0;
        }
        
        private void serialread(string s)
        {
            if (dev.Type == "Alge TdC 8001")
                AlgeProtocol(s);

            else if (dev.Type == "Tag Heuer CP545")
                TagHeuerProtocol(s);

            else if (dev.Type == "Microgate Rei2")
            {
                int iLength = 0; ;
                string[] strCutString;
                string strCommand = "";
                strCutString = s.Split('\n');
                for (int i = 0; i < strCutString.Length; i++)
                {
                    strCutString[i] += '\n';
                    do
                    {
                        iLength = strCutString[i].Length;

                        if (iLength >= 52)
                        {
                            strCommand = strCutString[i];
                            if (iLength > 52)
                                strCommand = strCommand.Remove(52, strCommand.Length - 52);

                            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Time.piok\" + bewerb.Name + "\\timing_log.txt", true))
                                file.WriteLine(strCommand);

                            MicrogateProtocol(strCommand);
                            strCutString[i] = strCutString[i].Remove(0, 52);
                        }
                        else
                            break;
                    } while (iLength != 52);
                }
            }
        }

        private void TagHeuerProtocol(string s)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Time.piok\" + bewerb.Name + "\\timing_log.txt", true))
                file.WriteLine(s);

            SplitLineTagHeuer(s);
        }
        private void TagHeuerProtocol_AusWertung(string s)
        {
            if (s.Contains("TN") || s.Contains("T-") || s.Contains("!N") || s.Contains("T+") || s.Contains("T="))
            {
                listb.Items.Add(s);
                string[] Teile = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                Teilnehmer tt = new Teilnehmer();
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
                else if (Teile[0] == "!N" || Teile[0] == "!+" || Teile[0] == "!="  || Teile[0]  == "!*")
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
            }
        }
        private void SplitLineTagHeuer(string s)
        {
            string strLine;
            string strTmp;
            int index;
            strTmp = s;
            index = strTmp.IndexOf("\n");
            if (s.Length < 5 || index <= 0)
                return;

            strLine = strTmp.Remove(index, strTmp.Length - index);
            TagHeuerProtocol_AusWertung(strLine);
            s = s.Remove(0, s.IndexOf("\n") + 1);
            SplitLineTagHeuer(s);
        }
        private void ClearZiel(int position)
        {         
                liste[position].Zielzeit = DateTime.Parse("00:00:00.00000");
                if (cb_lauf.SelectedIndex == 0)
                {
                    liste[position].Lauf1 = TimeSpan.Parse("00:00:00.00000");
                    liste[position].Status = "DNF";
                }
                else if (cb_lauf.SelectedIndex == 1)
                {
                    liste[position].Lauf2 = TimeSpan.Parse("00:00:00.00000");
                    liste[position].Status2 = "DNF";
                }
                liste[position].Endzeit = TimeSpan.Parse("00:00:00.00000");
                SortView();
                Rang_zuweisen();
                Abstand_ber();
            
        }

        private void ClearStart(int position)
        {
            liste[position].Startzeit = DateTime.Parse("00:00:00.00000");
            liste[position].Zielzeit = DateTime.Parse("00:00:00.00000");
            liste[position].Endzeit = TimeSpan.Parse("00:00:00.00000");
            if (cb_lauf.SelectedIndex == 0)
            {
                liste[position].Status = "DNS";
                liste[position].Lauf1 = TimeSpan.Parse("00:00:00.00000");
            }
            else if(cb_lauf.SelectedIndex==1)
            {
                liste[position].Status2 = "DNS";
                liste[position].Lauf1 = TimeSpan.Parse("00:00:00.00000");
            }
            SortView();
            Rang_zuweisen();
            Abstand_ber();
        }

        private void MicrogateProtocol(string strCommand)
        {
            listb.Items.Add(strCommand);
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
                        if (kanal == "315")
                        {
                            Zielzeit_zuweisen(uhrzeit, position);
                        }
                        else if (kanal == "300")
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
            liste[position].Status2 = "DSQ";
            liste[position].Startzeit = DateTime.Parse("00:00:00.00000");
            liste[position].Zielzeit = DateTime.Parse("00:00:00.00000");
            liste[position].Endzeit = TimeSpan.Parse("00:00:00.000000");
            SortView();
            Rang_zuweisen();
            Abstand_ber();
        }
        private void Abstand_ber()
        {
            int position = 0;
            Teilnehmer th = new Teilnehmer();
            th.Rang = 1;
            if (liste.Count <= 0)
                return;
            for (int i = 0; i < listek.Count; i++)
            {
                th.Klasse = listek[i].Name;
                position = 0;

                for (int q = 0; q < liste.Count; q++)
                {
                    if (liste[q].Rang == 1 && liste[q].Klasse == th.Klasse)
                    {
                        position = q;
                        break;
                    }
                }

                for (int x = 0; x < liste.Count; x++)
                {
                    if (position != -1)
                    {
                        if (liste[x].Klasse == th.Klasse)
                        {
                            if (liste[x].Status == "OK")
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

            SplitLineAlge(s);
        }

        private bool IsLineValidAlge(string s)
        {
            bool result = false;
            Regex myPattern = new Regex("[0-9]{4} [A-Z]");
            Regex Number = new Regex("n[0-9{4}]");
            if (Number.IsMatch(s) || myPattern.IsMatch(s))
                result = true;
            return result;
        }

        private void SplitLineAlge(string s)
        {
            string strLine;
            string strTmp;
            int index;

            strTmp = s;

            index = strTmp.IndexOf("\r");
            if (s.Length < 5 || index <= 0)
                return;

            strLine = strTmp.Remove(index, strTmp.Length - index);

            if (IsLineValidAlge(strLine))
                AlgeProtkoll_auswertung(strLine);

            s = s.Remove(0, s.IndexOf("\r") + 1);

            SplitLineAlge(s);
        }

        private void AlgeProtkoll_auswertung(string s)
        {

            char[] zeichen = s.ToCharArray();
            listb.Items.Add(s);
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
                    if (kanal == "RT " || kanal == "RTM" || kanal=="MT " || kanal=="MTM")
                    {
                        liste[position].Status = "OK";
                        if(cb_lauf.SelectedIndex==0)
                            liste[position].Lauf1 = TimeSpan.Parse(uhrzeit);
                        else if(cb_lauf.SelectedIndex==1)
                            liste[position].Lauf2 = TimeSpan.Parse(uhrzeit);
                        liste[position].Endzeit = liste[position].Lauf1 + liste[position].Lauf2;
                        SortView();
                        Rang_zuweisen();
                        Abstand_ber();
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
            SortView();
            Rang_zuweisen();
            Abstand_ber();
        }

        private void Zielzeit_zuweisen(string uhrzeit, int position)
        {
            liste[position].Zielzeit = DateTime.Parse(uhrzeit);
            liste[position].Status = "OK";
            if (cb_lauf.SelectedIndex == 0)
            {
                liste[position].Lauf1 = liste[position].Startzeit - liste[position].Zielzeit;
                int precision = 2;
                const int TIMESPAN_SIZE = 7;
                int factor = (int)Math.Pow(10, (TIMESPAN_SIZE - precision));
                liste[position].Lauf1 = new TimeSpan(liste[position].Lauf1.Ticks - (liste[position].Lauf1.Ticks % factor));
            }
            else if (cb_lauf.SelectedIndex == 1)
            {
                liste[position].Lauf2 = liste[position].Startzeit - liste[position].Zielzeit;
                int precision = 2;
                const int TIMESPAN_SIZE = 7;
                int factor = (int)Math.Pow(10, (TIMESPAN_SIZE - precision));
                liste[position].Lauf1 = new TimeSpan(liste[position].Lauf1.Ticks - (liste[position].Lauf1.Ticks % factor));
            }
            liste[position].Endzeit = liste[position].Lauf1 + liste[position].Lauf2;
            SortView();
            Rang_zuweisen();
            Abstand_ber();
            
        }

        private void read()
        {
            keepalive = true;
            try
            {
                while (keepalive)
                {
                    byte[] buffer = new byte[255];
                    int rec = sock.Receive(buffer);
                    Array.Resize(ref buffer, rec);
                    string s = Encoding.Default.GetString(buffer);
                    Dispatcher.Invoke(new readHandler(receive), s);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }
        private void receive(string s)
        {
            if (dev.Type == "Tag Heuer CP545")
                TagHeuerProtocol(s);
        }
        private void btn_settings_device_Click(object sender, RoutedEventArgs e)
        {
            Device d = new Device();
            Settings s1 = new Settings(d);
            bool? result1 = s1.ShowDialog();
            if (result1.HasValue && result1.Value)
            {
                dev.Ethernet = d.Ethernet;
                dev.IP = d.IP;
                dev.Com = d.Com;
                dev.Type = d.Type;
                dev.ComBaud = d.ComBaud;
                dev.ComPort = d.ComPort;
            }
            if (dev.ComBaud == 1200)
                wait = 80;
            if (dev.ComBaud == 2400)
                wait = 40;
            if (dev.ComBaud == 4800)
                wait = 20;
            if (dev.ComBaud == 9600)
                wait = 10;
            else
                wait = 5;
        }


        public void timer_Tick(object sender, EventArgs e)
        {
            Uhrzeit_ausgeben();
        }
        private void Uhrzeit_ausgeben()
        {
            lbl_uhrzeit.Content = DateTime.Now.ToString(@"HH\:mm\:ss\.f");
        }
        private void SortView()
        {
            using (ansicht.DeferRefresh())
            {
                ansicht.SortDescriptions.Clear();
                ansicht.SortDescriptions.Add(new SortDescription("Status", ListSortDirection.Descending));
                ansicht.SortDescriptions.Add(new SortDescription("Endzeit", ListSortDirection.Ascending));
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

        private void SortList()
        {
            int pos = 0;
            ObservableCollection<Teilnehmer> newTeilnemer = new ObservableCollection<Teilnehmer>(liste);

            for (int q = 0; q < listek.Count; q++)
            {
                for (int i = 0; i < liste.Count; i++)
                {
                    if (liste[i].ID == listek[q].ID)
                    {
                        newTeilnemer[pos] = liste[i];
                        pos++;
                    }
                }
            }
            liste = newTeilnemer;
        }

        private void btnstartauslos_Click(object sender, RoutedEventArgs e)
        {
            int frei = 0;
            int anzteil = 0;
            int oldanzteil = 0;
            int iPos = 0;
            Random r = new Random();
            int actualNumber;
            freilassen fr = new freilassen();
            bool? result = fr.ShowDialog();
            if (result == true)
            {
                frei = fr.GetFei;
                SortList();
                bool[] taken = new bool[(frei * listek.Count) + liste.Count];
                for (int z = 0; z < listek.Count; z++)
                {
                    anzteil = 0;
                    for (int x = 0; x < liste.Count; x++)
                    {
                        if (liste[x].Klasse == listek[z].Name)
                        {
                            anzteil++;
                        }
                    }
                    for (int i = oldanzteil; i < (anzteil + oldanzteil); i++)
                    {
                        if (anzteil <= 0)
                            break;
                        do
                        {
                            actualNumber = (r.Next(oldanzteil, anzteil + oldanzteil)) + 1;
                        } while (taken[(actualNumber - 1)]);
                        taken[(actualNumber - 1)] = true;
                        //Randomstartnummer
                        liste[iPos].Startnummer = actualNumber;
                        iPos++;
                    }
                    if (anzteil > 0)
                        oldanzteil = anzteil + frei + oldanzteil;
                }
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
                
                String[] values = File.ReadAllText(dlg.FileName, Encoding.Default).Split('\n');
                string[] strTmp;
                int i = 0;
                foreach (string str in values)
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
                        if (strTmp.Length == 6)
                        {
                          strTmp[5]=  strTmp[5].TrimEnd('\r');
                            t.Mannschaft = strTmp[5];
                        }
                        t.Klasse = "Standart";
                        t.Status = "DNS";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
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
        private void btnklasszu_Click(object sender, RoutedEventArgs e)
        {
            for (int x = 0; x < listek.Count; x++)
            {
                for (int i = 0; i < liste.Count; i++)
                {
                    if (listek[x].Geschlecht == liste[i].Geschlecht)
                    {
                        if (liste[i].Geburtsjahr <= listek[x].Endjahr && liste[i].Geburtsjahr >= listek[x].Anfangsjahr)
                        {
                            liste[i].Klasse = listek[x].Name;
                            liste[i].ID = listek[x].ID;
                        }
                    }
                }
            }
            SortView();
            MessageBox.Show("Alle Teilnehmer wurden den entsprechenden Klassen zugewiesen!");
            
        }

        private void cb_status_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb_status.SelectedIndex == 0)
            {
                txt_laufzeit.IsEnabled = true;
                lbl_endzeit.IsEnabled = true;
                txt_laufzeit.Text = "HH:MM:SS.ZH";
            }
            else
            {
                txt_laufzeit.IsEnabled = false;
                lbl_endzeit.IsEnabled = false;
                txt_laufzeit.Text = "";
            }
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                tt.Startnummer = int.Parse(txt_startn.Text);
                int position = liste.IndexOf(tt);
                if (position != -1)
                {
                    txt_startn.IsEnabled = false;
                    btn_save.IsEnabled = true;
                    lbl_vornach.IsEnabled = true;
                    lbl_vornach.Content = liste[position].Nachname + "," + liste[position].Vorname;
                    lbl_Kat.IsEnabled = true;
                    lbl_Kat.Content = liste[position].Klasse;
                    lbl_status.IsEnabled = true;
                    cb_status.IsEnabled = true;
                    if (liste[position].Status == "OK")
                    {
                        cb_status.SelectedIndex = 0;
                        lbl_endzeit.IsEnabled = true;
                        txt_laufzeit.IsEnabled = true;
                        if(cb_lauf.SelectedIndex==0)
                            txt_laufzeit.Text = liste[position].Lauf1.ToString();
                        else if (cb_lauf.SelectedIndex==1)
                            txt_laufzeit.Text = liste[position].Lauf1.ToString();
                    }
                    else if (liste[position].Status == "DNF")
                        cb_status.SelectedIndex = 1;
                    else if (liste[position].Status == "DNS")
                        cb_status.SelectedIndex = 2;
                    else if (liste[position].Status == "DSQ")
                        cb_status.SelectedIndex = 3;

                }
            }
        }
        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                tt.Startnummer = int.Parse(txt_startn.Text);
                int position = liste.IndexOf(tt);
                if (cb_status.SelectedIndex == 0)
                {
                    if (txt_laufzeit.Text != "")
                    {
                        if(cb_lauf.SelectedIndex==0)
                        liste[position].Lauf1 = TimeSpan.Parse(txt_laufzeit.Text);
                        else if (cb_lauf.SelectedIndex==1)
                        liste[position].Status = "OK";
                    }
                }
                else if (cb_status.SelectedIndex == 1)
                {
                    liste[position].Status = "DNF";
                    if (cb_lauf.SelectedIndex == 0)
                        liste[position].Lauf1 = TimeSpan.Parse("00:00:00.00000");
                    else if (cb_lauf.SelectedIndex == 1)
                        liste[position].Lauf2 = TimeSpan.Parse("00:00:00.00000");
                    liste[position].Endzeit = TimeSpan.Parse("00:00:00.0000000");
                }
                else if (cb_status.SelectedIndex == 2)
                {
                    if (cb_lauf.SelectedIndex == 0)
                        liste[position].Lauf1 = TimeSpan.Parse("00:00:00.00000");
                    else if (cb_lauf.SelectedIndex == 1)
                        liste[position].Lauf2 = TimeSpan.Parse("00:00:00.00000");
                    liste[position].Endzeit = TimeSpan.Parse("00:00:00.0000000");
                }
                else if (cb_status.SelectedIndex == 3)
                {
                    liste[position].Status = "DSQ";
                    if (cb_lauf.SelectedIndex == 0)
                        liste[position].Lauf1 = TimeSpan.Parse("00:00:00.00000");
                    else if (cb_lauf.SelectedIndex == 1)
                        liste[position].Lauf2 = TimeSpan.Parse("00:00:00.00000");
                    liste[position].Endzeit = TimeSpan.Parse("00:00:00.0000000");
                }
                txt_laufzeit.Text = "";
                txt_laufzeit.IsEnabled = false;
                txt_startn.IsEnabled = true;
                btn_save.IsEnabled = false;
                lbl_vornach.IsEnabled = true;
                lbl_vornach.Content = "Nachname,Vorname";
                lbl_Kat.IsEnabled = false;
                lbl_Kat.Content = "Kategorie";
                lbl_status.IsEnabled = false;
                cb_status.IsEnabled = false;
                txt_startn.Text = "";
                cb_status.SelectedIndex = -1;
                SortView();
                Rang_zuweisen();
                Abstand_ber();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btn_callrt_Click(object sender, RoutedEventArgs e)
        {
            try{mySerialPort.Write("CALRT\r");}
            catch(Exception ex){MessageBox.Show(ex.Message);}
        }

        private void btn_callc0_Click(object sender, RoutedEventArgs e)
        {
            try { mySerialPort.Write("PALST\r"); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private void btn_callc1_Click(object sender, RoutedEventArgs e)
        {
            try { mySerialPort.Write("PALFT\r"); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void btn_print_startlist_Click(object sender, RoutedEventArgs e)
        {
            SortList();
            Print_startlistcat();
        }
        private void btn_print_resultcat_Click(object sender, RoutedEventArgs e)
        {
            Print_resultcat();
          
        }
        private void btn_print_resultoverall_Click(object sender, RoutedEventArgs e)
        {
            result_overall();
        }

        private void result_overall()
        {
            ICollectionView erglisteoverall = CollectionViewSource.GetDefaultView(liste);
            erglisteoverall.SortDescriptions.Add(new SortDescription("Zeit", ListSortDirection.Ascending));
            PrintDialog prd = new System.Windows.Controls.PrintDialog();
            if (prd.ShowDialog() == true)
            {
                Paragraph myParagraph = new Paragraph();
                FlowDocument doc = new FlowDocument();
                Table table1 = new Table();
                doc.Blocks.Add(table1);
                table1.CellSpacing = 10;
                table1.Background = Brushes.White;
                int Collumnumber = 8;
                for (int i = 0; i < Collumnumber; i++)
                {
                    table1.Columns.Add(new TableColumn());
                }
                table1.RowGroups.Add(new TableRowGroup());
                table1.RowGroups[0].Rows.Add(new TableRow());
                TableRow currentRow = table1.RowGroups[0].Rows[0];
                currentRow.Background = Brushes.Silver;
                currentRow.FontSize = 40;
                currentRow.FontWeight = System.Windows.FontWeights.Bold;
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Ergebnisliste"))));
                currentRow.Cells[0].ColumnSpan = 8;
                table1.RowGroups[0].Rows.Add(new TableRow());
                currentRow = table1.RowGroups[0].Rows[1];
                currentRow.FontSize = 12;
                currentRow.FontWeight = FontWeights.Bold;
                //Feldbezeichnungen Erstellen
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Rang"))));
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("StNr"))));
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Vorname"))));
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Nachname"))));
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Jahrgang"))));
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Geschl."))));
                if(cb_lauf.SelectedIndex==1)
                {
                    currentRow.Cells.Add(new TableCell(new Paragraph(new Run("1.Lauf"))));
                    currentRow.Cells.Add(new TableCell(new Paragraph(new Run("2.Lauf"))));
                }
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Gesamt"))));
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Abstand"))));
                //Inhalt der Zeilen
                table1.RowGroups[0].Rows.Add(new TableRow());
                int row = 2;
                int catrow = 2;
                currentRow = table1.RowGroups[0].Rows[catrow];
                currentRow.FontSize = 12;
                currentRow.FontWeight = FontWeights.Normal;
                    foreach (Teilnehmer t in erglisteoverall)
                    {
                        table1.RowGroups[0].Rows.Add(new TableRow());
                        currentRow = table1.RowGroups[0].Rows[row];
                        currentRow.FontSize = 12;
                        currentRow.FontWeight = FontWeights.Normal;
                        if (row % 2 == 1)
                            currentRow.Background = Brushes.LightSalmon;
                        if (t.Rang != 0)
                            currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Rang.ToString()))));
                        else
                            currentRow.Cells.Add(new TableCell(new Paragraph(new Run(""))));
                        currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Startnummer.ToString()))));
                        currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Vorname))));
                        currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Nachname))));
                        currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Geburtsjahr.ToString()))));
                        currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Geschlecht))));
                        if (t.Rang != 0)
                        {
                            if(cb_lauf.SelectedIndex==1)
                            {
                                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Lauf1.ToString(@"mm\:ss\.ff")))));
                                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Lauf2.ToString(@"mm\:ss\.ff")))));
                            }
                            currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Endzeit.ToString(@"mm\:ss\.ff")))));
                            if(t.Rang >1)
                            currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Abstand.ToString(@"mm\:ss\.ff")))));
                        }
                        else
                        {
                            currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Status))));
                            currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Status))));
                        }
                        row++;
                        table1.RowGroups[0].Rows.Add(new TableRow());
                        currentRow = table1.RowGroups[0].Rows[row];
                        currentRow.FontSize = 12;
                        currentRow.FontWeight = FontWeights.Normal;
                    }
                    catrow = row + 1;

                //Druckeinstellungen     
                doc.PageHeight = prd.PrintableAreaHeight;
                doc.PageWidth = prd.PrintableAreaWidth;
                doc.PagePadding = new Thickness(50);
                doc.ColumnGap = 0;
                doc.ColumnWidth = prd.PrintableAreaWidth;
                IDocumentPaginatorSource dps = doc;
                prd.PrintDocument(dps.DocumentPaginator, bewerb.Name + " Ergebnissliste");
            }
        
        }
        private void btn_print_resultsex_Click(object sender, RoutedEventArgs e)
        {
            result_byges();
        }

        private void result_byges()
        {
            ICollectionView erglisteges = CollectionViewSource.GetDefaultView(liste);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Geschlecht");
            erglisteges.GroupDescriptions.Add(groupDescription);
            erglisteges.SortDescriptions.Add(new SortDescription("Endzeit", ListSortDirection.Ascending));
            PrintDialog prd = new System.Windows.Controls.PrintDialog();
            if (prd.ShowDialog() == true)
            {
                Paragraph myParagraph = new Paragraph();
                FlowDocument doc = new FlowDocument();
                Table table1 = new Table();
                doc.Blocks.Add(table1);
                table1.CellSpacing = 10;
                table1.Background = Brushes.White;
                int Collumnumber = 8;
                for (int i = 0; i < Collumnumber; i++)
                {
                    table1.Columns.Add(new TableColumn());
                }
                table1.RowGroups.Add(new TableRowGroup());
                table1.RowGroups[0].Rows.Add(new TableRow());
                TableRow currentRow = table1.RowGroups[0].Rows[0];
                currentRow.Background = Brushes.Silver;
                currentRow.FontSize = 40;
                currentRow.FontWeight = System.Windows.FontWeights.Bold;
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Ergebnisliste nach Geschlecht"))));
                currentRow.Cells[0].ColumnSpan = 8;
                table1.RowGroups[0].Rows.Add(new TableRow());
                currentRow = table1.RowGroups[0].Rows[1];
                currentRow.FontSize = 12;
                currentRow.FontWeight = FontWeights.Bold;
                //Feldbezeichnungen Erstellen
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Rang"))));
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("StNr"))));
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Vorname"))));
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Nachname"))));
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Jahrgang"))));
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Geschlecht"))));
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Gesamtzeit"))));
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Abstand"))));
                //Inhalt der Zeilen
                table1.RowGroups[0].Rows.Add(new TableRow());
                int row = 2;
                int catrow = 2;
                currentRow = table1.RowGroups[0].Rows[catrow];
                currentRow.FontSize = 12;
                currentRow.FontWeight = FontWeights.Normal;
                foreach (CollectionViewGroup g in erglisteges.Groups)
                {
                    table1.RowGroups[0].Rows.Add(new TableRow());
                    table1.RowGroups[0].Rows[catrow].Background = Brushes.Silver;
                    table1.RowGroups[0].Rows[catrow].FontSize = 12;
                    table1.RowGroups[0].Rows[catrow].FontWeight = FontWeights.Bold;
                    table1.RowGroups[0].Rows[catrow].Cells.Add(new TableCell(new Paragraph(new Run(g.Name.ToString()))));
                    table1.RowGroups[0].Rows[catrow].Cells[0].ColumnSpan = 8;
                    row = catrow + 1;


                    /*foreach (Teilnehmer t in g.Items)
                    {
                        table1.RowGroups[0].Rows.Add(new TableRow());
                        currentRow = table1.RowGroups[0].Rows[row];
                        currentRow.FontSize = 12;
                        currentRow.FontWeight = FontWeights.Normal;
                        if (row % 2 == 1)
                            currentRow.Background = Brushes.LightSalmon;
                        if (t.Rang != 0)
                            currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Rang.ToString()))));
                        else
                            currentRow.Cells.Add(new TableCell(new Paragraph(new Run(""))));
                        currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Startnummer.ToString()))));
                        currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Vorname))));
                        currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Nachname))));
                        currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Geburtsjahr.ToString()))));
                        currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Geschlecht))));
                        if (t.Rang != 0)
                        {
                            currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Endzeit.ToString()))));
                            currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Abstand.ToString()))));
                        }
                        else
                        {
                            currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Status))));
                            currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Status))));
                        }
                        row++;
                        table1.RowGroups[0].Rows.Add(new TableRow());
                        currentRow = table1.RowGroups[0].Rows[row];
                        currentRow.FontSize = 12;
                        currentRow.FontWeight = FontWeights.Normal;
                    }*/
                    catrow = row + 1;
                }

                //Druckeinstellungen     
                doc.PageHeight = prd.PrintableAreaHeight;
                doc.PageWidth = prd.PrintableAreaWidth;
                doc.PagePadding = new Thickness(50);
                doc.ColumnGap = 0;
                doc.ColumnWidth = prd.PrintableAreaWidth;
                IDocumentPaginatorSource dps = doc;
                prd.PrintDocument(dps.DocumentPaginator, bewerb.Name + " Ergebnissliste nach Geschlecht");
            }
        }
        

        private void Print_resultcat()
        {
            PrintDialog prd = new System.Windows.Controls.PrintDialog();
            if (prd.ShowDialog() == true)
            {
                Paragraph myParagraph = new Paragraph();
                FlowDocument doc = new FlowDocument();
                Table table1 = new Table();
                doc.Blocks.Add(table1);
                table1.CellSpacing = 10;
                table1.Background = Brushes.White;
                int Collumnumber = 8;
                for (int i = 0; i < Collumnumber; i++)
                {
                    table1.Columns.Add(new TableColumn());
                }
                table1.RowGroups.Add(new TableRowGroup());
                table1.RowGroups[0].Rows.Add(new TableRow());
                TableRow currentRow = table1.RowGroups[0].Rows[0];
                currentRow.Background = Brushes.Silver;
                currentRow.FontSize = 40;
                currentRow.FontWeight = System.Windows.FontWeights.Bold;
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Ergebnisliste"))));
                currentRow.Cells[0].ColumnSpan = 8;
                table1.RowGroups[0].Rows.Add(new TableRow());
                currentRow = table1.RowGroups[0].Rows[1];
                currentRow.FontSize = 12;
                currentRow.FontWeight = FontWeights.Bold;
                //Feldbezeichnungen Erstellen
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Rang"))));
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("StNr"))));
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Vorname"))));
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Nachname"))));
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Jahrgang"))));
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Geschl."))));
                if(cb_lauf.SelectedIndex == 0)
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Gesamt"))));
                else if(cb_lauf.SelectedIndex == 1)
                {
                    currentRow.Cells.Add(new TableCell(new Paragraph(new Run("1.Lauf"))));
                    currentRow.Cells.Add(new TableCell(new Paragraph(new Run("2.Lauf"))));
                    currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Gesamt"))));
                }
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Abstand"))));
                //Inhalt der Zeilen
                table1.RowGroups[0].Rows.Add(new TableRow());
                int row = 2;
                int catrow = 2;
                currentRow = table1.RowGroups[0].Rows[catrow];
                currentRow.FontSize = 12;
                currentRow.FontWeight = FontWeights.Normal;
                foreach (CollectionViewGroup g in ansicht.Groups)
                {
                    table1.RowGroups[0].Rows.Add(new TableRow());
                    table1.RowGroups[0].Rows[catrow].Background = Brushes.Silver;
                    table1.RowGroups[0].Rows[catrow].FontSize = 12;
                    table1.RowGroups[0].Rows[catrow].FontWeight = FontWeights.Bold;
                    table1.RowGroups[0].Rows[catrow].Cells.Add(new TableCell(new Paragraph(new Run(g.Name.ToString()))));
                    table1.RowGroups[0].Rows[catrow].Cells[0].ColumnSpan = 8;
                    row = catrow + 1;


                    foreach (Teilnehmer t in g.Items)
                    {
                        table1.RowGroups[0].Rows.Add(new TableRow());
                        currentRow = table1.RowGroups[0].Rows[row];
                        currentRow.FontSize = 12;
                        currentRow.FontWeight = FontWeights.Normal;
                        if (row % 2 == 1)
                            currentRow.Background = Brushes.LightSalmon;
                        if (t.Rang != 0)
                            currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Rang.ToString()))));
                        else
                            currentRow.Cells.Add(new TableCell(new Paragraph(new Run(""))));
                        currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Startnummer.ToString()))));
                        currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Vorname))));
                        currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Nachname))));
                        currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Geburtsjahr.ToString()))));
                        currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Geschlecht))));
                        if (t.Rang != 0)
                        {
                            if (cb_lauf.SelectedIndex == 0)
                            {
                                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Endzeit.ToString(@"mm\:ss\.ff")))));
                                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Abstand.ToString(@"mm\:ss\.ff")))));
                            }
                            else if(cb_lauf.SelectedIndex==1)
                            {
                                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Lauf1.ToString(@"mm\:ss\.ff")))));
                                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Lauf2.ToString(@"mm\:ss\.ff")))));
                                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Endzeit.ToString(@"mm\:ss\.ff")))));
                               if(t.Rang!=1)
                                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Abstand.ToString(@"mm\:ss\.ff")))));
                            }
                        }
                        else
                        {
                            if (cb_lauf.SelectedIndex == 1)
                            {
                                if(t.Lauf1.TotalMilliseconds==0)
                                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Status))));
                                else
                                    currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Lauf1.ToString(@"mm\:ss\.ff")))));
                                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Status))));
                            }
                            currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Status))));
                            currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Status))));
                        }
                        row++;
                        table1.RowGroups[0].Rows.Add(new TableRow());
                        currentRow = table1.RowGroups[0].Rows[row];
                        currentRow.FontSize = 12;
                        currentRow.FontWeight = FontWeights.Normal;
                    }
                    catrow = row + 1;
                }

                //Druckeinstellungen     
                doc.PageHeight = prd.PrintableAreaHeight;
                doc.PageWidth = prd.PrintableAreaWidth;
                doc.PagePadding = new Thickness(50);
                doc.ColumnGap = 0;
                doc.ColumnWidth = prd.PrintableAreaWidth;
                IDocumentPaginatorSource dps = doc;
                prd.PrintDocument(dps.DocumentPaginator, bewerb.Name + " Ergebnissliste");
            }
        }
        private void Print_startlistcat()
        {
            PrintDialog prd = new System.Windows.Controls.PrintDialog();
            if (prd.ShowDialog() == true)
            {
                 ICollectionView startliste = CollectionViewSource.GetDefaultView(liste);
                 PropertyGroupDescription groupDescription = new PropertyGroupDescription("Klasse");
                 startliste.GroupDescriptions.Add(groupDescription);
                 startliste.SortDescriptions.Add(new SortDescription("Startnummer", ListSortDirection.Ascending));
                Paragraph myParagraph = new Paragraph();
                FlowDocument doc = new FlowDocument();
                Table table1 = new Table();
                doc.Blocks.Add(table1);
                table1.CellSpacing = 10;
                table1.Background = Brushes.White;
                int Collumnumber = 5;
                for (int i = 0; i < Collumnumber; i++)
                {
                    table1.Columns.Add(new TableColumn());
                }
                table1.RowGroups.Add(new TableRowGroup());
                table1.RowGroups[0].Rows.Add(new TableRow());
                TableRow currentRow = table1.RowGroups[0].Rows[0];
                currentRow.Background = Brushes.Silver;
                currentRow.FontSize = 40;
                currentRow.FontWeight = System.Windows.FontWeights.Bold;
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Startliste"))));
                currentRow.Cells[0].ColumnSpan = 8;
                table1.RowGroups[0].Rows.Add(new TableRow());
                currentRow = table1.RowGroups[0].Rows[1];
                currentRow.FontSize = 12;
                currentRow.FontWeight = FontWeights.Bold;
                //Feldbezeichnungen Erstellen
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("StNr"))));
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Vorname"))));
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Nachname"))));
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Jahrgang"))));
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Geschlecht"))));
                //Inhalt der Zeilen
                table1.RowGroups[0].Rows.Add(new TableRow());
                int row = 2;
                int catrow = 2;
                currentRow = table1.RowGroups[0].Rows[catrow];
                currentRow.FontSize = 12;
                currentRow.FontWeight = FontWeights.Normal;
                foreach (CollectionViewGroup g in startliste.Groups)
                {
                    table1.RowGroups[0].Rows.Add(new TableRow());
                    table1.RowGroups[0].Rows[catrow].Background = Brushes.Silver;
                    table1.RowGroups[0].Rows[catrow].FontSize = 12;
                    table1.RowGroups[0].Rows[catrow].FontWeight = FontWeights.Bold;
                    table1.RowGroups[0].Rows[catrow].Cells.Add(new TableCell(new Paragraph(new Run(g.Name.ToString()))));
                    table1.RowGroups[0].Rows[catrow].Cells[0].ColumnSpan = 8;
                    row = catrow + 1;


                    foreach (Teilnehmer t in g.Items)
                    {
                        table1.RowGroups[0].Rows.Add(new TableRow());
                        currentRow = table1.RowGroups[0].Rows[row];
                        currentRow.FontSize = 12;
                        currentRow.FontWeight = FontWeights.Normal;
                        if (row % 2 == 1)
                            currentRow.Background = Brushes.LightSalmon;
                        currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Startnummer.ToString()))));
                        currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Vorname))));
                        currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Nachname))));
                        currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Geburtsjahr.ToString()))));
                        currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t.Geschlecht))));
                        row++;
                        table1.RowGroups[0].Rows.Add(new TableRow());
                        currentRow = table1.RowGroups[0].Rows[row];
                        currentRow.FontSize = 12;
                        currentRow.FontWeight = FontWeights.Normal;
                    }
                    catrow = row + 1;
                }
                //Druckeinstellungen     
                doc.PageHeight = prd.PrintableAreaHeight;
                doc.PageWidth = prd.PrintableAreaWidth;
                doc.PagePadding = new Thickness(50);
                doc.ColumnGap = 0;
                doc.ColumnWidth = prd.PrintableAreaWidth;
                IDocumentPaginatorSource dps = doc;
                prd.PrintDocument(dps.DocumentPaginator, bewerb.Name + " Startliste");

            }
        }

        private void cb_lauf_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            for (int i = 0; i < liste.Count;i++ )
            {
                if (cb_lauf.SelectedIndex == 0)
                {
                    if (liste[i].Lauf1.TotalMilliseconds!=0)
                    {
                        liste[i].Status = "OK";
                    }
                }
                else if (cb_lauf.SelectedIndex == 1)
                    if (liste[i].Status != "DSQ" && liste[i].Status!="DNF")
                        liste[i].Status = "DNS";
                writelist();
            }
                
        }
    }
}
