using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
 using System.Web.Mvc;
 using Microsoft.Reporting.WebForms;
using KJGarmentsWeb.DAL;
namespace KJGarmentsWeb.Models
{
    public class EmployeeMaster
    {
        public int EmpId { get; set; }
        public string EmpName { get; set; }
        public string DesignationName { get; set; }
        public string Location { get; set; }
        public Boolean EmpStatus { get; set; }
    }
    public class EmployeeModelService
    {
        public  List<EmployeeMaster> GetEmployeeInfo()
        {
            List<EmployeeMaster> objEmpList = new List<EmployeeMaster>();

            EmployeeMaster obj = new EmployeeMaster();
            obj.EmpId = 1;
            obj.EmpName = "Mr.XYZ";
            obj.DesignationName = "Software QA";
            obj.Location = "BLR";
            obj.EmpStatus = true;
            objEmpList.Add(obj);
            EmployeeMaster obj1 = new EmployeeMaster();
            obj1.EmpId = 2;
            obj1.EmpName = "Mr.ABC";
            obj1.DesignationName = "QA";
            obj1.Location = "BLR";
            obj1.EmpStatus = true;
            objEmpList.Add(obj1);
            
                return objEmpList;
            
        }
    }
    public class UserTransaction
    {
        public int Userid { get; set; }
        public string Name { get; set; }
        public string EmailId { get; set; }
        public int TotalAmount { get; set; }
        public int CommisionAmount { get; set; }
    }
    public class UserTransactionService
    {
        MughapuEntities db = new MughapuEntities();
        public List<UserTransaction> GetuserTranInfo()
        {
            List<UserTransaction> objuserTranList = new List<UserTransaction>();
            var query = from PO in db.ProductOrders
                        join u in db.User_Info on new { Email = PO.Emailid } equals new { Email = u.E_mail_id }
                        join K in db.Coupon_Code on PO.Cupn_id equals K.Cupn_Id
                        select new { UserId = u.UidNo, Name = PO.Customer_name, email = PO.Emailid, Amount = PO.Total, offervalue =K.MinQty,value_type=K.MaxQty};

            foreach (var d in query)
            {
                UserTransaction obj = new UserTransaction();
                obj.Userid = d.UserId;
                obj.Name = d.Name;
                obj.EmailId = d.email;
                obj.TotalAmount =Convert.ToInt32(d.Amount);
                obj.CommisionAmount =Convert.ToInt32(d.value_type == 1 ? (d.Amount - (d.offervalue * d.Amount) / 100) : d.Amount - d.offervalue);
                objuserTranList.Add(obj);
            }
            return objuserTranList;

        }
    }
     

      
 
}