using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Time.piok
{
    public class Device
    {
        string type;
        string ip="192.168.1.50";
        bool ethernet;
        bool com;
        int baud;
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
    public string ComPort
    {
        set { comport = value; }
        get { return comport; }
}
    }
}
