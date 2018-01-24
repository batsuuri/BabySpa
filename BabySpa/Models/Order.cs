using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BabySpa
{
    public class Order
    {
        public string order_id { get; set; }
        public string order_date { get; set; }
        public string branch_id { get; set; }
        public string cust_id { get; set; }
        public string service_count { get; set; }
        public string cust_note { get; set; }
        public string user_id { get; set; }
        public string status { get; set; }
        public string contact_info { get; set; }
        public string cancel_reason { get; set; }
        public Cust cust { get; set; }
    }
    public class OrderService
    {
        public string order_id { get; set; }
        public string child_id { get; set; }
        public string order_date { get; set; }
        public string order_time { get; set; }
        public string service_count { get; set; }
        public string extra_prod_id { get; set; }
        public string service_note { get; set; }
        public string service_price { get; set; }
        public string service_date { get; set; }
        public string service_time { get; set; }
        public string service_meas { get; set; }
        public Child child { get; set; }
    }

    public class Cust
    {
        public string cust_id { get; set; }
        public string father_name { get; set; }
        public string mother_name { get; set; }
        public string contact_phone { get; set; }
        public string email { get; set; }
        public string social_id { get; set; }
        public string status { get; set; }
        public string reg_date { get; set; }
        public string reg_user { get; set; }
        public List<Child> ChildList { get; set; }
    }
    public class Child
    {
        public string cust_id { get; set; }
        public string child_id { get; set; }
        public string child_name { get; set; }
        public string sex { get; set; }
        public string  register_no{ get; set; }
        public string birth_date { get; set; }
        public string reg_date { get; set; }
        public string reg_user { get; set; }
        public string last_measurement { get; set; }
    }
}

















}