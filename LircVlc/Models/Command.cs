using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LircVlc.Models
{
    public class Command
    {
        public string Remote { get; set; }
        public string Button { get; set; }
        public string VLCCommand { get; set; }
        public long LastSeen { get; set; }
    }
}
