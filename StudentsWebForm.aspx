<%@ Page Title="" Language="C#" MasterPageFile="~/AdminSite.Master" AutoEventWireup="true" CodeBehind="StudentsWebForm.aspx.cs" Inherits="College_Mangement_System.StudentsWebForm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="AdminHead" runat="server">
    <script src="https://code.jquery.com/jquery-1.11.0.min.js"></script>
    <script src="https://www.gstatic.com/charts/loader.js"></script>
    <script type="text/javascript">    
        google.load('visualization', '1', {
            packages: ['corechart'],
            callback: loadedResources
        });


        function loadedResources() {
            $(function () {
                $.ajax(
                    {
                        type: "POST",
                        dataType: "json",
                        contentType: "application/json",
                        url: "StudentsWebForm.aspx/GetStudyGroupsHomogenityData",
                        data: "{}",
                        error: function (requestObject, error, errorThrown) {
                            Console.Log(error);
                        }
                    }).done(function (response) {
                        drawChart(response.d);
                    });
            })

            function drawChart(values) {
                var data = new google.visualization.DataTable();
                data.addColumn("string", "StudyGroupId");
                data.addColumn("number", "StudentsNo");

                for (var i = 0; i < values.length; i++) {
                    data.addRow([values[i].studyGroupId, values[i].studentsNo]);
                }

                var chart = new google.visualization.PieChart(document.getElementById("studentsChart"));

                chart.draw(data,
                    {
                        title: "Study groups homogenity",
                        position: "top",
                        fontsize: "14px",
                        chartArea: {
                            width: '50%'
                        },
                    });
            }
        }
        
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="AdminContentPlaceHolder" runat="server">
    <asp:Label ID="Label12" runat="server" Font-Italic="True" Font-Overline="True" Font-Size="XX-Large" Font-Strikeout="False" Font-Underline="True" ForeColor="#FF9900" Text="___________________STUDENTS_________________"></asp:Label>
    <asp:GridView ID="StudentsGridView" runat="server" AutoGenerateColumns="False" DataKeyNames="StudentId" DataSourceID="StudentsSqlDataSource" Width="854px" OnRowCommand="StudentsGridView_RowCommand" style="margin-left: 42px; margin-top: 75px">
    <Columns>
        <asp:BoundField DataField="StudentId" HeaderText="StudentId" InsertVisible="False" ReadOnly="True" SortExpression="StudentId" />
        <asp:BoundField DataField="StudentName" HeaderText="StudentName" SortExpression="StudentName" />
        <asp:BoundField DataField="StudentEmail" HeaderText="StudentEmail" SortExpression="StudentEmail" />
        <asp:BoundField DataField="StudyGroupId" HeaderText="StudyGroupId" SortExpression="StudyGroupId" />
        <asp:ButtonField ButtonType="Button" CommandName="editStudentButton" Text="EDIT" />
        <asp:ButtonField ButtonType="Button" CommandName="deleteStudentButton" Text="DELETE" />
    </Columns>
</asp:GridView>
<asp:SqlDataSource ID="StudentsSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:CollegeManagementSystemDBConnectionString %>" DeleteCommand="DELETE FROM [Students] WHERE [StudentId] = @StudentId" InsertCommand="INSERT INTO [Students] ([StudyGroupId], [StudentName], [StudentEmail]) VALUES (@StudyGroupId, @StudentName, @StudentEmail)" SelectCommand="SELECT [StudyGroupId], [StudentName], [StudentEmail], [StudentId] FROM [Students]" UpdateCommand="UPDATE [Students] SET [StudyGroupId] = @StudyGroupId, [StudentName] = @StudentName, [StudentEmail] = @StudentEmail WHERE [StudentId] = @StudentId">
    <DeleteParameters>
        <asp:Parameter Name="StudentId" Type="Int32" />
    </DeleteParameters>
    <InsertParameters>
        <asp:Parameter Name="StudyGroupId" Type="Int32" />
        <asp:Parameter Name="StudentName" Type="String" />
        <asp:Parameter Name="StudentEmail" Type="String" />
    </InsertParameters>
    <UpdateParameters>
        <asp:Parameter Name="StudyGroupId" Type="Int32" />
        <asp:Parameter Name="StudentName" Type="String" />
        <asp:Parameter Name="StudentEmail" Type="String" />
        <asp:Parameter Name="StudentId" Type="Int32" />
    </UpdateParameters>
</asp:SqlDataSource>
    <asp:Button ID="downloadXmlButton" runat="server" BackColor="#999999" Font-Size="Larger" style="margin-left: 40px" Text="Download XML report" Width="858px" OnClick="xmlButton_Click" />
    <br />
    <br />
    <br />
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Label ID="Label1" runat="server" Font-Italic="True" Font-Size="Large" Text="Student id:::::"></asp:Label>
    <asp:TextBox ID="studentIdTextBox" runat="server" BorderColor="#FFCC00" Font-Size="Larger" style="margin-left: 19px; margin-top: 7px" Width="249px" Enabled="False"></asp:TextBox>
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <br />
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Label ID="Label2" runat="server" Font-Italic="True" Font-Size="Large" Text="Student name:"></asp:Label>
    <asp:TextBox ID="studentNameTextBox" runat="server" BorderColor="#FFCC00" Font-Size="Larger" style="margin-left: 18px" Width="248px"></asp:TextBox>
    <br />
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Label ID="Label3" runat="server" Font-Italic="True" Font-Size="Large" Text="Student email:"></asp:Label>
    <asp:TextBox ID="studentEmailTextBox" runat="server" BorderColor="#FFCC00" Font-Size="Larger" style="margin-left: 18px" Width="248px"></asp:TextBox>
    <br />
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Label ID="Label4" runat="server" Font-Italic="True" Font-Size="Large" Text="Study group:::"></asp:Label>
    <asp:DropDownList ID="studyGroupDropDownList" runat="server" Font-Size="Larger" style="margin-left: 17px" Width="255px">
        <asp:ListItem>1080</asp:ListItem>
        <asp:ListItem>1081</asp:ListItem>
        <asp:ListItem>1082</asp:ListItem>
    </asp:DropDownList>
    <br />
    &nbsp;<br />
    <br />
    <asp:Button ID="addStudentButton" runat="server" BackColor="#999999" Font-Size="Larger" style="margin-left: 168px" Text="ADD" Width="226px" OnClick="addStudentButton_Click" />
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <br />
    <asp:Button ID="updateStudentButton" runat="server" BackColor="#009900" Font-Size="Larger" style="margin-left: 92px" Text="UPDATE" Visible="False" Width="217px" OnClick="updateStudentButton_Click" />
    <asp:Button ID="cancelStudentButton" runat="server" BackColor="Red" Font-Size="Larger" style="margin-left: 40px" Text="CANCEL" Visible="False" Width="219px" OnClick="cancelStudentButton_Click" />
    <br />
    <br />
    <br />
    <br />
    <asp:TextBox ID="studentMessageTextBox" runat="server" BackColor="#FFCC00" BorderColor="White" BorderStyle="Ridge" Font-Bold="True" Font-Italic="True" Font-Size="Larger" ForeColor="Black" Height="90px" style="margin-left: 88px" TextMode="MultiLine" Width="468px" ReadOnly="True"></asp:TextBox>
    <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;

    <br />
    <br />

    <div id="studentsChart" style="width: 500px; height: 400px">

    </div>
    </asp:Content>
