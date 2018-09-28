using Orchad.DynamicsNavPov.WebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using odata = NAV;

namespace Orchad.DynamicsNavPov.Controllers
{
    public class DynamicsNavWebServicePocController : Controller
    {
        // GET: DynamicsNavWebServicePoc
        public string Index()
        {
            string customerInfo = "PoC";            
            customerInfo = TestDynamicsNavSoapService(); 
            return customerInfo;
        }

        public string IndexOdata()
        {
            string customerInfo = "PoC";
            customerInfo = TestDynamicsNavODataWebService();
            return customerInfo;
        }

        private static string TestDynamicsNavODataWebService()
        {
            var nav = new odata.NAV(new Uri("http://win-k4iu0ll836s:7048/DynamicsNAV90/OData/Company('CRONUS%20International%20Ltd.')/"));
            nav.Credentials = CredentialCache.DefaultNetworkCredentials;

            var sb = new StringBuilder();

            sb.AppendLine("Printing list of customers OData");
            sb.AppendLine(PrintCustomersOdata(nav));

            odata.Customer newCustomer = new odata.Customer();
            newCustomer.Name = "Customer Name Odata";
            nav.AddToCustomer(newCustomer);
            nav.SaveChanges();

            sb.AppendLine(PrintCustomersOdata(nav));
            return sb.ToString();
        }

        private static string PrintCustomersOdata(odata.NAV nav)
        {
            var sb = new StringBuilder();
            var customers = (from c in nav.Customer
                             where c.Name.Contains("a")
                             select c).ToList();
            bool customerFound = false;
            foreach (odata.Customer customer in customers)
            {
                customerFound = true;
                sb.AppendLine($"No.: {customer.No} Name: {customer.Name}");
            }

            if (!customerFound)
            {
                sb.AppendLine("The are no customers fulfilling filter criteria");
            }

            return sb.ToString();
        }


        private static string  TestDynamicsNavSoapService()
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
            sb.AppendLine("Printing Customer List Soap");

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