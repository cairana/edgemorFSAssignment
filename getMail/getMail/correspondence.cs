using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace getMail
{
    class Correspondence
    {
        public int id { get; set; }
        public string subject { get; set; }
        public DateTime time_stamp { get; set; }
        public string mail_content { get; set; }
        public int ticket_id { get; set; }

    }
}
