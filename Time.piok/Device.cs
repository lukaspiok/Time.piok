using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Time.piok
{
    public class Device
    {
        string type;
        string ip="192.168.1.50";
        bool ethernet;
        bool com;
        int baud;
        bool board;
        string type_board;
        string com_board;
        int baud_board;
        string comport;

        public string Type
        {
            set { type = value; }
            get { return type; }
        }
        public string IP
        {
            set { ip = value; }
            get { return ip; }
        }
        public bool Ethernet
        { set { ethernet = value; }
            get { return ethernet; }
        }
        public bool Com
        {
            set {com=value;}
            get{return com;}
        }
        public int ComBaud
        {
            set { baud = value; }
            get { return baud; }
        }
        [XmlIgnore]
        public string ComPort
        {
        set { comport = value; }
        get { return comport; }
        }
        [XmlIgnore]
        public string ComBoard
        {
            set { com_board = value; }
            get {return com_board;}
        }
        public bool Board
        {
            set { board = value; }
            get { return board; }
        }
        public int BaudBoard
        {
            set { baud_board = value; }
            get { return baud_board; }
        }
        public string BoardType
        {
            set { type_board = value; }
            get { return type_board; }
        }
    }
}
