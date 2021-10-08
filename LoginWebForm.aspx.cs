using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace College_Mangement_System
{
    public partial class LoginWebForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void loginButton_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=CollegeManagementSystemDB;Integrated Security=True;Pooling=False");
            messageTextBox.Text = "";
            try
            {
                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                Match match = regex.Match(loginEmailTextBox.Text);

                if (match.Success && loginEmailTextBox.Text.Length > 0)
                {
                    if(loginPasswordTextBox.Text.Length > 5)
                    {
                        conn.Open();
                        string callProcedureCommand = "LoginProcedure";

                        SqlCommand command = new SqlCommand(callProcedureCommand, conn);
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        SqlParameter userIdParameter = new SqlParameter("@userIdParameter", System.Data.SqlDbType.Int);
                        userIdParameter.Direction = System.Data.ParameterDirection.Output;

                        SqlParameter userEmailParameter = new SqlParameter("@userEmailParameter", System.Data.SqlDbType.VarChar, 50);
                        userEmailParameter.Value = loginEmailTextBox.Text;

                        SqlParameter userPasswordParameter = new SqlParameter("@userPasswordParameter", System.Data.SqlDbType.VarChar, 50);
                        userPasswordParameter.Value = loginPasswordTextBox.Text;

                        SqlParameter userExistsParameter = new SqlParameter("@userExistsParameter", System.Data.SqlDbType.Int);
                        userExistsParameter.Direction = System.Data.ParameterDirection.Output;

                        SqlParameter userRoleParameter = new SqlParameter("@userRoleParameter", System.Data.SqlDbType.VarChar, 50);
                        userRoleParameter.Direction = System.Data.ParameterDirection.Output;

                        SqlParameter userTokenParameter = new SqlParameter("@userTokenParameter", System.Data.SqlDbType.VarChar, 50);
                        userTokenParameter.Direction = System.Data.ParameterDirection.Output;

                        command.Parameters.Add(userEmailParameter);
                        command.Parameters.Add(userPasswordParameter);
                        command.Parameters.Add(userExistsParameter);
                        command.Parameters.Add(userRoleParameter);
                        command.Parameters.Add(userTokenParameter);
                        command.Parameters.Add(userIdParameter);

                        command.ExecuteNonQuery();

                        if (int.Parse(userExistsParameter.Value.ToString()) == 1)
                        {
                            Session["userToken"] = userTokenParameter.Value.ToString();
                            Session["userEmail"] = userEmailParameter.Value.ToString();
                            Session["userRole"] = userRoleParameter.Value.ToString();

                            messageTextBox.Text = userRoleParameter.Value.ToString();
                            if(userRoleParameter.Value.ToString() == "admin")
                            {
                                Response.Redirect("StudentsWebForm.aspx");
                            }

                            if(userRoleParameter.Value.ToString() == "student")
                            {
                                Response.Redirect("StudentDashboardWebForm.aspx");
                            }
                        } else
                        {
                            messageTextBox.Text = "Invalid match...Try again with the right credentials.";
                        }
                    } else
                    {
                        messageTextBox.Text = "Invalid password...Try again with a valid password.";
                    }
                } else
                {
                    messageTextBox.Text = "Invalid email...Try again, respecting the email pattern.";
                }
                


            }
            catch (Exception ex)
            {
                messageTextBox.Text = ex.Message;

            }
            finally
            {
                conn.Close();
            }
        }
    }
}