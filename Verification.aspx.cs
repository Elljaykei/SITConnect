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
                        using (SqlConnection connection = new SqlConnection(MYDBConnectionString))
                        {
                            try
                            {
                                string email = Session["email"].ToString();
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

                                Response.Redirect("Login.aspx");
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
                    Response.Redirect("Login.aspx");
                }
                else
                {
                    string code = Session["loggingIn"].ToString();

                    if (code == verCode_tb.Text.Trim())
                    {
                        string email = Session["email"].ToString();
                        Session["loggingIn"] = null;

                        Session.Clear();
                        Session.Abandon();
                        Session.RemoveAll();

                        Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                        Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
                        Response.Cookies["AuthTokenVerification"].Value = string.Empty;
                        Response.Cookies["AuthTokenVerification"].Expires = DateTime.Now.AddMonths(-20);

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
                                    cmd.Parameters.AddWithValue("@Event", "Logged In");
                                    cmd.Parameters.AddWithValue("@UserID", email);
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
                                Session["LoggedIn"] = reader;
                                string guid = Guid.NewGuid().ToString();
                                Response.Cookies.Add(new HttpCookie("AuthToken", guid));
                                Response.Redirect("UserDetails.aspx");
                            }
                            con.Close();

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
                Response.Redirect("Login.aspx");
            }
        }
    }
}