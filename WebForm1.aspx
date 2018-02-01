<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="TextSearchTesting.WebForm1" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <style type="text/css">
        .modal {
            position: fixed;
            top: 0;
            left: 0;
            background-color: black;
            z-index: 99;
            opacity: 0.8;
            filter: alpha(opacity=80);
            -moz-opacity: 0.8;
            min-height: 100%;
            width: 100%;}

        .loading {
            font-family: Arial;
            font-size: 10pt;
            border: 5px solid rgb(80, 124, 209);
            width: 200px;
            height: 130px;
            display: none;
            position: fixed;
            background-color: White;
            z-index: 999;}

        .tooltip {
            position: relative;
            display: inline-block;}

        .tooltip .tooltiptext {
            visibility: hidden;
            width: 250px;
            background-color: black;
            color: #fff;
            text-align: center;
            border-radius: 6px;
            padding: 5px 0;
            position: absolute;
            z-index: 1;
            top: 110%;
            left: 100%;
            margin-left: -60px;}

        .tooltip:hover .tooltiptext {
            visibility: visible;}
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table width="100%">
                <tr>
                    <td valign="top">
                        <table width="100%">
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtSearch" runat="server" Width="234px"></asp:TextBox>&nbsp;<asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label>
                                    <br />
                                    <div class="tooltip" style="padding-top: 5px;"><asp:Button ID="btnIndex" runat="server" Text="Index" OnClick="btnIndex_Click" /><span class="tooltiptext">Scan all documents and format them for quick searches.</span></div>
                                    . . <div class="tooltip"><asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" /><span class="tooltiptext">Enter a keyword and click search.</span></div>
                                </td>
                                <td>
                                    <asp:Image ID="Image1" runat="server" ImageUrl="~/images/kb.jpg" />
                                </td>
                                <td align="right" valign="top">
                                    <div class="tooltip">
                                        <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/images/normal.png" OnClick="ImageButton1_Click" onmouseover="this.src='/images/hover.png'" onmouseout="this.src='/images/normal.png'"/>
                                        <span class="tooltiptext" style="top: 95%;">View help and learn about the app.</span>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    <asp:Label ID="lblResults" runat="server" Width="100%"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td valign="top" align="right" width="10%">
                        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" OnRowDataBound="GridView1_RowDataBound" CellPadding="4" ForeColor="#333333" GridLines="None">
                            <AlternatingRowStyle BackColor="White" />
                            <Columns>
                                <asp:BoundField DataField="Text" HeaderText="File Name">
                                    <ItemStyle Wrap="False" />
                                </asp:BoundField>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkDownload" Text="download" CommandArgument='<%# Eval("Value") %>' runat="server" OnClick="DownloadFile"></asp:LinkButton>
                                        <asp:Image ID="Image3" runat="server" ImageUrl="~/images/foldericon.png" Visible="False"></asp:Image>
                                    </ItemTemplate>
                                    <ItemStyle Wrap="False" HorizontalAlign="Center" />
                                </asp:TemplateField>
                            </Columns>
                            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                            <RowStyle BackColor="#EFF3FB" />
                            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                            <SortedAscendingCellStyle BackColor="#F5F7FB" />
                            <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                            <SortedDescendingCellStyle BackColor="#E9EBEF" />
                            <SortedDescendingHeaderStyle BackColor="#4870BE" />
                        </asp:GridView>
                        <br />
                        <asp:Label ID="lblfilecount" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
            </table>
        </div>

        <div class="loading" align="center"> <br />
            Indexing Files.  Please wait.<br /> <br />
            <img src="loading3.gif" alt="" />
        </div>

        <script type="text/javascript"> 

            function ShowProgress() {
                setTimeout(function () {
                    var modal = $('<div />');
                    modal.addClass("modal");
                    $('body').append(modal);
                    var loading = $(".loading");
                    loading.show();
                    var top = Math.max($(window).height() / 2 - loading[0].offsetHeight / 2, 0);
                    var left = Math.max($(window).width() / 2 - loading[0].offsetWidth / 2, 0);
                    loading.css({ top: top, left: left });
                }, 200);
            }

            $('form').live("submit", function () {
                ShowProgress();
            });

        </script>
    </form>
</body>
</html>
