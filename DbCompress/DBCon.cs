using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DbCompress
{
    public class DBCon
    {
        /// <summary>
        /// Method that Update file from DB
        /// </summary>
        /// <param name="newXml"></param>
        /// <returns>null</returns>
        public static string UpdateFile(string newXml)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnection"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"dbo.[UpdateFile]", con)
            {
                CommandType = CommandType.StoredProcedure,
                Connection = con
            };
            cmd.Parameters.AddWithValue("@xml", newXml);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            return null;
        }

        /// <summary>
        /// Method that return files from DB
        /// </summary>
        /// <returns>files</returns>
        public static IEnumerable<File> GetFile()
        {
            var files = new List<File>();
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnection"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"dbo.[GetAllFiles]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            var reader = cmd.ExecuteReader();


            while (reader.Read())
            {
                files.Add(new File
                {
                    FileName = (string)reader["FileName"],
                    Server = (string)reader["Server"],
                    LastUpdateDate = (DateTime)reader["LastUpdateDate"]
                });


                if (files[0].Server == (string)reader["Server"])
                {
                    Server servers = new Server
                    {
                        Name = (string)reader["Server"],
                        Files = files
                    };
                }




            }
            con.Close();
            return files;
        }

        public static IEnumerable<Server> GetServers()
        {
            var servers = new List<Server>();
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnection"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"dbo.[GetAllFiles]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            var reader = cmd.ExecuteReader();


            while (reader.Read())
            {
                string serverName = (string)reader["Server"];
                var server = servers.FirstOrDefault(s => s.Name == serverName);
                if (server == null)
                {
                    server = new Server()
                    {
                        Name = (string)reader["Server"],
                        Files = new List<File>()
                    };
                    servers.Add(server);
                }

                server.Files.Add(new File()
                {
                    FileName = (string)reader["FileName"],
                    LastUpdateDate = (DateTime)reader["LastUpdateDate"]
                });

            }
            con.Close();
            return servers;
        }


    }
}
