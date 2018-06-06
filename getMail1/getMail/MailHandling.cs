using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using OpenPop.Pop3;

namespace getMail
{

    class MailHandling
    {

        Database db = new Database();


        // Opret Kunden hvis mailen ikke exisisterer - Metode (Use messenger_id, if exist)

        int ticketId = 0;
        string mailContent;
        string messageId;
        DateTime timeStamp;
        string subjectLine;
        string mailAddress;

        public void GetMail()
        {

            bool isHtml = false;
            //string c_name;
            //int team;

            Pop3Client client = new Pop3Client();

            client.Connect("pop.gmail.com", 995, true); //For SSL                
            client.Authenticate("recent:edgemo.omega@gmail.com", "Edgemo2018",
                                    AuthenticationMethod.UsernameAndPassword);

            int messageCount = client.GetMessageCount();

            for (int i = messageCount; i > 0; i--)
            {
                messageId = client.GetMessage(i).Headers.MessageId;
                OpenPop.Mime.Message msg = client.GetMessage(i);

                if (msg.FindFirstPlainTextVersion() == null)
                {
                    this.mailContent = msg.MessagePart.GetBodyAsText();
                    //Hvis teksten er i html, så står der true i isHtml collonnen.
                    //Bruges til at bestemme hvordan teksten skal håndteres.
                    isHtml = true;
                }
                else
                {
                    OpenPop.Mime.MessagePart plainTextPart = msg.FindFirstPlainTextVersion();
                    this.mailContent = plainTextPart.GetBodyAsText();
                    isHtml = false;
                }
                //Sikrer at datetime er dansk tidszone
                
                
                timeStamp = (msg.Headers.DateSent).ToLocalTime();

                subjectLine = msg.Headers.Subject.ToString();
                mailAddress = msg.Headers.From.MailAddress.Address.ToString();

                if (MessageIdExists(messageId))
                {
                    break;
                }

                ////SE HER
                //Hvis vi kan nå at lave en FORM til denne, så er koden lavet. Læs nedenfor (try/catch eller andet for at fange fejlindtastning)
                if (!db.CustomerMailExists(mailAddress))
                {                   
                    int c_id = db.NewCustomer("Ukendt", mailAddress, 2);                  
                }

                ticketId = TicketIdInSubjectLine(subjectLine);
                db.InsertCorrespondence(ticketId, mailContent, timeStamp, subjectLine, mailAddress, isHtml, messageId);
                //Console.WriteLine("Correspondence is added to ticket: " + ticketId);

            }
            //Console.WriteLine("Alle nye mails er hermed registreret");

        }


        /// <summary>
        /// Kalder db og tjekker om messageId existerer i correspondance - returnerer true hvis der er en række
        /// </summary>
        /// <param name="messageId"></param>
        /// <returns></returns>
        public bool MessageIdExists(string messageId)
        {
            string sql = "SELECT id FROM correspondence WHERE message_id = '" + messageId + "'";

            SqlDataReader sqlReader = db.ExecuteQuery(sql);

            return sqlReader.HasRows;
        }


        /// <summary>
        /// Tjekker om der er ticketId i subjectlinjen
        /// </summary>
        /// <param name="subjectLine"></param>
        /// <returns>ticketId</returns>
        public int TicketIdInSubjectLine(string subjectLine)
        {
            int ticketId = 0;
            try
            {
                int pFrom = subjectLine.IndexOf("Ticket[[") + "Ticket[[".Length;
                int pTo = subjectLine.LastIndexOf("]]");

                ticketId = Convert.ToInt32(subjectLine.Substring(pFrom, pTo - pFrom));
            }
            catch
            {
                ticketId = db.CreateTicket(subjectLine, timeStamp, mailAddress);
                SendInitialMail(ticketId, subjectLine, mailAddress);
            }

            return ticketId;
        }

        public void SendInitialMail(int ticketId, string subjectLine, string mailAddress, string mailContent = "")
        {
  
            string body = "Din henvendelse er registreret: \n\n" +
                           mailContent + "\n \n" +
                          "Den vil blive behandlet inden for 2 arb.dage. \n\n" +
                          "Dit ticket nr står i subject line - så hvis du ikke ændrer subject kan du svare tilbage \n" +
                          "på denne og efterfølgende mails og din henvendelse vil blive registreret på sagen. \n\n" +

                            "MVh \n" +
                            "Edgemo";

            SendMail(ticketId, subjectLine, mailAddress, body, false);
        }


        /// <summary>
        /// Kaldes med parameterne når der skal sendes en mail.
        /// </summary>
        /// <param name="ticketId"></param>
        /// <param name="subjectLine"></param>
        /// <param name="mailAddress"></param>
        /// <param name="body"></param>
        /// <param name="isHtml"></param>
        public void SendMail(int ticketId, string subjectLine , string customerMailAddress, string body, bool isHtml)
        {
            //Sikrer at datetime er dansk tidszone
            System.Globalization.CultureInfo.CurrentCulture.ClearCachedData();

            DateTime timestamp = DateTime.Now;

            //Hvis der ikke står ticketid på subjectline så sæt det på.
            if (!subjectLine.Contains("Ticket[[")) subjectLine = "Ticket[[" + ticketId + "]] " + subjectLine;

            try
            {
                NetworkCredential credentials = new NetworkCredential("edgemo.omega@gmail.com", "Edgemo2018");

                MailMessage mail = new MailMessage()
                {
                    From = new MailAddress("edgemo.omega@gmail.com", "Edgemo Team Omega"),
                    Subject = subjectLine,                
                    Body = body,

                };

                mail.To.Add(new MailAddress(customerMailAddress));
                mail.IsBodyHtml = isHtml; //Kan bruges ved at tjekke om inkomne mails bliver sendt som HTML - FindFirstPlainTextVersion();

                SmtpClient smtpClient = new SmtpClient()
                {
                Port = 587,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Host = "smtp.gmail.com",
                EnableSsl = true,
                Credentials = credentials               
                };

                smtpClient.Send(mail);

                //Afsendte mail logges i correspondancen
                db.InsertCorrespondence(ticketId, body, timestamp, subjectLine, "edgemo.omega@gmail.com", false);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in sending email: " + ex.Message);
                Console.ReadKey();
                return;
            };

            Console.WriteLine("Email sccessfully sent");
            Console.ReadKey();


        }
        
    } }

