using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace getMail
{
    class Ticket
    {
        // public eller private diskuterer vi bagefter. Jeg gik ud fra at det, der kan ændres må være public.
        private int _tID;
        private string _subjectLine;
        private string _request;
        public string Comment;
        public string Priority;
        public string Status;
        private Customer _customer; // metode for at hente kunden fra DB? 
        private DateTime _timeStamp;


        public int Tid
        {
            get { return _tID; }
            set { _tID = value; }
        }

        public string SubjectLine
        {
            get { return _subjectLine; }
            set { _subjectLine = value; }
        }
        public string Request
        {
            get { return _request; }
            set { _request = value; }
        }

        public string CustomerEmail
        {
            get { return _customer.Email; }
            set
            {
                if (value.Contains("@"))
                {
                    _customer.Email = value;
                }
                else
                {
                    throw new Exception("Not an email!");
                }
            }
        }

        public DateTime TimeStamp
        {
            get { return _timeStamp; }
            set { _timeStamp = value; }
        }
        //public Ticket(string request, DateTime timeStamp, string email)
        //{
        //    Request = request;
        //    CustomerEmail = email;
        //    TimeStamp = timeStamp;

        //}
        public Ticket(int tId)
        {
        // aflæser ticket fra en DB
        }


    }

}
