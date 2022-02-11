using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect
{
    public partial class Login : System.Web.UI.Page
    {
        public class MyObject
        {
            public string success { get; set; }
            public List<String> ErrorMessage { get; set; }
        }

        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void loginBtn_Click(object sender, EventArgs e)
        {
            if (ValidateCaptcha())
            {
                string pwd = password_tb.Text.Trim();
                string email = emailAddress_tb.Text.Trim();

                if(lockoutCheck(email)) {
                    error_msg_lbl.Text = "You have been locked out.";
                    return;
                }
                else
                {
                    var validReg = true;
                    var errorMsg = "";

                    if (string.IsNullOrEmpty(email))
                    {
                        errorMsg += "*Email Address is required<br>";
                        validReg = false;
                    }

                    if (string.IsNullOrEmpty(pwd))
                    {
                        errorMsg += "*Password is required<br>";
                        validReg = false;
                    }

                    if (validReg)
                    {
                        using (SqlConnection con = new SqlConnection(MYDBConnectionString))
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
                                    cmd.Parameters.AddWithValue("@UserID", emailAddress_tb.Text.Trim());
                                    cmd.Parameters.AddWithValue("@Time", DateTime.Now);
                                    cmd.Parameters.AddWithValue("@IPAddress", ipAdd);
                                    cmd.Connection = con;
                                    con.Open();
                                    cmd.ExecuteNonQuery();
                                    con.Close();

                                }
                            }
                            con.Open();
                            //check if email exists
                            SqlCommand check_email = new SqlCommand("SELECT * FROM Account WHERE Email = @Email", con);
                            check_email.Parameters.AddWithValue("@Email", email);
                            SqlDataReader reader = check_email.ExecuteReader();
                            if (reader.HasRows)
                            {
                                SHA512Managed hashing = new SHA512Managed();
                                string dbHash = getDBHash(email);
                                string dbSalt = getDBSalt(email);
                                try
                                {
                                    if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                                    {
                                        string pwdWithSalt = pwd + dbSalt;
                                        byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                                        string userHash = Convert.ToBase64String(hashWithSalt);
                                        if (!userHash.Equals(dbHash))
                                        {
                                            errorMsg += "Email or password is not valid. Please try again.";
                                            validReg = false;
                                            return;
                                        }
                                        Session["UserID"] = email;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(ex.ToString());
                                }
                                finally { }
                            }
                            else
                            {
                                error_msg_lbl.Text = "Email or password is not valid. Please try again.";
                                return;
                            }
                            con.Close();

                        }
                    }


                    if (validReg)
                    {
                        if (verificationEmail(email))
                        {
                            Session["email"] = email;
                            string guid = Guid.NewGuid().ToString();
                            Session["AuthTokenVerification"] = guid;
                            Response.Cookies.Add(new HttpCookie("AuthTokenVerification", guid));
                            Response.Redirect("Verification.aspx");
                        }
                        else
                        {
                            error_msg_lbl.Text = "*Email does not exist.";
                        }

                    }
                    else
                    {
                        error_msg_lbl.Text = errorMsg;
                    }
                }

                
            }//if captcha
            else
            {
                error_msg_lbl.Text = "Validate captcha to prove that you are a human.";
            }

        }

        private bool verificationEmail(string email)
        {
            string mailAccount = System.Configuration.ConfigurationManager.ConnectionStrings["mailAccount"].ConnectionString;
            string mailPassword = System.Configuration.ConfigurationManager.ConnectionStrings["mailPassword"].ConnectionString;
            try
            {
                Random random = new Random();
                int code = random.Next(000000, 1000000);

                Session["loggingIn"] = code;

                MailMessage msg = new MailMessage();
                msg.To.Add(new MailAddress(email, "Request for Verification"));
                msg.From = new MailAddress(mailAccount);
                msg.Body = "Your verification code is: " + code;
                msg.IsBodyHtml = true;
                msg.Subject = "Verification";
                SmtpClient smcl = new SmtpClient();
                smcl.Host = "smtp.gmail.com";
                smcl.Port = 587;
                smcl.Credentials = new NetworkCredential(mailAccount, mailPassword);
                smcl.EnableSsl = true;
                smcl.Send(msg);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public bool ValidateCaptcha()
        {
            bool result = true;
            string captchaResponse = Request.Form["g-recaptcha-response"];
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create
           (" https://www.google.com/recaptcha/api/siteverify?secret=6LcXaHAeAAAAAOY_WEnCVIwJ7Xv193GXVszumHZz &response=" + captchaResponse);

            try
            {
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        string jsonResponse = readStream.ReadToEnd();
                        //lbl_gScore.Text = jsonResponse.ToString();
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        MyObject jsonObject = js.Deserialize<MyObject>(jsonResponse);
                        result = Convert.ToBoolean(jsonObject.success);//

                    }
                }

                return result;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        protected string getDBHash(string userid)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PasswordHash FROM Account WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["PasswordHash"] != null)
                        {
                            if (reader["PasswordHash"] != DBNull.Value)
                            {
                                h = reader["PasswordHash"].ToString();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return h;
        }

        protected string getDBSalt(string userid)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PASSWORDSALT FROM ACCOUNT WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PASSWORDSALT"] != null)
                        {
                            if (reader["PASSWORDSALT"] != DBNull.Value)
                            {
                                s = reader["PASSWORDSALT"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return s;
        }

        protected bool lockoutCheck(string email)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
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
    }
}