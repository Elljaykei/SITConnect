using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect
{
    public partial class Verification : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["registerCode"] == null && Session["loggingIn"] == null)
            {
                Response.Redirect("Login.aspx");
            }
        }

        protected void verBtn_Click(object sender, EventArgs e)
        {
            if (Session["registerCode"] != null)
            {
                if (!(Session["AuthTokenVerification"].ToString() == Request.Cookies["AuthTokenVerification"].Value)) {
                    Response.Redirect("Registration.aspx");
                }
                else
                {
                    string code = Session["registerCode"].ToString();

                    if (code == verCode_tb.Text.Trim())
                    {
                        string email = Session["email"].ToString();
                        using (SqlConnection connection = new SqlConnection(MYDBConnectionString))
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
                                        cmd.Parameters.AddWithValue("@Event", "Account Created");
                                        cmd.Parameters.AddWithValue("@UserID", email);
                                        cmd.Parameters.AddWithValue("@Time", DateTime.Now);
                                        cmd.Parameters.AddWithValue("@IPAddress", ipAdd);
                                        cmd.Connection = connection;
                                        connection.Open();
                                        cmd.ExecuteNonQuery();
                                        connection.Close();
                                    }

                                    catch (Exception ex)
                                    {
                                        throw new Exception(ex.ToString());
                                    }

                                }
                            }

                            try
                            {
                                string sql = "Update Account Set IsVerified=@IsVerified WHERE Email=@Email";
                                SqlCommand command = new SqlCommand(sql, connection);
                                command.Parameters.AddWithValue("@Email", email);
                                command.Parameters.AddWithValue("@IsVerified", true);
                                connection.Open();
                                command.ExecuteNonQuery();
                                connection.Close();
                                Session["email"] = null;
                                Session["registerCode"] = null;

                                Session.Clear();
                                Session.Abandon();
                                Session.RemoveAll();

                                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);

                                Response.Cookies["AuthTokenVerification"].Value = string.Empty;
                                Response.Cookies["AuthTokenVerification"].Expires = DateTime.Now.AddMonths(-20);

                                Response.Redirect("Login.aspx",false);
                            }

                            catch (Exception ex)
                            {
                                throw new Exception(ex.ToString());
                            }
                        }
                    }
                    else
                    {
                        verCode_lbl.Text = "Invalid Verification Code";
                        verCode_lbl.ForeColor = Color.Red;
                    }
                }
            }

            else if (Session["loggingIn"] != null)
            {
                if (!(Session["AuthTokenVerification"].ToString() == Request.Cookies["AuthTokenVerification"].Value))
                {
                    Response.Redirect("Login.aspx", false);
                }
                else
                {
                    string code = Session["loggingIn"].ToString();

                    if (code == verCode_tb.Text.Trim())
                    {
                        string email = (string)Session["UserID"];
                        Session["loggingIn"] = null;

                        using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                        {
                            using (SqlCommand cmd = new SqlCommand("INSERT INTO AuditLog VALUES(@Event, @UserID, @Time, @IPAddress)"))
                            {
                                using (SqlDataAdapter sda = new SqlDataAdapter())
                                {
                                    try {
                                        var ipAdd = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                                        if (string.IsNullOrEmpty(ipAdd))
                                        {
                                            ipAdd = Request.ServerVariables["REMOTE_ADDR"];
                                        }
                                        cmd.CommandType = CommandType.Text;
                                        cmd.Parameters.AddWithValue("@Event", "Logged In");
                                        cmd.Parameters.AddWithValue("@UserID", email);
                                        cmd.Parameters.AddWithValue("@Time", DateTime.Now);
                                        cmd.Parameters.AddWithValue("@IPAddress", ipAdd);
                                        cmd.Connection = con;
                                        con.Open();
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
                        string guid = Guid.NewGuid().ToString();
                        Session["AuthToken"] = guid;
                        Response.Cookies.Add(new HttpCookie("AuthToken", guid));
                        if (chgPwdCheck(email))
                        {
                            Response.Redirect("UserDetails.aspx");
                        }
                        else
                        {
                            Response.Redirect("PasswordChange.aspx");
                        }
                    }
                    else
                    {
                        verCode_lbl.Text = "Invalid Verification Code";
                        verCode_lbl.ForeColor = Color.Red;
                    }
                }
            }
            else
            {
                Response.Redirect("Login.aspx", false);
            }
        }

        protected bool chgPwdCheck(string email)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select Event FROM AuditLog WHERE UserID=@USERID AND Time >= @time";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", email);
            command.Parameters.AddWithValue("@time", DateTime.Now.AddMinutes(-3));

            try
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Event"] != null)
                        {
                            if (reader["Event"] != DBNull.Value)
                            {
                                string activity = (string)reader["Event"];
                                if (activity == "Password Changed" || activity == "Account Created")
                                {
                                    return true;
                                }
                            }
                        }
                    }
                    return false;
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