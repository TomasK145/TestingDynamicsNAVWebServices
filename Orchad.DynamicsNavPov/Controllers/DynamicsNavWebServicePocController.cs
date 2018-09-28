using Orchad.DynamicsNavPov.WebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Orchad.DynamicsNavPov.Controllers
{
    public class DynamicsNavWebServicePocController : Controller
    {
        // GET: DynamicsNavWebServicePoc
        public string Index()
        {
            // Creates instance of service and sets credentials.  
            Customer_Service service = new Customer_Service();
            service.UseDefaultCredentials = true;

            List<Customer_Filter> filterArray = new List<Customer_Filter>();
            Customer_Filter nameFilter = new Customer_Filter();
            nameFilter.Field = Customer_Fields.Name;
            nameFilter.Criteria = "C*";
            filterArray.Add(nameFilter);

            var customerInfo = PrintCustomerList(service, filterArray);
            return customerInfo;
        }

        private static string PrintCustomerList(Customer_Service service, List<Customer_Filter> filter)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Printing Customer List");

            // Runs the actual search.  
            Customer[] list = service.ReadMultiple(filter.ToArray(), null, 100);
            foreach (Customer c in list)
            {
                sb.AppendLine(PrintCustomer(c));
            }
            sb.AppendLine("End of List");
            return sb.ToString();
        }

        private static string PrintCustomer(Customer c)
        {
            return String.Format($"No: {c.No} Name: {c.Name} Location_code: {c.Location_Code}");
        }
    }
}