using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using DataSetSampleQueries;
using SampleSupport;


namespace SampleQueries
{
    [Title("LINQ Query Samples")]
    [Prefix("Linq")]
    public class CustomSamples : SampleHarness
    {
        LinqSamples samples = new LinqSamples();
        [Category("Day 18")]
        [Title("Task 1")]
        [Description("Gets a list of all customers whose total orders exceed the given value.")]
        public void LinqQuery01()
        {
            List<LinqSamples.Customer> customers = samples.GetCustomerList();
            decimal value = 5;
            Console.WriteLine("the given value: {0}", value);
            var resultCustomers = customers.Where(customer => customer.Orders.Sum(order => order.Total) > value);
            foreach (var x in resultCustomers)
            {
                Console.WriteLine("Customer ID: {0}", x.CustomerID);
            }
        }

        [Category("Day 18")]
        [Title("Task 2_1")]
        [Description("Gets a list of supplies for each customer located at the same country and city without GroupBy")]
        public void LinqQuery02_1()
        {
            List<LinqSamples.Customer> customers = samples.GetCustomerList();
            List<LinqSamples.Supplier> suppliers = samples.GetSupplierList();
            foreach (var customer in customers)
            {
                Console.WriteLine("For customer {0}:", customer.CustomerID);
                Console.WriteLine("located in {0}, {1}:", customer.Country, customer.City);
                var resultSuppliers = suppliers.Where(x => x.Country == customer.Country && x.City == customer.City);
                foreach (var item in resultSuppliers)
                {
                    Console.WriteLine("{0}, {1}, {2}", item.SupplierName, item.Country, item.City);

                }
            }
        }

        [Category("Day 18")]
        [Title("Task 2_2")]
        [Description("Gets a list of supplies for each customer located at the same country and city with GroupBy")]
        public void LinqQuery02_2()
        {
            List<LinqSamples.Customer> customers = samples.GetCustomerList();
            List<LinqSamples.Supplier> suppliers = samples.GetSupplierList();
            foreach (var customer in customers)
            {
                Console.WriteLine("For customer {0}:", customer.CustomerID);
                Console.WriteLine("located in {0}, {1}:", customer.Country, customer.City);
                var resultSuppliers = suppliers.Where(x => x.Country == customer.Country && x.City == customer.City).GroupBy(x => x.Country);
                foreach (var group in resultSuppliers)
                {
                    foreach (var item in group)
                    {
                        Console.WriteLine("{0}, {1}, {2}", item.SupplierName, item.Country, item.City);
                    }
                }
            }
        }

        [Category("Day 18")]
        [Title("Task 3")]
        [Description("Gets a list of all customers whose order exceed the given value")]
        public void LinqQuery03()
        {
            List<LinqSamples.Customer> customers = samples.GetCustomerList();
            int value = 300;
            Console.WriteLine("The given value {0}", value);
            var resultCustomers = customers.Where(customer => customer.Orders.All(order => order.Total > value));
            foreach (var customer in resultCustomers)
            {
                Console.WriteLine("Customer id: {0}\n Min cost of order: {1}", customer.CustomerID, customer.Orders.Min(order => order.Total));
            }
        }

        [Category("Day 18")]
        [Title("Task 4")]
        [Description("Gets a list of all customers ordered by year and month of the first order, orders tour, customer name")]
        public void LinqQuery04()
        {
            List<LinqSamples.Customer> customers = samples.GetCustomerList();
            var resultCustomers = customers.OrderBy(customer => customer.Orders.First().OrderDate).OrderBy(x => x.Orders.OrderBy(y => y.Total));
            foreach (var customer in resultCustomers)
            {
                Console.WriteLine("Customer id: {0}\n First order date: {1}", customer.CustomerID, customer.Orders.First().OrderDate);
                Console.WriteLine("Total: {0}", customer.Orders);

            }
        }

        [Category("Day 18")]
        [Title("Task 5")]
        [Description("Gets a list of customers with non-decimal postcode or the region is not specified or there is no parentheses in phone number ")]
        public void LinqQuery05()
        {
            List<LinqSamples.Customer> customers = samples.GetCustomerList();
            var resultCustomers = customers.Where(customer => ContainsLetters(customer.PostalCode) ||
                customer.Region == "" || customer.Region == null || !customer.Phone.Contains("("));

            bool ContainsLetters(string postalCode)
            {
                return int.TryParse(postalCode, out int number);
            }

            foreach (var customer in resultCustomers)
            {
                Console.WriteLine("Customer id {0}", customer.CustomerID);
                Console.WriteLine("Customer postcode {0}", customer.PostalCode);
                Console.WriteLine("Customer region {0}", customer.Region);
                Console.WriteLine("Customer phone number {0}", customer.Phone);
            }
        }

