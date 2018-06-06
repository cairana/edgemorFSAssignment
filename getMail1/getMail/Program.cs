using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenPop.Pop3;
using OpenPop.Mime;
using System.Threading;

namespace getMail
{
    class Program
    {
        static void Main(string[] args)
        {
            Database db = new Database();

            MailHandling mailHandler = new MailHandling();

            while (true)
            {
                //kører uendeligt (kører en omgang og sover i 3 sek)
                mailHandler.GetMail();
                Thread.Sleep(3000);
            }

            //Console.WriteLine("Alle mails er lagt ind nu");

            //// Bruges ved fremvisingen af correspondancen i FORM 3
            //List<Correspondence> show = db.GetCorrespondenceByTicket(1);

            //---------------------------------------------------------------------------------------

            ////SEND MAIL TIL KUNDEN METODE
            //Console.WriteLine("Skriv ticketId: ");
            //int ticketId = Convert.ToInt32(Console.ReadLine());
            //Console.WriteLine("Skriv besked: ");
            //string body = Console.ReadLine();

            //Svar kundemail - gemmer den afsendte mail i correspondancen.
            //Lave en subjectlinje i FORM - Send, hvor vi selv manuelt skriver Ticket[[id nummeret]] ind.
            //mailHandler.SendMail(1, "TesterTiden", "lisakjakobsen@gmail.com", "Dette er en test af dateTime", false);
            //mailHandler.SendMail(ticket_id, subjectLine, mailAddress som vi sender til, body, false);

            //----------------------------------------------------------------------------------------

            //Console.WriteLine("Skriv subjectLine: ");
            //string subjectLine = Console.ReadLine();

            //DateTime timeStamp = DateTime.Now;

            //string mailAddress = "lisaKjakobsen@gmail.com"; 
            //string mailContent = Console.ReadLine();


            ////Skal koples til CreateTicket sammen med SendInitialMail der også------- DateTime timeStamp = DateTime.Now;
            //int ticketId = db.CreateTicket(subjectLine, timeStamp, mailAddress);

            ////SENDER AUTOMATISK MAILEN TIL KUNDEN NÅR EN SAG BLIVER REGISTRERET - bruges også når vi sender en mail til kunden først----
            //mailHandler.SendInitialMail(ticketId, subjectLine, mailAddress, mailContent);

            Console.ReadKey();

        }
    }
}
