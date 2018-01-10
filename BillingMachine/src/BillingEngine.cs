using BillingMachine.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BillingMachine
{
    public class BillingEngine : IBillingEngine
    {
        private ICalculations _calculations;
        private ICSVRead _csvRead;

        List<CDR> _cdrs = new List<CDR>();
        List<Customer> _customers = new List<Customer>();
        List<Package> _packages = new List<Package>();
        double _taxRate, _taxPresentage;

        public BillingEngine()
        {
            _calculations = new Calculations();
            _csvRead = new CSVRead();

            _packages = _csvRead.ReadPackages();
            _cdrs = _csvRead.ReadCDRs();
            _customers = _csvRead.ReadCustomers(_packages);

            _taxPresentage = 20.0;
            _taxRate = _taxPresentage / 100;
        }

        /*
        This method will calculate charges for each call and add call details to bill
        then finally bill will be attche to the corresponding customer.
        */
        public List<double> ChargersCalculate(int cutomerID)
        {
            
            List<double> callChargers = new List<double>(); //list of call chargers for return details for testing
            Customer customer = _customers.Find(c => c.ID == cutomerID); //Finding the customer using customer ID

            //Genarate new bill for the cust0mer
            Bill bill = new Bill
            {
                Number = customer.Number,
                CallList = new List<CallDetails>()
            };
            int i = 0;

            //Checking each cdr and find cdr's that belongs to the current customer
            foreach(CDR item in _cdrs)
            {
                if(item.CallingParty.Extention == customer.Number.Extention && item.CallingParty.Unique == customer.Number.Unique)
                {
                    double charge = 0;

                    //Check cutomer package is per minite or per second
                    if (customer.PackageType.BillingType == BillType.PerMinute)
                    {
                        charge = _calculations.CalculateChargeMinite(item, customer.PackageType);
                    }
                    else
                    {
                        charge = _calculations.CalculateChargeSecond(item, customer.PackageType);
                    }

                    //round up caculated call charge to 2 decimal points
                    charge = Math.Round(charge, 2);

                    //genare new detail for this call and add that to the bill
                    CallDetails details = new CallDetails
                    {
                        ID = i,
                        Destination = item.CalledParty,
                        StartTime = item.StartingTime,
                        DurationSeconds = item.CallDuration,
                        Charge = charge
                    };
                    i++;
                    callChargers.Add(charge);//for testing
                    bill.CallList.Add(details);
                }
            }
            //calling the method for add tax and calculate total bill
            AddExternalCharges(bill, customer.PackageType.MonthlyRental);
            if (_customers.Find(c => c.ID == cutomerID).Bills == null)
                _customers.Find(c => c.ID == cutomerID).Bills = new List<Bill>();

            //adding bill ID
            bill.ID = _customers.Find(c => c.ID == cutomerID).Bills.Count;
            _customers.Find(c => c.ID == cutomerID).Bills.Add(bill);

            return callChargers; //for testing
        }

        //This method is the method that read csv data and get each of customer and cdrs connected.
        public List<Customer> Generate()
        {
            //genarating bills for each customer by calling ChargersCalculate method
            foreach(Customer item in _customers)
            {
                var l = ChargersCalculate(item.ID);
            }

            return _customers;
        }

        //Adding aditional chargers like  rental fee, Discunts, taxes and finally calculating total bill
        private Bill AddExternalCharges(Bill bill, double rental)
        {
            //getting list of call list into the separate list for calculating purposes
            ICollection<CallDetails> callLits = bill.CallList;
            bill.TotalCharges = 0; //initializing total charges

            //calculating total call charges
            foreach(CallDetails item in callLits)
            {
                bill.TotalCharges += item.Charge;
            }

            bill.Rental = rental; //adding rental value
            bill.Tax = Math.Round((bill.Rental + bill.TotalCharges) * _taxRate, 2); //calculating tax
            bill.TotalDiscount = 0; //calculating totla discount
            //Calculating total bill amount
            bill.BillAmount = Math.Round(bill.Rental + bill.Tax + bill.TotalCharges - bill.TotalDiscount, 2);

            return bill;
        }

        //method for get bill for a customer that given by user
        public Customer GetBills(string number)
        {
            //getting all the customers details by executing Generate() method
            List<Customer> customers = Generate();
            var values = number.Split('-');
            int.TryParse(values[0], out int ext);
            int.TryParse(values[1], out int unq);
            //getting specific customer
            Customer customer =  customers.Find(cstmr => cstmr.Number.Extention == ext && cstmr.Number.Unique == unq);

            return customer;
        }
    }
}
