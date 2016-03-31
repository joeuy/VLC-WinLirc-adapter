using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LircVlc.Models
{
    public class Config
    {
        public IPEndPoint Lirc { get; set; }
        public string Vlc { get; set; }
    }
}
