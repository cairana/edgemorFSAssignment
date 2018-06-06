using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace getMail
{
    class Customer
    {

        private int _cID;
        private string _name;
        public string Team;
        private string _email;
        public int Cid
        {
            get { return _cID; }
            set { _cID = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        public Customer(string email)
        {
            Email = email;
        }

     


    }

}

