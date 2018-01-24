using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BabySpa
{
    public class OrderViewModel
    {
         public Order Order { get; set; }
         public List<OrderService> List{ get; set; }
    }
}