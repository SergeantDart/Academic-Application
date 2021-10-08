using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web.Services;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using System.Net.Http;
using System.Xml;
using System.IO;

namespace College_Mangement_System
{
    public partial class StudentsWebForm : System.Web.UI.Page
    {
        private static int maxStudentId;
        private static SqlConnection conn;
        private static SqlCommand command;

        protected void Page_Init(object sender, EventArgs e)
        {
            conn = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=CollegeManagementSystemDB;Integrated Security=True;Pooling=False");
            
            try
            {
                conn.Open();

                if(serverRequestAuth() == 0 && Session["userRole"] != "admin")
                {
                    Response.Redirect("LoginWebForm.aspx");
                }
                string getMaxIdCommand = "GetMaxStudentIdProcedure";

                command = new SqlCommand(getMaxIdCommand, conn);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameter maxStudentIdParameter = new SqlParameter("@maxStudentIdParameter", System.Data.SqlDbType.Int);
                maxStudentIdParameter.Direction = System.Data.ParameterDirection.Output;

                command.Parameters.Add(maxStudentIdParameter);
                command.ExecuteNonQuery();

                maxStudentId = int.Parse(maxStudentIdParameter.Value.ToString()) + 1;

                studentIdTextBox.Text = maxStudentId.ToString();
            }catch(Exception ex)
            {
                studentMessageTextBox.Text = ex.Message;
            }   
        }

