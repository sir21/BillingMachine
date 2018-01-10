using BillingMachine.Entity;
using System;
using System.Collections.Generic;

namespace BillingMachine
{
    class Program
    {
        static void Main(string[] args)
        {
            IBillingEngine _billingEngine = new BillingEngine();

            List<Customer> customers = _billingEngine.Generate();

            int switchCase = 0;
            string input;

            while (switchCase >= 0)
            {
                Console.WriteLine("To See all customer bills press 1");
                Console.WriteLine("To select one customer bill press 2");
                Console.WriteLine("To see available customers Press 3");
                    input = Console.ReadLine();
                int.TryParse(input, out switchCase);

                switch (switchCase)
                {
                    case 1:
                        List<Bill> bills = new List<Bill>();
                        foreach(Customer customer in customers)
                        {
                            foreach(Bill bill in customer.Bills)
                            {
                                Console.WriteLine("{0} - Rs. {1}", customer.FullName, bill.BillAmount);
                                Console.Write("\n\t\t-----------------\n");
                            }
                        }
                        break;

                    case 2:
                        try
                        {
                            Console.WriteLine("Enter Phone Number Eg:- 012-3456789");
                            string number = Console.ReadLine();
                            Customer selectedCustomer = _billingEngine.GetBills(number);
                            foreach (Bill bill in selectedCustomer.Bills)
                            {
                                Console.Write("Full Name: {0}\nAddress: {1}\nTotal Call Charges: {2}\nTax: {3}\nTotal Bill: {4}\n", selectedCustomer.FullName, selectedCustomer.BillingAddress
                                , bill.TotalCharges, bill.Tax, bill.BillAmount);
                            }
                        }
                        catch
                        {
                            Console.WriteLine("Invalid Phone Number");
                        }
                        break;

                    case 3:
                        try
                        {
                            foreach(Customer cus in customers)
                            {
                                Console.WriteLine("{0}, {1}-{2}", cus.FullName, cus.Number.Extention, cus.Number.Unique);
                                Console.Write("\n\t\t-----------------\n");
                            }
                        }
                        catch
                        {
                            Console.WriteLine("Couldn't read customers");
                        }
                        break;

                    default:
                        switchCase = -1;
                        break;
                }
            }
        }
    }
}
