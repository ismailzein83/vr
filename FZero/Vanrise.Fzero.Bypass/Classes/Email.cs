using System;
using System.Data;
using System.Data.SqlClient;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.Bypass
{
    public partial class Email
    {
        public static DataTable GetAllEmails(string DestinationEmail, DateTime? FromDate, DateTime? ToDate)
        {
            DataTable Data = new DataTable();

            try
            {

                using (DbConnection connection = new DbConnection())
                {
                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "prGetEmails";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@DestinationEmail", DestinationEmail);
                    cmd.Parameters.AddWithValue("@FromDate", FromDate);
                    cmd.Parameters.AddWithValue("@ToDate", ToDate);


                    connection.OpenConnection();
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
                    sqlDataAdapter.Fill(Data);
                    connection.CloseConnection();

                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Emails.GetEmails", err);
            }

            return Data;
        }

        public static bool SendMail(Email email, string profile_name)
        {
            bool SentSuccessfully = false;

            try
            {

                using (DbConnection connection = new DbConnection())
                {
                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "SaveMail";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@profile_name", profile_name);
                    cmd.Parameters.AddWithValue("@recipients", email.DestinationEmail);
                    cmd.Parameters.AddWithValue("@subject", email.Subject);
                    cmd.Parameters.AddWithValue("@body", email.Body);
                    cmd.Parameters.AddWithValue("@EmailTemplateId", email.EmailTemplateID);
                    cmd.Parameters.AddWithValue("@copy_recipients", email.CC);

                    cmd.ExecuteNonQuery();
                 

                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Emails.SendMail", err);
            }



            return SentSuccessfully;
        }

        public static bool SendMailWithAttachement(Email email, string AttachmentPath, string profile_name)
        {
            bool SentSuccessfully = false;

            try
            {

                using (DbConnection connection = new DbConnection())
                {
                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "SaveMailWithAttachement";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@profile_name", profile_name);
                    cmd.Parameters.AddWithValue("@recipients", email.DestinationEmail);
                    cmd.Parameters.AddWithValue("@subject", email.Subject);
                    cmd.Parameters.AddWithValue("@body", email.Body);
                    cmd.Parameters.AddWithValue("@EmailTemplateId", email.EmailTemplateID);
                    cmd.Parameters.AddWithValue("@file_attachments", AttachmentPath);
                    cmd.Parameters.AddWithValue("@copy_recipients", email.CC);


                    cmd.ExecuteNonQuery();
                   

                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Emails.SendMailWithAttachement", err);
            }



            return SentSuccessfully;
        }

        public static void DeleteEmail(string Ids)
        {
            try
            {
                using (DbConnection connection = new DbConnection())
                {
                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "prDeleteEmail";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Ids", Ids);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Emails.DeleteEmail", err);
            }

        }
    }
}

    



