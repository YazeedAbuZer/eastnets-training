using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.Xsl;

namespace DbCompress
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                                   | SecurityProtocolType.Tls11
                                                   | SecurityProtocolType.Tls12;
            var paths = ConfigurationManager.AppSettings;
            var server = ConfigurationManager.AppSettings["Server"];
            var list = new List<File>();

            foreach (string key in paths)
            {
                if (key.EndsWith("Path"))
                {
                    string expectedFilePath = $"{paths[key]}_{DateTime.Now:yyyyMMdd}.rar";

                    var file = new File
                    {
                        FileName = paths[key],
                        Server = server,
                        LastUpdateDate = DateTime.Now
                        //Status = IsFileExist(expectedFilePath)
                    };
                    list.Add(file);
                }

            }

            var n= DBCon.GetServers();
            string xmln = GetXMLFromObject(n);
            string newXmlnn = RemoveAllNamespaces(xmln);
            Console.WriteLine(newXmlnn);



            string xml = GetXMLFromObject(list);
            string newXml = RemoveAllNamespaces(xml);
            //DBCon.UpdateFile(newXml);

            if (Convert.ToBoolean(ConfigurationManager.AppSettings["SendEmail"]))
            {
                var files = DBCon.GetFile();

                bool isSuccess =  !files.Any(f => f.LastUpdateDate.Date != DateTime.Now.Date);
                string emailMessage = EmailBody(files);
                SendEmail(emailMessage, ConfigurationManager.AppSettings["ToEmail"], isSuccess);
            }
        }

        public static string Getxsl()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string FileName = "DbCompress.Files.transform.xsl";
            StreamReader reader = new StreamReader(assembly.GetManifestResourceStream(FileName));
            string result = reader.ReadToEnd();
            return result;
        }

        public static string EmailBody(IEnumerable<File> files)
        {
            string serverName = files.FirstOrDefault()?.Server;
            DateTime LastCheckIn = DateTime.Now;

            //var getFiles = DBCon.GetFile();
            //var getFilesXml = GetXMLFromObject(getFiles);
            //var xmlInput = RemoveAllNamespaces(getFilesXml);

            var getFiles = DBCon.GetServers();
            string getFilesXml = GetXMLFromObject(getFiles);
            string xmlInput = RemoveAllNamespaces(getFilesXml);


            string output = "";
            using (StringReader xsltStringReader = new StringReader(Getxsl())) // xslInput is a string that contains xsl
            using (StringReader xmlContentReader = new StringReader(xmlInput)) // xmlInput is a string that contains xml
            {

                using (XmlReader xsltReader = XmlReader.Create(xsltStringReader))
                {
                    using (XmlReader xmlReader = XmlReader.Create(xmlContentReader))
                    {
                        var xsltCompiledTransform = new XslCompiledTransform();
                        var settings = new XsltSettings
                        {
                            EnableScript = true
                        };
                        xsltCompiledTransform.Load(xsltReader, settings, new XmlUrlResolver());
                        using (StringWriter stringWriter = new StringWriter())
                        {
                            using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, xsltCompiledTransform.OutputSettings)) //Use OutputSettings of xsl, so it can be output as HTML
                            {
                                xsltCompiledTransform.Transform(xmlReader, null, xmlWriter);
                                output = stringWriter.ToString();
                            }
                        }
                    }
                }
            }

            //string body = $"<h3 style='color:#ec1010;'>{serverName} Last Check In " + LastCheckIn + "</h3>";
            //body += "<hr align ='left'; width='400px'>";

            //body += "<table>";
            //body += "<tr>";
            //body += "<th style='background-color:#04AA6D; color:white; align-items:left; padding:5px;'> File Name </th>";
            //body += "<th style='background-color:#04AA6D; color:white; align-items:left; padding:5px;'> Last Update Date </th>";
            //body += "</tr>";

            //foreach (var file in files)
            //{
            //    DateTime lastUpdateDate = file.LastUpdateDate;

            //    if (lastUpdateDate == DateTime.Now)
            //    {
            //        if (file.Server == serverName)
            //        {
            //            body += "<tr>";
            //            body += $"<td style='align-items:left; padding:5px;'> {file.FileName} </td>";
            //            body += $"<td style='align-items:left; padding:5px;'> {file.LastUpdateDate} </td>";
            //            body += "</tr>";
            //        }
            //        else
            //        {
            //            body += "</table>";
            //            body += "<br>";
            //            serverName = file.Server;
            //            body += $"<h3 style='color: #ec1010;'>{serverName} Last Check In " + LastCheckIn + "</h3>";
            //            body += "<hr align ='left'; width='400px'>";

            //            if (file.FileName == null)
            //            {
            //                continue;
            //            }

            //            body += "<table>";
            //            body += "<tr>";
            //            body += "<th style='background-color:#04AA6D; color:white; align-items:left; padding:5px;'> File Name </th>";
            //            body += "<th style='background-color:#04AA6D; color:white; align-items:left; padding:5px;'> Last Update Date </th>";
            //            body += "</tr>";

            //            body += "<tr>";
            //            body += $"<td style='align-items:left; padding:5px;'> {file.FileName} </td>";
            //            body += $"<td style='align-items:left; padding:5px;'> {file.LastUpdateDate} </td>";
            //            body += "</tr>";

            //        }
            //    }
            //    else
            //    {//last update Date not today
            //        if (file.Server == serverName)
            //        {
            //            body += "<tr style='background-color: #FF5C5C;'>";
            //            body += $"<td style='align-items:left; padding:5px;'> {file.FileName} </td>";
            //            body += $"<td style='align-items:left; padding:5px;'> {file.LastUpdateDate} </td>";
            //            body += "</tr>";
            //        }
            //        else
            //        {
            //            body += "</table>";
            //            body += "<br>";
            //            serverName = file.Server;
            //            body += $"<h3 style='color: #ec1010;'>{serverName} Last Check In " + LastCheckIn + "</h3>";
            //            body += "<hr align ='left'; width='400px'>";

            //            if (file.FileName == null)
            //            {
            //                continue;
            //            }

            //            body += "<table>";
            //            body += "<tr>";
            //            body += "<th style='background-color:#04AA6D; color:white; align-items:left; padding:5px;'> File Name </th>";
            //            body += "<th style='background-color:#04AA6D; color:white; align-items:left; padding:5px;'> Last Update Date </th>";
            //            body += "</tr>";

            //            body += "<tr style='background-color: #FF5C5C;'>";
            //            body += $"<td style='align-items:left; padding:5px;'> {file.FileName} </td>";
            //            body += $"<td style='align-items:left; padding:5px;'> {file.LastUpdateDate} </td>";
            //            body += "</tr>";

            //        }
            //    }
            //}
            //body += "</table>";
            //return body;

            return output;
        }
 
        
        public static void SendEmail(string body, string toAddresses, bool isSuccess)
        {
            if (string.IsNullOrWhiteSpace(toAddresses))
            {
                return;
            }
            MailMessage mailMessage = new MailMessage();
            MailAddress PortalMailAddress = new MailAddress("PaymentSafe DEV <cloud_ops_noreply@eastnets.com>");
            mailMessage.From = PortalMailAddress;
            foreach (var address in toAddresses.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
            {
                mailMessage.To.Add(address);
            }
            string status = isSuccess ? "Done Successfully" : "Failed";
            mailMessage.Subject = $"PaymentSafe - Compress DB Files {status}";
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;
            string username = ConfigurationManager.AppSettings["EmailUsername"];
            string password = ConfigurationManager.AppSettings["EmailPassword"];
            string host = ConfigurationManager.AppSettings["EmailHost"];
            int.TryParse(ConfigurationManager.AppSettings["EmailPort"], out int port);
            SmtpClient client = new SmtpClient(host, port);
            NetworkCredential SMTPUserInfo = new NetworkCredential(username, password);
            client.Credentials = SMTPUserInfo;
            client.EnableSsl = true;
            client.Send(mailMessage);
        }

        public static bool IsFileExist(string fileName)
        {
            FileInfo fi = new FileInfo(fileName);
            return fi.Exists && fi.Length > 0;
        }

        public static string GetXMLFromObject(object obj)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter tw = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                tw = new XmlTextWriter(sw);
                serializer.Serialize(tw, obj);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                sw.Close();
                if (tw != null)
                {
                    tw.Close();
                }
            }
            return sw.ToString();
        }

        public static string RemoveAllNamespaces(string xmlDocument)
        {
            XElement xmlDocumentWithoutNs = RemoveAllNamespaces(XElement.Parse(xmlDocument));

            return xmlDocumentWithoutNs.ToString();
        }

        //Core recursion function
        private static XElement RemoveAllNamespaces(XElement xmlDocument)
        {
            if (!xmlDocument.HasElements)
            {
                XElement xElement = new XElement(xmlDocument.Name.LocalName);
                xElement.Value = xmlDocument.Value;

                foreach (XAttribute attribute in xmlDocument.Attributes())
                    xElement.Add(attribute);

                return xElement;
            }
            return new XElement(xmlDocument.Name.LocalName, xmlDocument.Elements().Select(el => RemoveAllNamespaces(el)));
        }

    }
}