        protected void Page_UnLoad(object sender, EventArgs e)
        {
            if(conn.State == System.Data.ConnectionState.Open)
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

            } catch (Exception ex)
            {
                return 0;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    //verify authenticity of connection
                    if (serverRequestAuth() == 0 && Session["userRole"] != "admin")
                    {
                        Response.Redirect("LoginWebForm.aspx");
                    }
                }
                catch (Exception ex)
                {
                    studentMessageTextBox.Text += ex.Message + "\n";
                }
            }
        }

        protected void StudentsGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridViewRow clickedRow = StudentsGridView.Rows[Convert.ToInt32(e.CommandArgument)];

            if (e.CommandName == "editStudentButton")
            {

                int studentId = int.Parse(clickedRow.Cells[0].Text);
                string studentName = clickedRow.Cells[1].Text;
                string studentEmail = clickedRow.Cells[2].Text;
                int studyGroupId = int.Parse(clickedRow.Cells[3].Text);

                addStudentButton.Visible = false;
                updateStudentButton.Visible = true;
                cancelStudentButton.Visible = true;

                studentIdTextBox.Text = studentId.ToString();
                studentNameTextBox.Text = studentName;
                studentEmailTextBox.Text = studentEmail;
                studyGroupDropDownList.SelectedValue = studyGroupId.ToString();

                studentIdTextBox.ReadOnly = true;

            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("Do you really want to delete this student ?", "Attention", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    try
                    {
                        command = new SqlCommand("DeleteStudentProcedure", conn);
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        SqlParameter studentIdParameter = new SqlParameter("@studentIdParameter", System.Data.SqlDbType.Int);
                        studentIdParameter.Value = int.Parse(clickedRow.Cells[0].Text.ToString());

                        SqlParameter studentEmailParameter = new SqlParameter("@studentEmailParameter", System.Data.SqlDbType.VarChar, 50);
                        studentEmailParameter.Value = clickedRow.Cells[2].Text.ToString();

                        SqlParameter studentDeletedFailureParameter = new SqlParameter("@studentDeletedFailureParameter", System.Data.SqlDbType.Int);
                        studentDeletedFailureParameter.Direction = System.Data.ParameterDirection.Output;

                        command.Parameters.Add(studentIdParameter);
                        command.Parameters.Add(studentEmailParameter);
                        command.Parameters.Add(studentDeletedFailureParameter);

                        command.ExecuteNonQuery();

                        if(int.Parse(studentDeletedFailureParameter.Value.ToString()) == 0)
                        {
                            studentMessageTextBox.Text = "Student deleted succesfully." + "\n";
                            refreshPage();
                        } else
                        {
                            studentMessageTextBox.Text = "Student deletion failed." + "\n";
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        studentMessageTextBox.Text = ex.Message + "\n";
                    }
                }
            }
        }

        protected void addStudentButton_Click(object sender, EventArgs e)
        {
            try
            {
                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                Match match = regex.Match(studentEmailTextBox.Text);

                if (studentNameTextBox.Text.Length > 5)
                {
                    if (match.Success && studentEmailTextBox.Text.Length > 0)
                    {
                        string callProcedureCommand = "InsertStudentProcedure";

                        command = new SqlCommand(callProcedureCommand, conn);
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        SqlParameter studentIdParameter = new SqlParameter("@studentIdParameter", System.Data.SqlDbType.Int);
                        studentIdParameter.Value = int.Parse(studentIdTextBox.Text);

                        SqlParameter studentNameParameter = new SqlParameter("@studentNameParameter", System.Data.SqlDbType.VarChar, 50);
                        studentNameParameter.Value = studentNameTextBox.Text;

                        SqlParameter studentEmailParameter = new SqlParameter("@studentEmailParameter", System.Data.SqlDbType.VarChar, 50);
                        studentEmailParameter.Value = studentEmailTextBox.Text;

                        SqlParameter studyGroupIdParameter = new SqlParameter("@studyGroupIdParameter", System.Data.SqlDbType.Int);
                        studyGroupIdParameter.Value = int.Parse(studyGroupDropDownList.SelectedValue);

                        SqlParameter insertSuccessParameter = new SqlParameter("@insertSuccessParameter", System.Data.SqlDbType.Int);
                        insertSuccessParameter.Direction = System.Data.ParameterDirection.Output;

                        SqlParameter maxStudentIdParameter = new SqlParameter("@maxStudentIdParameter", System.Data.SqlDbType.Int);
                        maxStudentIdParameter.Direction = System.Data.ParameterDirection.Output;

                        command.Parameters.Add(studentIdParameter);
                        command.Parameters.Add(studentNameParameter);
                        command.Parameters.Add(studentEmailParameter);
                        command.Parameters.Add(studyGroupIdParameter);
                        command.Parameters.Add(insertSuccessParameter);
                        command.Parameters.Add(maxStudentIdParameter);


                        command.ExecuteNonQuery();

                        if (int.Parse(insertSuccessParameter.Value.ToString()) == 1)
                        {
                            studentMessageTextBox.Text = "Student succesfully inserted.\n";
                            maxStudentId = int.Parse(maxStudentIdParameter.Value.ToString()) + 1;
                            refreshPage();
                        }
                        else
                        {
                            studentMessageTextBox.Text = "Failed to insert student...Try again.\n";
                        }

                    }
                    else
                    {
                        studentMessageTextBox.Text = "Invalid name...Try again with a valid name with at least 6 characters.\n";
                    }
                }
                else
                {
                    studentMessageTextBox.Text= "Invalid email...Try again, respecting the email pattern.\n";
                }

            }
            catch (Exception ex)
            {
                studentMessageTextBox.Text = ex.Message + "\n";
            }
        }

        protected void updateStudentButton_Click(object sender, EventArgs e)
        {
            studentMessageTextBox.Text = "";
            try
            {
                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                Match match = regex.Match(studentEmailTextBox.Text);

                if (match.Success && studentEmailTextBox.Text.Length > 0)
                {
                    if (studentNameTextBox.Text.Length > 6)
                    {
                        string callUpdateProcedureCommand = "UpdateStudentProcedure";

                        command = new SqlCommand(callUpdateProcedureCommand, conn);
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        SqlParameter studentIdParameter = new SqlParameter("@studentIdParameter", System.Data.SqlDbType.Int);
                        studentIdParameter.Value = int.Parse(studentIdTextBox.Text);
                        studentIdParameter.Direction = System.Data.ParameterDirection.Input;
                        studentMessageTextBox.Text = studentIdParameter.Value.ToString();

                        SqlParameter studentNameParameter = new SqlParameter("@studentNameParameter", System.Data.SqlDbType.VarChar, 50);
                        studentNameParameter.Value = studentNameTextBox.Text;
                        studentNameParameter.Direction = System.Data.ParameterDirection.Input;

                        SqlParameter studentEmailParameter = new SqlParameter("@studentEmailParameter", System.Data.SqlDbType.VarChar, 50);
                        studentEmailParameter.Value = studentEmailTextBox.Text;
                        studentEmailParameter.Direction = System.Data.ParameterDirection.Input;

                        SqlParameter studyGroupIdParameter = new SqlParameter("@studyGroupIdParameter", System.Data.SqlDbType.Int);
                        studyGroupIdParameter.Value = int.Parse(studyGroupDropDownList.SelectedValue);
                        studyGroupIdParameter.Direction = System.Data.ParameterDirection.Input;

                        command.Parameters.Add(studentIdParameter);
                        command.Parameters.Add(studentNameParameter);
                        command.Parameters.Add(studentEmailParameter);
                        command.Parameters.Add(studyGroupIdParameter);

                        command.ExecuteNonQuery();

                        cancelStudentButton.Visible = false;
                        updateStudentButton.Visible = false;
                        addStudentButton.Visible = true;

                        studentMessageTextBox.Text = "Student succesfully updated.\n";

                        refreshPage();

                    }
                    else
                    {
                        studentMessageTextBox.Text = "Invalid name...Try again, with a name longer than 6 characters.";

                    }
                }
                else
                {
                    studentMessageTextBox.Text = "Invalid email...Try again, respecting the email pattern.";
                }
            }
            catch (Exception ex)
            {
                studentMessageTextBox.Text = ex.Message + "\n";
            }
        }

        protected void cancelStudentButton_Click(object sender, EventArgs e)
        {
            updateStudentButton.Visible = false;
            cancelStudentButton.Visible = false;
            addStudentButton.Visible = true;
            studentEmailTextBox.Text = "";
            studentNameTextBox.Text = "";
            studentIdTextBox.Text = maxStudentId.ToString();
            studyGroupDropDownList.SelectedIndex = -1;
            studentMessageTextBox.Text = "";
        }

        protected void refreshPage()
        {
            studentIdTextBox.Text = maxStudentId.ToString();
            studentNameTextBox.Text = "";
            studentEmailTextBox.Text = "";
            studyGroupDropDownList.SelectedIndex = -1;
            StudentsGridView.DataBind();
        }

        [WebMethod]
        public static List<StudyGroupsHomogenityDetails> GetStudyGroupsHomogenityData()
        {
            try
            {
                string callProcedureString = "GetStudyGroupsHomogenityDataProcedure";
                command = new SqlCommand(callProcedureString, conn);
                command.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter sda = new SqlDataAdapter();
                sda.SelectCommand = command;

                DataTable dt = new DataTable();
                sda.Fill(dt);

                List<StudyGroupsHomogenityDetails> sthdList = new List<StudyGroupsHomogenityDetails>();
                foreach (DataRow row in dt.Rows)
                {
                    StudyGroupsHomogenityDetails stdh = new StudyGroupsHomogenityDetails(row[0].ToString(), int.Parse(row[1].ToString()));
                    sthdList.Add(stdh);
                }
                return sthdList;
            } catch(Exception ex)
            {
                return null;
            }
        }

        protected void xmlButton_Click(object sender, EventArgs e)
        {
            XmlDocument doc = new XmlDocument();

            XmlNode declaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(declaration);

            // Root element: Catalog
            XmlElement root = doc.CreateElement("students");
            doc.AppendChild(root);

            try
            {
                string callProcedureString = "GetAllStudentsProcedure";
                command = new SqlCommand(callProcedureString, conn);
                command.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter studentsSda = new SqlDataAdapter();
                studentsSda.SelectCommand = command;

                DataTable studentsDt = new DataTable();
                studentsSda.Fill(studentsDt);

                foreach (DataRow studentRow in studentsDt.Rows)
                {
                    XmlNode student = doc.CreateElement("student");
                    root.AppendChild(student);

                    XmlNode id = doc.CreateElement("id");
                    id.InnerText = studentRow[0].ToString();
                    student.AppendChild(id);

                    XmlNode name = doc.CreateElement("name");
                    name.InnerText = studentRow[3].ToString();
                    student.AppendChild(name);

                    XmlNode email = doc.CreateElement("email");
                    email.InnerText = studentRow[2].ToString();
                    student.AppendChild(email);

                    XmlNode studyGroup = doc.CreateElement("studyGroup");
                    studyGroup.InnerText = studentRow[4].ToString();
                    student.AppendChild(studyGroup);

                    XmlNode marks = doc.CreateElement("marks");
                    student.AppendChild(marks);

                    callProcedureString = "GetMarksByStudentProcedure";
                    command = new SqlCommand(callProcedureString, conn);
                    command.CommandType = CommandType.StoredProcedure;

                    SqlParameter studentIdParameter = new SqlParameter("@studentIdParameter", SqlDbType.Int);
                    studentIdParameter.Value = int.Parse(studentRow[0].ToString());
                    command.Parameters.Add(studentIdParameter);

                    SqlDataAdapter marksSda = new SqlDataAdapter();
                    marksSda.SelectCommand = command;

                    DataTable marksDt = new DataTable();
                    marksSda.Fill(marksDt);

                    foreach (DataRow markRow in marksDt.Rows)
                    {
                        XmlNode mark = doc.CreateElement("mark");
                        marks.AppendChild(mark);

                        XmlNode subject = doc.CreateElement("subject");
                        subject.InnerText = markRow[0].ToString();
                        mark.AppendChild(subject);

                        XmlNode score = doc.CreateElement("score");
                        score.InnerText = markRow[1].ToString();
                        mark.AppendChild(score);
                    }
                }

                doc.Save("students.xml");
                studentMessageTextBox.Text = @"XML stored in C:\Program Files (x86)\IIS Express";

            }
            catch (Exception ex)
            {
                studentMessageTextBox.Text = ex.Message;
            }
        }
    }
}