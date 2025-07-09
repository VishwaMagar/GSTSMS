using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace GSTSMSHelper
{
    public class MSSQL
    {
        static string connectionString = ConfigurationManager.ConnectionStrings["GSTSocietyManagementSystem"].ConnectionString;

        // Execute Stored Procedure and Return DataSet
        public async Task<DataSet> ExecuteStoreProcedureReturnDS(string SPName, Dictionary<string, string> InPara)
        {
            DataSet ds = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(SPName, con)
                    {
                        CommandTimeout = 60,
                        CommandType = CommandType.StoredProcedure
                    };

                    foreach (KeyValuePair<string, string> para in InPara)
                    {
                        cmd.Parameters.AddWithValue(para.Key, para.Value);
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    await Task.Run(() => da.Fill(ds));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }

        // Execute Stored Procedure and Return SqlDataReader
        public async Task<SqlDataReader> ExecuteStoreProcedureReturnDataReader(string SPName, Dictionary<string, string> InPara)
        {
            SqlDataReader dr = null;
            try
            {
                SqlConnection con = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand(SPName, con)
                {
                    CommandTimeout = 60,
                    CommandType = CommandType.StoredProcedure
                };

                foreach (KeyValuePair<string, string> para in InPara)
                {
                    cmd.Parameters.AddWithValue(para.Key, para.Value);
                }

                await con.OpenAsync();
                dr = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dr;
        }

        // Execute Stored Procedure and Return DataTable
        public async Task<DataTable> ExecuteStoreProcedureReturnDataTable(string SPName, Dictionary<string, string> InPara)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(SPName, con)
                    {
                        CommandTimeout = 60,
                        CommandType = CommandType.StoredProcedure
                    };

                    foreach (KeyValuePair<string, string> para in InPara)
                    {
                        cmd.Parameters.AddWithValue(para.Key, para.Value);
                    }

                    await con.OpenAsync();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        // Execute Stored Procedure and Return Object
        public async Task<object> ExecuteStoreProcedureReturnObj(string SPName, Dictionary<string, string> InPara)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(SPName, con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                foreach (KeyValuePair<string, string> para in InPara)
                {
                    cmd.Parameters.AddWithValue(para.Key, para.Value);
                }

                await con.OpenAsync();
                object Obj = await cmd.ExecuteScalarAsync();
                con.Close();
                return Obj;
            }
        }

        // Execute Stored Procedure without return
        public async Task ExecuteStoreProcedure(string SPName, Dictionary<string, string> InPara)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(SPName, con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    foreach (KeyValuePair<string, string> para in InPara)
                    {
                        cmd.Parameters.AddWithValue(para.Key, para.Value);
                    }

                    await con.OpenAsync();
                    await cmd.ExecuteScalarAsync();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<int> ExecuteStoreProcedureReturnInt(string SPName, Dictionary<string, string> InPara)
        {
            int result = 0;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(SPName, con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                foreach (KeyValuePair<string, string> para in InPara)
                {
                    cmd.Parameters.AddWithValue(para.Key, para.Value);
                }

                await con.OpenAsync();
                result = await cmd.ExecuteNonQueryAsync();
            }
            return result;
        }

        // Execute Raw SQL Query and Return DataSet
        public DataSet ExecuteQueryReturnDs(string sqlQuery, Dictionary<string, string> parameters)
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(sqlQuery, con)
                {
                    CommandTimeout = 60,
                    CommandType = CommandType.Text
                };

                foreach (KeyValuePair<string, string> para in parameters)
                {
                    cmd.Parameters.AddWithValue(para.Key, para.Value);
                }

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(ds);
                }
            }
            return ds;
        }

        // Send email (with optional attachment)
        public void SendMailPB(string SenderID, string Subject, string Msg, HttpPostedFileBase FileUploader)
        {
            string senderEmail = "hreducationerp@gmail.com";
            string senderPassword = "zapg chik ymwk biza";
            string smtpHost = "smtp.gmail.com";
            int smtpPort = 587;

            MailMessage mailMessage = new MailMessage(senderEmail, SenderID, Subject, Msg)
            {
                IsBodyHtml = true
            };

            if (FileUploader != null)
            {
                string fileName = Path.GetFileName(FileUploader.FileName);
                mailMessage.Attachments.Add(new Attachment(FileUploader.InputStream, fileName));
            }

            SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort)
            {
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true
            };

            smtpClient.Send(mailMessage);
        }
        public async Task ExecuteQuery(string tableNameOrProcedureName, Dictionary<string, string> parameters)
        {
            // This is where you would put your database interaction logic.
            // For example, using SqlConnection, SqlCommand, etc.
            // Example using System.Data.SqlClient (you'd need to add a reference if not already there)
            string connectionString = "Your_Connection_String_Here"; // Get this from a config file or securely.

            using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(tableNameOrProcedureName, connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure; // Or CommandType.Text for direct queries

                    foreach (var param in parameters)
                    {
                        command.Parameters.AddWithValue(param.Key, param.Value);
                    }

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync(); // Use ExecuteNonQueryAsync for non-query operations
                }
            }
        }



        public static async Task<DataTable> ExecuteStoredProcedure( string procedureName, SqlParameter[] parameters)
        {

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(procedureName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(parameters);

                await conn.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    return dt;
                }
            }
        }




    }
}

