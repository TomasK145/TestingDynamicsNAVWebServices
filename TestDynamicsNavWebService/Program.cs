using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using soap = TestDynamicsNavWebService.WebService ;
using odata = NAV;
using System.Net;

namespace TestDynamicsNavWebService
{
    class Program
    {
        static void Main(string[] args)
        {
            TestDynamicsNavODataWebService();

            Msg("Press [ENTER] to exit program!");
            Console.ReadLine();
        }

        private static void TestDynamicsNavODataWebService()
        {
            var nav = new odata.NAV(new Uri("http://win-k4iu0ll836s:7048/DynamicsNAV90/OData/Company('CRONUS%20International%20Ltd.')/"));
            nav.Credentials = CredentialCache.DefaultNetworkCredentials;

            Console.WriteLine("Printing list of customers");
            PrintCustomersOdata(nav);

            odata.Customer newCustomer = new odata.Customer();
            newCustomer.Name = "Customer Name Odata";
            nav.AddToCustomer(newCustomer);
            nav.SaveChanges();


        }

        private static void PrintCustomersOdata(odata.NAV nav)
        {
            var customers = (from c in nav.Customer
                            where c.Name.Contains("a")
                            select c).ToList();
            bool customerFound = false;
            foreach (odata.Customer customer in customers)
            {
                customerFound = true;
                Console.WriteLine($"No.: {customer.No} Name: {customer.Name}");
            }

            if (!customerFound)
            {
                Console.WriteLine("The are no customers fulfilling filter criteria");
            }
        }

        private static void TestDynamicsNavSoapWebService()
        {
            // Creates instance of service and sets credentials.  
            soap.Customer_Service service = new soap.Customer_Service();
            service.UseDefaultCredentials = true;
            // Creates instance of customer.  
            soap.Customer cust = new soap.Customer();
            cust.Name = "Customer Name";
            Msg("Pre Create");
            PrintCustomer(cust);

            // Inserts customer.  
            service.Create(ref cust);
            Msg("Post Create");
            PrintCustomer(cust);

            // Creates filter for searching for customers.  
            List<soap.Customer_Filter> filterArray = new List<soap.Customer_Filter>();
            soap.Customer_Filter nameFilter = new soap.Customer_Filter();
            nameFilter.Field = soap.Customer_Fields.Name;
            nameFilter.Criteria = "C*";
            filterArray.Add(nameFilter);

            Msg("List before modification");
            PrintCustomerList(service, filterArray);

            // Creates filter for searching for customers.  
            List<soap.Customer_Filter> filterArray1 = new List<soap.Customer_Filter>();
            soap.Customer_Filter noFilter = new soap.Customer_Filter();
            noFilter.Field = soap.Customer_Fields.No;
            noFilter.Criteria = "01*";
            filterArray1.Add(noFilter);
            PrintCustomerList(service, filterArray1);

            cust.Name = cust.Name + "Updated";
            service.Update(ref cust);

            Msg("Post Update");
            PrintCustomer(cust);

            Msg("List after modification");
            PrintCustomerList(service, filterArray);
            //service.Delete(cust.Key);

            Msg("List after deletion");
            PrintCustomerList(service, filterArray);

            

        }

        // Prints list of filtered customers.  
        static void PrintCustomerList(soap.Customer_Service service, List<soap.Customer_Filter> filter)
        {
            Msg("Printing Customer List");

            // Runs the actual search.  
            soap.Customer[] list = service.ReadMultiple(filter.ToArray(), null, 100);
            foreach (soap.Customer c in list)
            {
                PrintCustomer(c);
            }
            Msg("End of List");
        }

        static void PrintCustomer(soap.Customer c)
        {
            Console.WriteLine("No: {0} Name: {1} Location_code: {2}", c.No, c.Name, c.Location_Code);
        }

        static void Msg(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