        [Category("Day 18")]
        [Title("Task 6")]
        [Description("Groups all products by categories. Inside that - by existance in stock. Inside that - by price")]
        public void LinqQuery06()
        {
            List<LinqSamples.Product> products = samples.GetProductList();
            var productgroups = products.GroupBy(product => product.Category).GroupBy(x => x.GroupBy(y => y.UnitsInStock).GroupBy(z => z.GroupBy(c => c.UnitPrice)));

            foreach (var product in productgroups)
            {
                Console.WriteLine(product.Key);
                foreach (var itemgroup in product)
                {
                    Console.WriteLine(itemgroup.Key);

                    foreach (var item in itemgroup)
                    {
                        Console.WriteLine("Category: {0}\n Units in stock: {1}\n Price: {2}", item.Category, item.UnitsInStock, item.UnitPrice);
                    }
                }
            }
        }

        [Category("Day 18")]
        [Title("Task 7")]
        [Description("Groups all products into 3 groups: cheap, middle, expensive")]
        public void LinqQuery07()
        {
            List<LinqSamples.Product> products = samples.GetProductList();

            var productgroupCheap = products.Where(x => x.UnitPrice < 5);
            var productgroupMiddle = products.Where(x => x.UnitPrice >= 5 || x.UnitPrice < 20);
            var productgroupExpensive = products.Where(x => x.UnitPrice > 20);

            Console.WriteLine("Cheap group");
            foreach (var item in productgroupCheap)
            {
                Console.WriteLine("{0}", item.ProductID);
                Console.WriteLine("{0}", item.UnitPrice);
            }

            Console.WriteLine("Middle group");
            foreach (var item in productgroupMiddle)
            {
                Console.WriteLine("{0}", item.ProductID);
                Console.WriteLine("{0}", item.UnitPrice);
            }

            Console.WriteLine("Expensive group");
            foreach (var item in productgroupExpensive)
            {
                Console.WriteLine("{0}", item.ProductID);
                Console.WriteLine("{0}", item.UnitPrice);
            }
        }


        [Category("Day 18")]
        [Title("Task 8")]
        [Description("Gets average order price by all customers in specified city, average amount of orders from each city per customer")]
        public void LinqQuery08()
        {
            List<LinqSamples.Customer> customers = samples.GetCustomerList();
            var city = "London";
            Console.WriteLine("City: {0}", city);

            var avgOrderTotal = customers.Where(x => x.City == city).Average(y => y.Orders.Average(z => z.Total));
            Console.WriteLine("Average total order price: {0}", avgOrderTotal);

            var avgOrderCount = customers.GroupBy(x => x.City)
                .Select(cityGroup => new { City = cityGroup.Key, AvgValue = cityGroup.Average(customer => customer.Orders.Count()) });

            foreach (var avg in avgOrderCount)
            {
                Console.WriteLine("City: {0}, Average amount of orders: {1}", avg.City, avg.AvgValue);
            }
        }

        [Category("Day 18")]
        [Title("Task 9_1")]
        [Description("Gets supplies with address started with decimal")]
        public void LinqQuery09_1()
        {
            List<LinqSamples.Supplier> supplies = samples.GetSupplierList();
            var suppliesWithDecAddress = supplies.Where(x => !String.IsNullOrEmpty(x.Address) && !Char.IsLetter(x.Address[0]));

            foreach (var supplier in suppliesWithDecAddress)
            {
                Console.WriteLine("Name: {0}, Address: {1}", supplier.SupplierName, supplier.Address);
            }
        }

        [Category("Day 18")]
        [Title("Task 9_2")]
        [Description("Gets all customers and supplies located at the same country")]
        public void LinqQuery09_2()
        {
            List<LinqSamples.Supplier> supplies = samples.GetSupplierList();
            List<LinqSamples.Customer> customers = samples.GetCustomerList();

            var cityGroup = customers.GroupBy(x => x.City);

            foreach (var cityGroupItem in cityGroup)
            {
                Console.WriteLine("City: " + cityGroupItem.Key);
                Console.Write("Cusomers: ");

                foreach (var customer in cityGroupItem)
                {
                    Console.Write(customer.CustomerID + " ");
                }

                Console.Write("Suppliers: ");

                foreach (var supplier in supplies.Where(x => x.City == cityGroupItem.Key))
                {
                    Console.Write(supplier.SupplierName + " ");
                }

                Console.WriteLine();
            }

        }
    }
}
