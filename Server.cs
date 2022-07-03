using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serveur
{
    using System;
    using System.Data.SqlClient;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    public class Server
    {

        // Incoming data from the client.  
        public static string data = null;

        public static void StartListening()
        {
            // Data buffer for incoming data.  
            byte[] bytes = new Byte[1024];

            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);
                    Console.WriteLine("Waiting for a connection...");
                Socket handler = listener.Accept();

                while (true)
                {

                    data = null;

                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (data.IndexOf("<EOF>") > -1)
                        {
                            
                            break;
                        }
                    Console.WriteLine("Text received : {0}", data);

                    byte[] msg = Encoding.ASCII.GetBytes(ExecReq(data));

                    handler.Send(msg);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                    // Show the data on the console.  
                    // Echo the data back to the client.  

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nCliquez sur la touche Entrer pour continuer...");
            Console.Read();

        }
        public static String ExecReq(String req)
        {
            string connectionString =
            "Server = LAPTOP-AIVT04E6\\SQLEXPRESS; Database = SQL_ACADEMY; Trusted_Connection = True; MultipleActiveResultSets = true";
            //LAPTOP-AIVT04E6\SQLEXPRESS
            String result = "";
            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(req, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int i = 0;
                        while(i < reader.FieldCount)
                            {
                            String a = reader.GetName(i);
                            result = reader[i].ToString() + ";";
                            i+=1; 
                            }
                        result += "¤";
                        }
                    
                         reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return result;
                }
            }
                return result;
        }
        public static int Main(String[] args)
        {
            StartListening();
            return 0;
        }
    }
}
