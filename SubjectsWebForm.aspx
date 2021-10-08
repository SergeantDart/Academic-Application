<%@ Page Title="" Language="C#" MasterPageFile="~/AdminSite.Master" AutoEventWireup="true" CodeBehind="SubjectsWebForm.aspx.cs" Inherits="College_Mangement_System.SubjectsWebForm" %>
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
                        url: "SubjectsWebForm.aspx/GetEachSubjectAvg",
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
                data.addColumn("string", "subjectName");
                data.addColumn("number", "avgMark");

                for (var i = 0; i < values.length; i++) {
                    data.addRow([values[i].subjectName, values[i].avgMark]);
                }

                var chart = new google.visualization.ColumnChart(document.getElementById("subjectsChart"));

                chart.draw(data,
                    {
                        title: "Average mark per subject",
                        position: "top",
                        fontsize: "14px",
                        chartArea: {
                            width: "50%"
                        },
                    });
            }
        }
        
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="AdminContentPlaceHolder" runat="server">
    <asp:Label ID="Label12" runat="server" Font-Italic="True" Font-Overline="True" Font-Size="XX-Large" Font-Strikeout="False" Font-Underline="True" ForeColor="#FF9900" Text="___________________SUBJECTS_________________"></asp:Label>
    <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <br />
    <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:TextBox ID="subjectSearchTextBox" runat="server" BorderColor="#FFCC00" Font-Size="Larger" style="margin-left: 19px; margin-top: 7px" Width="249px"></asp:TextBox>
    &nbsp;&nbsp;
    <asp:Button ID="searchSubjectButton" runat="server" BackColor="#999999" Font-Size="Larger" style="margin-left: 35px; margin-top: 0px;" Text="SEARCH" Width="162px" OnClick="searchSubjectButton_Click" Height="42px" />
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Button ID="getAllSubjectsButton" runat="server" BackColor="#999999" Font-Size="Larger" style="margin-left: 14px; margin-top: 0px;" Text="GET ALL" Width="162px" OnClick="getAllSubjectsButton_Click" Height="42px" />
    &nbsp;<asp:GridView ID="SubjectsGridView" runat="server" AutoGenerateColumns="False" BackColor="LightGoldenrodYellow" BorderColor="Tan" BorderWidth="1px" CellPadding="2" DataKeyNames="SubjectId" DataSourceID="SubjectsSqlDataSource" ForeColor="Black" GridLines="None" style="margin-left: 120px; margin-right: 1px; margin-top: 45px" AllowPaging="True" PageSize="8">
        <AlternatingRowStyle BackColor="PaleGoldenrod" />
        <Columns>
            <asp:BoundField DataField="SubjectId" HeaderText="SubjectId" InsertVisible="False" ReadOnly="True" SortExpression="SubjectId" />
            <asp:BoundField DataField="SubjectName" HeaderText="SubjectName" SortExpression="SubjectName" ReadOnly="True" />
            <asp:BoundField DataField="SubjectCreditPoints" HeaderText="SubjectCreditPoints" SortExpression="SubjectCreditPoints" />
            <asp:BoundField DataField="SubjectDescription" HeaderText="SubjectDescription" SortExpression="SubjectDescription" />
            <asp:CommandField ShowEditButton="True" />
            <asp:CommandField ShowDeleteButton="True" />
        </Columns>
        <FooterStyle BackColor="Tan" />
        <HeaderStyle BackColor="Tan" Font-Bold="True" />
        <PagerStyle BackColor="PaleGoldenrod" ForeColor="DarkSlateBlue" HorizontalAlign="Center" />
        <SelectedRowStyle BackColor="DarkSlateBlue" ForeColor="GhostWhite" />
        <SortedAscendingCellStyle BackColor="#FAFAE7" />
        <SortedAscendingHeaderStyle BackColor="#DAC09E" />
        <SortedDescendingCellStyle BackColor="#E1DB9C" />
        <SortedDescendingHeaderStyle BackColor="#C2A47B" />
    </asp:GridView>
    <asp:SqlDataSource ID="SubjectsSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:CollegeManagementSystemDBConnectionString %>" SelectCommand="SELECT * FROM [Subjects]" DeleteCommand="DELETE FROM [Subjects] WHERE [SubjectId] = @SubjectId" InsertCommand="INSERT INTO [Subjects] ([SubjectName], [SubjectCreditPoints], [SubjectDescription]) VALUES (@SubjectName, @SubjectCreditPoints, @SubjectDescription)" UpdateCommand="UPDATE [Subjects] SET [SubjectName] = @SubjectName, [SubjectCreditPoints] = @SubjectCreditPoints, [SubjectDescription] = @SubjectDescription WHERE [SubjectId] = @SubjectId">
        <DeleteParameters>
            <asp:Parameter Name="SubjectId" Type="Int32" />
        </DeleteParameters>
        <InsertParameters>
            <asp:Parameter Name="SubjectName" Type="String" />
            <asp:Parameter Name="SubjectCreditPoints" Type="Int32" />
            <asp:Parameter Name="SubjectDescription" Type="String" />
        </InsertParameters>
        <UpdateParameters>
            <asp:Parameter Name="SubjectName" Type="String" />
            <asp:Parameter Name="SubjectCreditPoints" Type="Int32" />
            <asp:Parameter Name="SubjectDescription" Type="String" />
            <asp:Parameter Name="SubjectId" Type="Int32" />
        </UpdateParameters>
    </asp:SqlDataSource>
    <p>
    </p>
    <p>
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Label ID="Label11" runat="server" Font-Italic="True" Font-Size="Large" Text="Subject id:::::"></asp:Label>
    <asp:TextBox ID="subjectIdTextBox" runat="server" BorderColor="#FFCC00" Font-Size="Larger" style="margin-left: 19px; margin-top: 7px" Width="249px" ReadOnly="True"></asp:TextBox>
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <br />
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Label ID="Label2" runat="server" Font-Italic="True" Font-Size="Large" Text="Subject name:"></asp:Label>
    <asp:TextBox ID="subjectNameTextBox" runat="server" BorderColor="#FFCC00" Font-Size="Larger" style="margin-left: 18px" Width="248px"></asp:TextBox>
    <br />
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Label ID="Label3" runat="server" Font-Italic="True" Font-Size="Large" Text="CreditPoints::"></asp:Label>
    <asp:TextBox ID="subjectCreditPointsTextBox" runat="server" BorderColor="#FFCC00" Font-Size="Larger" style="margin-left: 18px" Width="248px" TextMode="Number"></asp:TextBox>
    <br />
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Label ID="Label4" runat="server" Font-Italic="True" Font-Size="Large" Text="Description::::"></asp:Label>
    <asp:TextBox ID="subjectDescriptionTextBox" runat="server" BorderColor="#FFCC00" Font-Size="Larger" style="margin-left: 13px" Width="248px" TextMode="MultiLine"></asp:TextBox>
    </p>
    <p>
        &nbsp;</p>
    <p>
    <asp:Button ID="addSubjectButton" runat="server" BackColor="#999999" Font-Size="Larger" style="margin-left: 168px" Text="ADD" Width="301px" OnClick="addSubjectButton_Click" />
    </p>
    <p>
        &nbsp;</p>
    <p>
    <asp:TextBox ID="subjectMessageTextBox" runat="server" BackColor="#FFCC00" BorderColor="White" BorderStyle="Ridge" Font-Bold="True" Font-Italic="True" Font-Size="Larger" ForeColor="Black" Height="90px" style="margin-left: 88px" TextMode="MultiLine" Width="468px"></asp:TextBox>
    </p>
    <p>
        &nbsp;</p>
    <div id="subjectsChart">

    </div>
</asp:Content>
