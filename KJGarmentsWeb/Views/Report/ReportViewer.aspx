<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>
<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
<meta name="viewport" content="width=device-width" />
<title>ReportViwer in MVC4 Application</title>
<script runat="server">
void Page_Load(object sender, EventArgs e)
 {
if (!IsPostBack)
{
List<KJGarmentsWeb.Models.UserTransaction> objuserTranList =null;
using (KJGarmentsWeb.DAL.MughapuEntities db = new KJGarmentsWeb.DAL.MughapuEntities())
 {
//     customers = dc.PRODUCTs.OrderBy(a => a.Pid).ToList();
//     ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/RptUserTransaction.rdlc");
//ReportViewer1.LocalReport.DataSources.Clear();
//ReportDataSource rdc = new ReportDataSource("EmpDataset", customers);
// ReportViewer1.LocalReport.DataSources.Add(rdc);
//ReportViewer1.LocalReport.Refresh();

objuserTranList = new List<KJGarmentsWeb.Models.UserTransaction>();
var query = from PO in db.ProductOrders
            join u in db.User_Info on new { Email = PO.Emailid } equals new { Email = u.E_mail_id }
            join K in db.Coupon_Code on PO.Cupn_id equals K.Cupn_Id
            select new { UserId = u.UidNo, Name = PO.Customer_name, email = PO.Emailid, Amount = PO.Total, offervalue = K.MinQty, value_type = K.MaxQty };

foreach (var d in query)
{
    KJGarmentsWeb.Models.UserTransaction obj = new KJGarmentsWeb.Models.UserTransaction();
    obj.Userid = d.UserId;
    obj.Name = d.Name;
    obj.EmailId = d.email;
    obj.TotalAmount = Convert.ToInt32(d.Amount);
    obj.CommisionAmount = Convert.ToInt32(d.value_type == 1 ? ((d.offervalue * d.Amount) / 100) : d.Amount - d.offervalue);
    objuserTranList.Add(obj);
}

ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/RptUserAgent.rdlc");
ReportViewer1.LocalReport.DataSources.Clear();
ReportDataSource rdc = new ReportDataSource("RptDataSet", objuserTranList);
ReportViewer1.LocalReport.DataSources.Add(rdc);
ReportViewer1.LocalReport.Refresh();  
}
}
}
 </script>
</head>
<body>
<form id="form1" runat="server">
<div>
 <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
<rsweb:ReportViewer ID="ReportViewer1" runat="server" AsyncRendering="false" SizeToReportContent="true">
</rsweb:ReportViewer>
</div>
</form>
</body>
</html>
