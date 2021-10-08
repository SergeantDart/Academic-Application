using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace College_Mangement_System
{
    public partial class GradesWebForm : System.Web.UI.Page
    {
        private static int maxMarkId;
        private static SqlConnection conn;
        private static SqlCommand command;

        protected int serverRequestAuth()
        {
            try
            {
                string serverRequestCommand = "ServerRequestProcedure";

                command = new SqlCommand(serverRequestCommand, conn);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameter sessionStoredUserTokenParameter = new SqlParameter("@sessionStoredUserTokenParameter", System.Data.SqlDbType.VarChar, 50);
                sessionStoredUserTokenParameter.Value = Session["userToken"];

                SqlParameter userEmailParameter = new SqlParameter("@userEmailParameter", System.Data.SqlDbType.VarChar, 50);
                userEmailParameter.Value = Session["userEmail"];

                SqlParameter requestAuthorisedParameter = new SqlParameter("@requestAuthorisedParameter", System.Data.SqlDbType.Int);
                requestAuthorisedParameter.Direction = System.Data.ParameterDirection.Output;

                command.Parameters.Add(sessionStoredUserTokenParameter);
                command.Parameters.Add(userEmailParameter);
                command.Parameters.Add(requestAuthorisedParameter);

                command.ExecuteNonQuery();

                return int.Parse(requestAuthorisedParameter.Value.ToString());

            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            conn = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=CollegeManagementSystemDB;Integrated Security=True;Pooling=False;MultipleActiveResultSets=true");

            try
            {
                conn.Open();
  

                if (serverRequestAuth() == 0 && Session["userRole"] != "admin")
                {
                    Response.Redirect("LoginWebForm.aspx");
                }

                string getMaxIdCommand = "GetMaxMarkIdProcedure";

                command = new SqlCommand(getMaxIdCommand, conn);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameter maxMarkIdParameter = new SqlParameter("@maxMarkIdParameter", System.Data.SqlDbType.Int);
                maxMarkIdParameter.Direction = System.Data.ParameterDirection.Output;

                command.Parameters.Add(maxMarkIdParameter);
                command.ExecuteNonQuery();

                maxMarkId = int.Parse(maxMarkIdParameter.Value.ToString()) + 1;

                markIdTextBox.Text = maxMarkId.ToString();
            }
            catch (Exception ex)
            {
                markMessageTextBox.Text = ex.Message;
            }
        }

        protected void Page_UnLoad(object sender, EventArgs e)
        {
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                try
                {
                    //verify authenticity of connection
                    if (serverRequestAuth() == 0 && Session["userRole"] != "admin")
                    {
                        Response.Redirect("LoginWebForm.aspx");
                    }

                    string commandText = "SELECT StudentId, StudentName FROM Students";
                    command = new SqlCommand(commandText, conn);
                    studentsDropDownList.DataSource = command.ExecuteReader();
                    studentsDropDownList.DataTextField = "StudentName";
                    studentsDropDownList.DataValueField = "StudentId";
                    studentsDropDownList.DataBind();

                    commandText = "SELECT SubjectId, SubjectName FROM Subjects";
                    command = new SqlCommand(commandText, conn);
                    subjectsDropDownList.DataSource = command.ExecuteReader();
                    subjectsDropDownList.DataTextField = "SubjectName";
                    subjectsDropDownList.DataValueField = "SubjectId";
                    subjectsDropDownList.DataBind();
                }
                catch (Exception ex)
                {
                    markMessageTextBox.Text += ex.Message + "\n";
                }
            }
            
        }

        protected void addMarkButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (int.Parse(markScoreTextBox.Text) >= 1 && int.Parse(markScoreTextBox.Text) <= 10)
                {
                    if (markDateTextBox.Text != "")
                    {
                        string commandText = "InsertMarkProcedure";
                        command = new SqlCommand(commandText, conn);
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        SqlParameter studentIdParameter = new SqlParameter("@studentIdParameter", System.Data.SqlDbType.Int);
                        studentIdParameter.Value = int.Parse(studentsDropDownList.SelectedValue);

                        SqlParameter subjectIdParameter = new SqlParameter("@subjectIdParameter", System.Data.SqlDbType.Int);
                        subjectIdParameter.Value = int.Parse(subjectsDropDownList.SelectedValue);

                        SqlParameter markScoreParameter = new SqlParameter("@markScoreParameter", System.Data.SqlDbType.Int);
                        markScoreParameter.Value = int.Parse(markScoreTextBox.Text);

                        SqlParameter markDateParameter = new SqlParameter("@markDateParameter", System.Data.SqlDbType.Date);
                        markDateParameter.Value = DateTime.Parse(markDateTextBox.Text);

                        SqlParameter insertSuccessParameter = new SqlParameter("@insertSuccessParameter", System.Data.SqlDbType.Int);
                        insertSuccessParameter.Direction = System.Data.ParameterDirection.Output;

                        command.Parameters.Add(studentIdParameter);
                        command.Parameters.Add(subjectIdParameter);
                        command.Parameters.Add(markScoreParameter);
                        command.Parameters.Add(markDateParameter);
                        command.Parameters.Add(insertSuccessParameter);

                        command.ExecuteNonQuery();

                        if (int.Parse(insertSuccessParameter.Value.ToString()) == 1)
                        {
                            markMessageTextBox.Text = "Mark inserted succesfully.";
                            maxMarkId++;
                            refreshPage();
                        }
                        else
                        {
                            markMessageTextBox.Text = "Failed to insert mark.";
                        }

                    }
                    else
                    {
                        markMessageTextBox.Text = "Enter a valid date.";
                    }
                }
                else
                {
                    markMessageTextBox.Text = "Enter a valid mark between 1 and 10.";
                }
            } catch(Exception ex)
            {
                markMessageTextBox.Text = ex.Message;
            }

        }

        protected void refreshPage()
        {
            markIdTextBox.Text = maxMarkId.ToString();
            studentsDropDownList.SelectedIndex = -1;
            subjectsDropDownList.SelectedIndex = -1;
            markScoreTextBox.Text = "";
            markDateTextBox.Text = "";
            MarksGridView.DataBind();
        }

        [WebMethod]
        public static List<YearlyFlunkingsNo> GetYearlyFlunkings()
        {
            try
            {
                string callProcedureString = "GetYearlyFlunkingsProcedure";
                command = new SqlCommand(callProcedureString, conn);
                command.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter sda = new SqlDataAdapter();
                sda.SelectCommand = command;

                DataTable dt = new DataTable();
                sda.Fill(dt);

                List<YearlyFlunkingsNo> yfnList = new List<YearlyFlunkingsNo>();
                foreach (DataRow row in dt.Rows)
                {
                    YearlyFlunkingsNo yfn = new YearlyFlunkingsNo(row[0].ToString(), int.Parse(row[1].ToString()));
                    yfnList.Add(yfn);
                }
                return yfnList;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
    }
}