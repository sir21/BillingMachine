using BillingMachine;
using BillingMachine.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BillingMachineTest
{
    [TestClass]
    public class BillingMachineTests
    {
        private IBillingEngine _billingEngine;

        [TestInitialize]
        public void Init()
        {
            _billingEngine = new BillingEngine();
        }


        [TestMethod]//1
        public void Generate_Bills_For_Each_Customer()

        {
            var customers =_billingEngine.Generate("Data/customerTest1.csv", "", "");

            Assert.AreEqual(3, customers.Count);



            var customer1 = customers.Find(b => b.Number.Extention == 77 && b.Number.Unique == 7342345);

            Assert.IsNotNull(customer1.Bills);

            Assert.AreEqual("Customer Name 1", customer1.FullName);

            Assert.AreEqual("Billing Address 1", customer1.BillingAddress);



            var customer2 = customers.Find(b => b.Number.Extention == 77 && b.Number.Unique == 2345434);

            Assert.IsNotNull(customer2.Bills);

            Assert.AreEqual("Customer Name 2", customer2.FullName);

            Assert.AreEqual("Billing Address 2", customer2.BillingAddress);

        }

        [TestMethod]//2
        public void Generate_Bills_For_Each_Customer_With_Correct_CDRs()
        {
            var customers = _billingEngine.Generate("Data/customerTest2.csv", "Data/cdrTest2.csv", "");

            var customer1 = customers.Find(b => b.Number.Extention == 77 && b.Number.Unique == 7342345);
            Assert.IsNotNull(customer1);

            Bill bill1 = customer1.Bills.Last();

            Assert.AreEqual(2, bill1.CallList.Count);

            var call1 = bill1.CallList.FirstOrDefault(c => c.StartTime == new DateTime(2017, 1, 2, 8, 30, 0));

            Assert.IsNotNull(call1);

            Assert.AreEqual(20, call1.DurationSeconds);

            var call2 = bill1.CallList.FirstOrDefault(c => c.StartTime == new DateTime(2017, 1, 4, 20, 30, 0));

            Assert.IsNotNull(call2);

            Assert.AreEqual(40, call2.DurationSeconds);
        }

        [TestMethod]//3
        public void Calculate_Peak_Billing_Charges_for_PerMinute_Local_Calls_For_Full_Minutes_Correctly()
        {
            var customers = _billingEngine.Generate("Data/customerTest3.csv", "Data/cdrTest3.csv", "");
            var bills = customers.First().Bills;

            Assert.AreEqual(1, bills.First().CallList.Count);

            Assert.AreEqual(6, bills.First().CallList.First().Charge);

        }



        [TestMethod]//4
        public void Calculate_Peak_Billing_Charges_for_PerMinute_Local_Calls_For_Partial_Minutes_Correctly()
        {
            var customers = _billingEngine.Generate("Data/customerTest3.csv", "Data/cdrTest4.csv", "");

            Assert.AreEqual(1, customers.First().Bills.First().CallList.Count);

            Assert.AreEqual(3, customers.First().Bills.First().CallList.First().Charge);

        }

        [TestMethod]//5
        public void Calculate_Peak_Billing_Charges_for_PerMinute_Local_Calls_For_Durations_More_Than_One_Minutes_Correctly()
        {
            var customers = _billingEngine.Generate("Data/customerTest3.csv", "Data/cdrTest5.csv", "");

            Assert.AreEqual(1, customers.First().Bills.First().CallList.Count);

            Assert.AreEqual(6, customers.First().Bills.First().CallList.First().Charge);

        }

        [TestMethod]//6
        public void Calculate_Peak_Billing_Charges_for_PerSecond_Local_Calls_For_Partial_Minutes_Correctly()
        {
            var customers = _billingEngine.Generate("Data/customerTest6.csv", "Data/cdrTest4.csv", "");

            Assert.AreEqual(1, customers.First().Bills.First().CallList.Count);

            Assert.AreEqual(1.5, customers.First().Bills.First().CallList.First().Charge);

        }

        [TestMethod]//7
        public void Calculate_Peak_Billing_Charges_for_Long_Distance_Calls_Correctly()
        {
            var customers = _billingEngine.Generate("Data/customerTest3.csv", "Data/cdrTest7.csv", "");

            Assert.AreEqual(1, customers.First().Bills.First().CallList.Count);

            Assert.AreEqual(10, customers.First().Bills.First().CallList.First().Charge);

        }

        [TestMethod]//8
        public void Calculate_OffPeak_Billing_Charges_for_Local_Calls_Before_Peak_Correctly()
        {
            var customers = _billingEngine.Generate("Data/customerTest3.csv", "Data/cdrTest8.csv", "");

            Assert.AreEqual(1, customers.First().Bills.First().CallList.Count);

            Assert.AreEqual(4, customers.First().Bills.First().CallList.First().Charge);

        }

        [TestMethod]//9
        public void Calculate_OffPeak_Billing_Charges_for_Local_Calls_After_Peak_Correctly()
        {
            var customers = _billingEngine.Generate("Data/customerTest3.csv", "Data/cdrTest9.csv", "");

            Assert.AreEqual(1, customers.First().Bills.First().CallList.Count);

            Assert.AreEqual(4, customers.First().Bills.First().CallList.First().Charge);

        }

        [TestMethod]//10
        public void Calculate_Free_Minutes_Correctly()
        {
            var customers = _billingEngine.Generate("Data/customerTest10.csv", "Data/cdrTest3.csv", "");

            Assert.AreEqual(1, customers.First().Bills.First().CallList.Count);

            Assert.AreEqual(2, customers.First().Bills.First().CallList.First().Charge);

        }



        [TestMethod]//11
        public void Calculate_Summery_Correctly()
        {
            var customers = _billingEngine.Generate("Data/customerTest3.csv", "Data/cdrTest11.csv", "");

            Assert.AreEqual(1, customers.First().Bills.Count);
            Bill bill = customers.First().Bills.First();

            Assert.AreEqual(3, bill.CallList.Count);

            Assert.AreEqual(100, bill.Rental);

            Assert.AreEqual(15,bill.TotalCharges);

            Assert.AreEqual(0,bill.TotalDiscount);

            Assert.AreEqual(23, bill.Tax);

            Assert.AreEqual(138, bill.BillAmount);
        }

        [TestMethod]//12
        public void Calculate_Summery_With_Discount_Correctly()
        {
            var customers = _billingEngine.Generate("Data/customerTest3.csv", "Data/cdrTest12.csv", "");

            Assert.AreEqual(1, customers.First().Bills.Count);

            Bill bill = customers.First().Bills.First();

            Assert.AreEqual(3, bill.CallList.Count);

            Assert.AreEqual(100, bill.Rental);

            Assert.AreEqual(1101, bill.TotalCharges);

            Assert.AreEqual(440.4, bill.TotalDiscount);

            Assert.AreEqual(240.2, bill.Tax);

            Assert.AreEqual(1000.8, bill.BillAmount);

        }
    }
}
