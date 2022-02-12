using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Net.Mail;
using System.Net;

namespace SITConnect
{
    public partial class Registration : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void registerBtn_Click(object sender, EventArgs e)
        {
            int score = checkPassword(password_tb.Text.Trim());
            string strength = pwdStr(score);

            var validReg = true;
            var errorMsg = "";

            if (string.IsNullOrEmpty(fName_tb.Text.Trim()))
            {
                errorMsg += "*First Name is required<br>";
                validReg = false;
            }
            
            if (string.IsNullOrEmpty(lName_tb.Text.Trim()))
            {
                errorMsg += "*Last Name is required<br>";
                validReg = false;
            }

            if (string.IsNullOrEmpty(creditCard_tb.Text) || !Regex.IsMatch(creditCard_tb.Text, "[0-9]") || creditCard_tb.Text.Length != 16)
            {
                errorMsg += "*Credit Card Info must have 16 digits<br>";
                validReg = false;
            }

            if (strength != "Excellent")
            {
                errorMsg += "*Password not strong enough<br>";
                validReg = false;
            }

            if (validReg) {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    con.Open();
                    //check if email exists
                    SqlCommand check_email = new SqlCommand("SELECT * FROM Account WHERE Email = @Email", con);
                    check_email.Parameters.AddWithValue("@Email", emailAddress_tb.Text.Trim());
                    SqlDataReader reader = check_email.ExecuteReader();
                    if (reader.HasRows)
                    {
                        errorMsg += "Account with this email already exists <br>";
                        validReg = false;
                    }
                    con.Close();

                }
            }
            

            if (validReg)
            {
                if (verificationEmail(emailAddress_tb.Text.Trim()))
                {
                    Session["email"] = emailAddress_tb.Text.Trim();
                    string guid = Guid.NewGuid().ToString();
                    Session["AuthTokenVerification"] = guid;
                    Response.Cookies.Add(new HttpCookie("AuthTokenVerification", guid));
                    saltHashPass();
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
            pwdChecker_lbl.Text = "Strength: " + status;
            if (score < 4)
            {
                pwdChecker_lbl.ForeColor = Color.Red;
                return status;
            }
            pwdChecker_lbl.ForeColor = Color.Green;
            return status;
        }

        private void saltHashPass()
        {
            string pwd = password_tb.Text.ToString().Trim(); ;
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] saltByte = new byte[8];

            rng.GetBytes(saltByte);
            salt = Convert.ToBase64String(saltByte);

            SHA512Managed hashing = new SHA512Managed();

            string pwdWithSalt = pwd + salt;
            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));

            finalHash = Convert.ToBase64String(hashWithSalt);

            RijndaelManaged cipher = new RijndaelManaged();
            cipher.GenerateKey();
            Key = cipher.Key;
            IV = cipher.IV;
            createAccount();
        }

        protected void createAccount()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Account VALUES(@firstName, @lastName, @creditCard, @Email, @PasswordHash, @PasswordSalt, @DoB, @Photo, @IV, @Key, @IsVerified, @PasswordHistory)"))
                {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@firstName", fName_tb.Text.Trim());
                            cmd.Parameters.AddWithValue("@lastName", lName_tb.Text.Trim());
                            cmd.Parameters.AddWithValue("@creditCard", Convert.ToBase64String(encryptData(creditCard_tb.Text.Trim())));
                            cmd.Parameters.AddWithValue("@Email", emailAddress_tb.Text.Trim());
                            cmd.Parameters.AddWithValue("@PasswordHash", finalHash);
                            cmd.Parameters.AddWithValue("@PasswordSalt", salt);
                            cmd.Parameters.AddWithValue("@DoB", DateTime.Parse(dob_tb.Text.Trim()));
                            cmd.Parameters.AddWithValue("@Photo", photo_tb.Text.Trim());
                            cmd.Parameters.AddWithValue("@IV",Convert.ToBase64String(IV));
                            cmd.Parameters.AddWithValue("@Key", Convert.ToBase64String(Key));
                            cmd.Parameters.AddWithValue("@IsVerified", 0);
                            cmd.Parameters.AddWithValue("@PasswordHistory", "");
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }


        protected byte[] encryptData(string data)
        {
            byte[] cipherText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0, plainText.Length);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            finally { }
            return cipherText;
        }

        protected bool verificationEmail(string email)
        {
            string mailAccount = System.Configuration.ConfigurationManager.ConnectionStrings["mailAccount"].ConnectionString;
            string mailPassword = System.Configuration.ConfigurationManager.ConnectionStrings["mailPassword"].ConnectionString;
            try
            {
                Random random = new Random();
                int code = random.Next(000000, 1000000);

                Session["registerCode"] = code;

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

    }
}