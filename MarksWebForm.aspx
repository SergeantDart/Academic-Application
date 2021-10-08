<%@ Page Title="" Language="C#" MasterPageFile="~/AdminSite.Master" AutoEventWireup="true" CodeBehind="MarksWebForm.aspx.cs" Inherits="College_Mangement_System.GradesWebForm" %>
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
                        url: "MarksWebForm.aspx/GetYearlyFlunkings",
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
                data.addColumn("string", "year");
                data.addColumn("number", "flunkingsNo");

                for (var i = 0; i < values.length; i++) {
                    data.addRow([values[i].year, values[i].flunkingsNo]);
                }

                var chart = new google.visualization.LineChart(document.getElementById("marksChart"));

                chart.draw(data,
                    {
                        title: "Evolution of flunkings per year",
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
    <asp:Label ID="Label12" runat="server" Font-Italic="True" Font-Overline="True" Font-Size="XX-Large" Font-Strikeout="False" Font-Underline="True" ForeColor="#FF9900" Text="_____________________MARKS_________________"></asp:Label>
    <asp:GridView ID="MarksGridView" runat="server" AutoGenerateColumns="False" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" DataKeyNames="MarkId" DataSourceID="MarksSqlDataSource" ForeColor="Black" GridLines="Vertical" style="margin-left: 110px; margin-right: 0px; margin-top: 67px" AllowPaging="True" PageSize="8">
        <AlternatingRowStyle BackColor="White" />
        <Columns>
            <asp:BoundField DataField="MarkId" HeaderText="MarkId" InsertVisible="False" ReadOnly="True" SortExpression="MarkId" />
            <asp:BoundField DataField="SubjectName" HeaderText="SubjectName" ReadOnly="True" SortExpression="SubjectName" />
            <asp:BoundField DataField="StudentName" HeaderText="StudentName" ReadOnly="True" SortExpression="StudentName" />
            <asp:BoundField DataField="MarkScore" HeaderText="MarkScore" SortExpression="MarkScore" />
            <asp:BoundField DataField="MarkDate" HeaderText="MarkDate" ReadOnly="True" SortExpression="MarkDate" />
            <asp:CommandField ShowEditButton="True" />
            <asp:CommandField ShowDeleteButton="True" />
        </Columns>
        <FooterStyle BackColor="#CCCC99" />
        <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
        <RowStyle BackColor="#F7F7DE" />
        <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
        <SortedAscendingCellStyle BackColor="#FBFBF2" />
        <SortedAscendingHeaderStyle BackColor="#848384" />
        <SortedDescendingCellStyle BackColor="#EAEAD3" />
        <SortedDescendingHeaderStyle BackColor="#575357" />
    </asp:GridView>
    <asp:SqlDataSource ID="MarksSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:CollegeManagementSystemDBConnectionString %>" SelectCommand="SELECT MarkId, SubjectName, StudentName, MarkScore, MarkDate
FROM Students st, Subjects su, Marks m
WHERE m.SubjectId = su.SubjectId AND m.StudentId = st.StudentId" UpdateCommand="UPDATE Marks SET MarkScore = @MarkScore WHERE (MarkId = @MarkId)" DeleteCommand="DELETE FROM Marks WHERE MarkId = @MarkId">
        <DeleteParameters>
            <asp:Parameter Name="MarkId" />
        </DeleteParameters>
        <UpdateParameters>
            <asp:Parameter Name="MarkScore" />
            <asp:Parameter Name="MarkId" />
        </UpdateParameters>
    </asp:SqlDataSource>
    <br />
    <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Label ID="Label1" runat="server" Font-Italic="True" Font-Size="Large" Text="Mark id::::::::::"></asp:Label>
    <asp:TextBox ID="markIdTextBox" runat="server" BorderColor="#FFCC00" Font-Size="Larger" style="margin-left: 16px; margin-top: 7px" Width="249px" Enabled="False" ReadOnly="True"></asp:TextBox>
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <br />
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Label ID="Label2" runat="server" Font-Italic="True" Font-Size="Large" Text="Subject:::::::::"></asp:Label>
    <asp:DropDownList ID="subjectsDropDownList" runat="server" Font-Size="Larger" style="margin-left: 28px" Width="255px">
    </asp:DropDownList>
    <br />
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Label ID="Label3" runat="server" Font-Italic="True" Font-Size="Large" Text="Student name::"></asp:Label>
    <asp:DropDownList ID="studentsDropDownList" runat="server" Font-Size="Larger" style="margin-left: 25px" Width="255px">
    </asp:DropDownList>
    <br />
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Label ID="Label4" runat="server" Font-Italic="True" Font-Size="Large" Text="Mark score:::::"></asp:Label>
    <asp:TextBox ID="markScoreTextBox" runat="server" BorderColor="#FFCC00" Font-Size="Larger" style="margin-left: 20px; margin-top: 7px" Width="249px" TextMode="Number"></asp:TextBox>
    <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Label ID="Label13" runat="server" Font-Italic="True" Font-Size="Large" Text="Mark date:::::"></asp:Label>
    <asp:TextBox ID="markDateTextBox" runat="server" BorderColor="#FFCC00" Font-Size="Larger" style="margin-left: 27px; margin-top: 7px" Width="253px" TextMode="Date"></asp:TextBox>
    <br />
    <br />
    <br />
    <asp:Button ID="addMarkButton" runat="server" BackColor="#999999" Font-Size="Larger" style="margin-left: 168px" Text="ADD" Width="321px" OnClick="addMarkButton_Click" />
    <br />
    <br />
    <br />
    <asp:TextBox ID="markMessageTextBox" runat="server" BackColor="#FFCC00" BorderColor="White" BorderStyle="Ridge" Font-Bold="True" Font-Italic="True" Font-Size="Larger" ForeColor="Black" Height="90px" style="margin-left: 88px" TextMode="MultiLine" Width="468px"></asp:TextBox>
    <br />
    <br />
    <br />
    <br />

    <div id="marksChart">

    </div>
</asp:Content>
