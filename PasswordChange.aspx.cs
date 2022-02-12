using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect
{
    public partial class PasswordChange : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("Login.aspx", false);
                }
            }
            else
            {
                Response.Redirect("Login.aspx", false);
            }
        }

        protected void SaveBtn_Click(object sender, EventArgs e)
        {
            string email = (string)Session["UserID"];

            string oldPassword = password_tb1.Text.Trim();
            string newPassword = password_tb2.Text.Trim();
            string cfmPassword = password_tb3.Text.Trim();

            int score = checkPassword(password_tb2.Text.Trim());
            string strength = pwdStr(score);

            var validReg = true;
            var errorMsg = "";

            if (string.IsNullOrEmpty(oldPassword))
            {
                errorMsg += "*Old password is required<br>";
                validReg = false;
            }

            if (string.IsNullOrEmpty(cfmPassword))
            {
                errorMsg += "*Confirm password is required<br>";
                validReg = false;
            }

            if (!(cfmPassword == newPassword))
            {
                errorMsg += "*New password and Confirm password are not the same.<br>";
                validReg = false;
            }

            if (strength != "Excellent")
            {
                errorMsg += "*Password not strong enough<br>";
                validReg = false;
            }

            if (validReg)
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {

                    con.Open();
                    SqlCommand check_password = new SqlCommand("SELECT PasswordHash, PasswordHistory FROM Account WHERE Email = @Email", con);
                    check_password.Parameters.AddWithValue("@Email", email);
                    SqlDataReader reader = check_password.ExecuteReader();
                    if (reader.HasRows)
                    {
                        SHA512Managed hashing = new SHA512Managed();
                        List<String> dbHash = getDBHash(email);
                        string dbSalt = getDBSalt(email);
                        try
                        {
                            if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Count > 0)
                            {
                                string pwdWithSalt = oldPassword + dbSalt;
                                byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                                string userHash = Convert.ToBase64String(hashWithSalt);

                                string newPwdWithSalt = newPassword + dbSalt;
                                byte[] newHashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(newPwdWithSalt));
                                string newUserHash = Convert.ToBase64String(newHashWithSalt);
                                if (newUserHash.Equals(dbHash[0]) || newUserHash.Equals(dbHash[1]) || !userHash.Equals(dbHash[0]))
                                {
                                    errorMsg += "Cannot reuse last two passwords and Old Password must match previous password";
                                    validReg = false;
                                }
                                else
                                {
                                    try
                                    {
                                        string sql = "Update Account Set PasswordHistory = @PasswordHash, PasswordHash = @newPassword WHERE Email=@Email";
                                        SqlCommand command = new SqlCommand(sql, con);
                                        command.Parameters.AddWithValue("@Email", email);
                                        command.Parameters.AddWithValue("@PasswordHash", userHash);
                                        command.Parameters.AddWithValue("@newPassword", newUserHash);
                                        con.Close();
                                        con.Open();
                                        command.Connection = con;
                                        command.ExecuteNonQuery();

                                        con.Close();

                                        Audit();


                                        Response.Redirect("UserDetails.aspx", false);
                                    }

                                    catch (Exception ex)
                                    {
                                        throw new Exception(ex.ToString());
                                    }
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.ToString());
                        }
                        finally { }
                        con.Close();
                    }
                    else
                    {
                        Response.Redirect("Login.aspx", false);
                        return;
                    }
                }
            }

            if (!validReg)
            {
                error_msg_lbl.Text = errorMsg;
            }
        }

        protected List<String> getDBHash(string userid)
        {
            List<String>passwords = new List<String>();
            string original = "";
            string history = "";
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PasswordHash, PasswordHistory FROM Account WHERE Email=@USERID";
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
                                original = reader["PasswordHash"].ToString();
                            }
                        }
                        
                        if (reader["PasswordHistory"] != null)
                        {
                            if (reader["PasswordHistory"] != DBNull.Value)
                            {
                                history = reader["PasswordHistory"].ToString();
                            }
                        }
                    }

                    passwords.Add(original);
                    passwords.Add(history);

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return passwords;
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

        private int checkPassword(string password)
        {
            int score = 0;

            if (password.Length < 12)
            {
                return 1;
            }
            else
            {
                score = 1;
            }

            if (Regex.IsMatch(password, "[a-z]"))
            {
                score++;
            }

            if (Regex.IsMatch(password, "[A-Z]"))
            {
                score++;
            }

            if (Regex.IsMatch(password, "[0-9]"))
            {
                score++;
            }

            if (Regex.IsMatch(password, "[^a-zA-Z0-9]"))
            {
                score++;
            }

            return score;
        }

        private string pwdStr(int score)
        {
            string status = "";

            switch (score)
            {
                case 1:
                    status = "Very Weak";
                    break;
                case 2:
                    status = "Weak";
                    break;
                case 3:
                    status = "Medium";
                    break;
                case 4:
                    status = "Strong";
                    break;
                case 5:
                    status = "Excellent";
                    break;
                default:
                    break;
            }
            pwdChecker_lbl2.Text = "Strength: " + status;
            if (score < 4)
            {
                pwdChecker_lbl2.ForeColor = Color.Red;
                return status;
            }
            pwdChecker_lbl2.ForeColor = Color.Green;
            return status;
        }

        public void Audit()
        {
            using (SqlConnection con = new SqlConnection(MYDBConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("INSERT INTO AuditLog VALUES(@Event, @UserID, @Time, @IPAddress)"))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        try
                        {
                            var ipAdd = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                            if (string.IsNullOrEmpty(ipAdd))
                            {
                                ipAdd = Request.ServerVariables["REMOTE_ADDR"];
                            }
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Event", "Password Changed");
                            cmd.Parameters.AddWithValue("@UserID", (string)Session["UserID"]);
                            cmd.Parameters.AddWithValue("@Time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@IPAddress", ipAdd);

                            con.Open();
                            cmd.Connection = con;
                            cmd.ExecuteNonQuery();
                            con.Close();

                        }

                        catch (Exception ex)
                        {
                            throw new Exception(ex.ToString());
                        }

                    }
                }
            }
        }
    }
}