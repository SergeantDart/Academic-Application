using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace College_Mangement_System
{
    public partial class SubjectsWebForm : System.Web.UI.Page
    {
        private static SqlConnection conn;
        private static SqlCommand command;
        private static int maxSubjectId;

        protected void Page_Init(object sender, EventArgs e)
        {
            conn = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=CollegeManagementSystemDB;Integrated Security=True;Pooling=False");

            try
            {
                conn.Open();

                if (serverRequestAuth() == 0 && Session["userRole"] != "admin")
                {
                    Response.Redirect("LoginWebForm.aspx");
                }

                string callProcedureText = "GetMaxSubjectIdProcedure";
                command = new SqlCommand(callProcedureText, conn);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameter maxSubjectIdParameter = new SqlParameter("@maxSubjectIdParameter", System.Data.SqlDbType.Int);
                maxSubjectIdParameter.Direction = System.Data.ParameterDirection.Output;

                command.Parameters.Add(maxSubjectIdParameter);

                command.ExecuteNonQuery();

                maxSubjectId = int.Parse(maxSubjectIdParameter.Value.ToString()) + 1;
                subjectIdTextBox.Text = maxSubjectId.ToString();

            }
            catch (Exception ex)
            {
                subjectMessageTextBox.Text = ex.Message;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                try
                {
                    if (serverRequestAuth() == 0 && Session["userRole"] != "admin")
                    {
                        Response.Redirect("LoginWebForm.aspx");
                    }
                }
                catch (Exception ex)
                {
                    subjectMessageTextBox.Text = ex.Message;
                }
            }

        }

        protected void Page_UnLoad(object sender, EventArgs e)
        {
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
        }

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

        protected void addSubjectButton_Click(object sender, EventArgs e)
        {
            try
            {
                if(subjectNameTextBox.Text.Length < 25)
                {
                    if(int.Parse(subjectCreditPointsTextBox.Text) >= 2 || int.Parse(subjectCreditPointsTextBox.Text) <= 15)
                    {
                        string callProcedureString = "InsertSubjectProcedure";
                        command = new SqlCommand(callProcedureString, conn);
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        SqlParameter subjectName = new SqlParameter("@subjectName", System.Data.SqlDbType.VarChar, 50);
                        subjectName.Value = subjectNameTextBox.Text;

                        SqlParameter subjectCreditPoints = new SqlParameter("@subjectCreditPoints", System.Data.SqlDbType.Int);
                        subjectCreditPoints.Value = int.Parse(subjectCreditPointsTextBox.Text);

                        SqlParameter subjectDescription = new SqlParameter("@subjectDescription", System.Data.SqlDbType.VarChar, 50);
                        subjectDescription.Value = subjectDescriptionTextBox.Text;

                        SqlParameter insertSuccessParameter = new SqlParameter("@insertSuccessParameter", System.Data.SqlDbType.Int);
                        insertSuccessParameter.Direction = System.Data.ParameterDirection.Output;


                        command.Parameters.Add(subjectName);
                        command.Parameters.Add(subjectCreditPoints);
                        command.Parameters.Add(subjectDescription);
                        command.Parameters.Add(insertSuccessParameter);

                        command.ExecuteNonQuery();

                        if (int.Parse(insertSuccessParameter.Value.ToString()) == 1)
                        {
                            subjectMessageTextBox.Text = "Subject inserted succesfully.";
                            maxSubjectId++;
                            refreshPage();
                        }
                        else
                        {
                            subjectMessageTextBox.Text = "Failed to insert subject.";
                        }
                    } else
                    {
                        subjectMessageTextBox.Text = "Subject credit points are out of range. Must be between 2 and 15.";
                    }

                } else
                {
                    subjectMessageTextBox.Text = "Subject name is too long. Must be shorter than 25.";
                }
                command.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                subjectMessageTextBox.Text = ex.Message;
            }
        }

        protected void refreshPage()
        {
            subjectIdTextBox.Text = maxSubjectId.ToString();
            subjectNameTextBox.Text = "";
            subjectCreditPointsTextBox.Text = "";
            subjectDescriptionTextBox.Text = "";
            SubjectsGridView.DataBind();
        }

        protected void searchSubjectButton_Click(object sender, EventArgs e)
        {
            try
            {
                string callSearchProcedureText = "SearchSubjectProcedure";
                command = new SqlCommand(callSearchProcedureText, conn);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameter keywordParameter = new SqlParameter("@keywordParameter", System.Data.SqlDbType.VarChar, 50);
                keywordParameter.Value = subjectSearchTextBox.Text;

                command.Parameters.Add(keywordParameter);

                SqlDataAdapter sda = new SqlDataAdapter();
                sda.SelectCommand = command;

                DataTable dt = new DataTable();
                sda.Fill(dt);
                SubjectsGridView.DataSource = dt;
                SubjectsGridView.DataSourceID = null;

            }
            catch (Exception ex)
            {
                subjectMessageTextBox.Text = ex.Message;
            }
        }

        protected void getAllSubjectsButton_Click(object sender, EventArgs e)
        {
            SubjectsGridView.DataSourceID = "SubjectsSqlDataSource";
            SubjectsGridView.DataSource = null;
        }

        [WebMethod]
        public static List<SubjectAverageDetails> GetEachSubjectAvg
()
        {
            try
            {
                string callProcedureString = "GetEachSubjectAvgDataProcedure";
                command = new SqlCommand(callProcedureString, conn);
                command.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter sda = new SqlDataAdapter();
                sda.SelectCommand = command;

                DataTable dt = new DataTable();
                sda.Fill(dt);

                List<SubjectAverageDetails> sadList = new List<SubjectAverageDetails>();
                foreach (DataRow row in dt.Rows)
                {
                    SubjectAverageDetails sad = new SubjectAverageDetails(row[0].ToString(), double.Parse(row[1].ToString()));
                    sadList.Add(sad);
                }
                return sadList;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
    }
}