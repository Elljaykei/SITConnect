using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect
{
    public partial class UserDetails : System.Web.UI.Page
    {
        string MYDBConnectionstring = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        byte[] Key;
        byte[] IV;
        byte[] creditCard = null;
        string email;
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (Session["UserID"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("Login.aspx", false);
                }
                else
                {
                    email = Session["UserID"].ToString();
                    readUserInfo(email);
                }

            }
            else
            {
                Response.Redirect("Login.aspx", false);
            }

}

        protected void readUserInfo(string email)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionstring);
            string sql = "select * FROM Account WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", email);

            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["firstName"] != DBNull.Value)
                        {
                            fname_lbl.Text = HttpUtility.HtmlEncode(reader["firstName"].ToString());
                        }
                        if (reader["lastName"] != DBNull.Value)
                        {
                            lname_lbl.Text = HttpUtility.HtmlEncode(reader["lastName"].ToString());
                        }
                        if (reader["Email"] != DBNull.Value)
                        {
                            email_lbl.Text = HttpUtility.HtmlEncode(reader["Email"].ToString());
                        }
                        if (reader["DoB"] != DBNull.Value)
                        {
                            dob_lbl.Text = HttpUtility.HtmlEncode(((DateTime)reader["DoB"]).ToString("d"));
                        }
                        if (reader["Photo"] != DBNull.Value)
                        {
                            photo_lbl.Text = HttpUtility.HtmlEncode(reader["Photo"].ToString());
                        }
                        if (reader["creditCard"] != DBNull.Value)
                        {
                            creditCard = Convert.FromBase64String(reader["creditCard"].ToString());
                        }
                        if (reader["IV"] != DBNull.Value)
                        {
                            IV = Convert.FromBase64String(reader["IV"].ToString());
                        }
                        if (reader["Key"] != DBNull.Value)
                        {
                            Key = Convert.FromBase64String(reader["Key"].ToString());
                        }
                    }
                    creditCard_lbl.Text = HttpUtility.HtmlEncode(decryptData(creditCard));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
        }

        protected string decryptData(byte[] cipherText)
        {
            string plainText = null;

            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;

                ICryptoTransform decryptTransform = cipher.CreateDecryptor();

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptTransform, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plainText = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            finally { }

            return plainText;
        }

        protected void Logout(object sender, EventArgs e)
        {
            var email = (string)Session["UserID"];
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            using (SqlConnection con = new SqlConnection(MYDBConnectionstring))
            {
                using (SqlCommand cmd = new SqlCommand("INSERT INTO AuditLog VALUES(@Event, @UserID, @Time,  @IPAddress)"))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        var ipAdd = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                        if (string.IsNullOrEmpty(ipAdd))
                        {
                            ipAdd = Request.ServerVariables["REMOTE_ADDR"];
                        }

                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@Event", "Attempted Log In");
                        cmd.Parameters.AddWithValue("@UserID", email);
                        cmd.Parameters.AddWithValue("@Time", DateTime.Now);
                        cmd.Parameters.AddWithValue("@IPAddress", ipAdd);
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
            }

            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            }

            if (Request.Cookies["AuthToken"] != null)
            {
                Response.Cookies["AuthToken"].Value = string.Empty;
                Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
            }

            Response.Redirect("Login.aspx", false);
        }

        protected bool lockoutCheck(string email)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionstring);
            string sql = "select Event FROM AuditLog WHERE UserID=@USERID AND Time >= @time";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", email);
            command.Parameters.AddWithValue("@time", DateTime.Now.AddMinutes(-1));

            try
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    int failedattempts = 0;

                    while (reader.Read())
                    {
                        if (reader["Event"] != null)
                        {
                            if (reader["Event"] != DBNull.Value)
                            {
                                string activity = (string)reader["Event"];
                                if (activity == "Attempted Log In")
                                {
                                    failedattempts += 1;
                                }
                                else if (activity == "Logged In")
                                {
                                    failedattempts = 0;
                                }
                            }
                        }
                    }
                    if (failedattempts >= 3)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            finally
            {
                connection.Close();
            }
            return false;

        }

        protected void chgPassBtn_Click(object sender, EventArgs e)
        {
            string email = (string)Session["UserID"];
            if (chgPwdCheck(email))
            {
                Response.Redirect("PasswordChange.aspx");
            }
            else
            {
                errorMsg_lbl.Text = "Password can only be changed once every minute";
            }
        }

        protected bool chgPwdCheck(string email)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionstring);
            string sql = "select Event FROM AuditLog WHERE UserID=@USERID AND Time >= @time";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", email);
            command.Parameters.AddWithValue("@time", DateTime.Now.AddMinutes(-1));

            try
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    int failedattempts = 0;

                    while (reader.Read())
                    {
                        if (reader["Event"] != null)
                        {
                            if (reader["Event"] != DBNull.Value)
                            {
                                string activity = (string)reader["Event"];
                                if (activity == "Password Changed")
                                {
                                    return false;
                                }
                            }
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            finally
            {
                connection.Close();
            }
        }
    }
}