using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace getMail
{
    class Database
    {

        private SqlConnection connection;
        private SqlCommand command; 

       // Metoden indeholder connectionstring.
        private string GetConnectionString()
        {
            return "Data Source = den1.mssql5.gear.host; Initial catalog = edgemodat; User ID = edgemodat; password = Bq5rvZa_8C_g";
        }

       // Metoden åbner SQL-connection og indeholder command.
        private void PrepareSql(string sqlString, List<SqlParameter> paramList = null)
        {
            connection = new SqlConnection(GetConnectionString());
            connection.Open();
            command = new SqlCommand
            {
                Connection = connection,
                CommandText = sqlString
            };

            if (paramList != null) command.Parameters.AddRange(paramList.ToArray());

        }

        //HAR NAVNGIVET DEN ANDERLEDES
        // Metoden bruges for ExecuteNonQuery() commands, som Create, Update, Delete.
        // Lagrer det ticketId som er blevet genereret under create, så vi kan bruge det under oprettelse af korrespondancen
        public int ExecuteNonQuerySql(string sqlString, bool isUpdateOrDelete = false, List <SqlParameter> paramList = null)
        {
            PrepareSql(sqlString, paramList);
            int affectedRows = command.ExecuteNonQuery(); //Tjekker hvor mange rækker der er blevet påvirket 

            if (isUpdateOrDelete) return 0;

            if (affectedRows == 1)
            {
                //Sæt ny sql men bibehold commandobjectet så vi ikke får en ny connection og derved mister id.
                command.CommandText = "SELECT @@IDENTITY as ID";
                SqlDataReader reader =  command.ExecuteReader(); //Henter id af den seneste record.
                reader.Read();
                return ((int)reader.GetDecimal(0)); // hent som decimal og cast/konverter som int og returer int'en
            }

            connection.Close();
            return -1;

            
        }

        /// <summary>
        /// Metoden åbner forbindelsen til db og executeReader sender sql til db og får et reader object tilbage.
        /// Pegepind der holder styr på læsningen. 
        /// </summary>
        /// <param name="sqlString"></param>
        /// <returns></returns>
        public SqlDataReader ExecuteQuery(string sqlString)
        {
            PrepareSql(sqlString);         
            return command.ExecuteReader();
        }



        public void Close()
        {
            this.connection.Close();
        }

        //KODET AF LISA
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="timeStamp">e</param>
        /// <param name="mailAddress"></param>
        /// <returns></returns>
        public int CreateTicket(string request, DateTime timeStamp, string mailAddress)
        {
            string sqlString = "INSERT INTO Ticket (request, ti_timeStamp, customer) " +
            "VALUES (@request, @timeStamp, (SELECT c_id FROM customer WHERE mailAddress = @mailAddress))";

            List<SqlParameter> myParams = new List<SqlParameter>();
            myParams.Add(new SqlParameter("@request", request));
            myParams.Add(new SqlParameter("@timeStamp", timeStamp));
            myParams.Add(new SqlParameter("@mailAddress", mailAddress.ToLower()));

            int tId = ExecuteNonQuerySql(sqlString, false, myParams);

            return tId; 
        }

        /// <summary>
        /// Insert new customer/company
        /// </summary>
        /// <param name="c_name">companyName</param>
        /// <param name="mailAddress"></param>
        /// <param name="team">The team that handles the company</param>
        /// <returns></returns>
        public int NewCustomer(string c_name, string mailAddress, int team)
        {
            string sqlString = "INSERT INTO customer (c_name, mailAddress, team) " +
            "VALUES (@companyName, @mailAddress, @team)";

            List<SqlParameter> myParams = new List<SqlParameter>();
            myParams.Add(new SqlParameter("@companyName", c_name));
            myParams.Add(new SqlParameter("@mailAddress", mailAddress.ToLower()));
            myParams.Add(new SqlParameter("@team", team));

            int c_Id = ExecuteNonQuerySql(sqlString, false, myParams);

            return c_Id;
        }


        /// <summary>
        /// Insætter mailen i correspondence tabellen og sætter ticket_id som foreign key.
        /// </summary>
        /// <param name="ticket_id">FK til ticket tabellen</param>
        /// <param name="MailContent">Indholdet af mailen</param>
        /// <param name="MessageID">ID der kan sikre at vi ikke gemmer samme besked flere gange, er null når vi selv afsender mails</param>
        /// <param name="TimeStamp">tidspunkt for afsendelse</param>
        /// <param name="SubjectLine">Emnet i subject feltet i mailen</param>
        /// <param name="MailAddress">afsenderens mailadresse/kunden</param>
        public void InsertCorrespondence(int ticket_id, string mailContent, DateTime timeStamp, string subjectLine, string mailAddress, bool isHtml, string messageID = "")
        {

            string sql = $"INSERT INTO correspondence (ticket_id, mail_content, message_id, time_stamp, subject , mail_address, isHtml) " +
                         $"VALUES(@ticket_id, @mailContent, @messageID, @timeStamp, @subjectLine, @mailAddress, @isHtml)";

            List<SqlParameter> myParams = new List<SqlParameter>();

            myParams.Add(new SqlParameter("@ticket_id", ticket_id));
            myParams.Add(new SqlParameter("@mailContent", mailContent));
            myParams.Add(new SqlParameter("@messageID", messageID));
            myParams.Add(new SqlParameter("@timeStamp", timeStamp));
            myParams.Add(new SqlParameter("@subjectLine", subjectLine));
            myParams.Add(new SqlParameter("@mailAddress", mailAddress.ToLower()));
            myParams.Add(new SqlParameter("@isHtml", isHtml));

            ExecuteNonQuerySql(sql, false, myParams);
        }

        public bool CustomerMailExists(string Email)
        {
            //Console.WriteLine("This email should not exist in customer table: " +Email);
            string sql = "SELECT c_id FROM customer WHERE mailAddress = '" + Email.ToLower() + "'";

            SqlDataReader sqlReader = ExecuteQuery(sql);

            return sqlReader.HasRows;
        }

       
        /// <summary>
        /// Henter hele correspondancen der har været på et ticket.
        /// </summary>
        /// <param name="ticket_id"></param>
        /// <returns></returns>
        public List<Correspondence> GetCorrespondenceByTicket(int ticket_id)
        {
            List<Correspondence> cor = new List<Correspondence>();

            string sql = "SELECT subject, mail_content, time_stamp, id FROM correspondence Where ticket_id = " + ticket_id;

            SqlDataReader sqlReader = ExecuteQuery(sql);

            
            while (sqlReader.Read())
            {
                Correspondence item = new Correspondence();
                item.subject = sqlReader.GetString(0);
                item.mail_content = sqlReader.GetString(1);
                item.time_stamp = sqlReader.GetDateTime(2);
                item.id = sqlReader.GetInt32(3);
                item.ticket_id = ticket_id;

                cor.Add(item);
            }

            sqlReader.Close();

            return cor;
        }

    }
}

