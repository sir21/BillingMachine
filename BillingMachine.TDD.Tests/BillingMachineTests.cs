using BillingMachine.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace BillingMachine.TDD.Tests
{
    [TestClass]
    public class BillingMachineTests
    {
        private IBillingEngine _sut;
        private ICalculations _calculations;
        private ICSVRead _csvRead;

        [TestInitialize]
        public void Init()
        {
            _sut = new BillingEngine();
            _calculations = new Calculations();
            _csvRead = new CSVRead();
        }

        [TestMethod]
        public void OnFileRead_ShouldReturnCustomerNames()
        {
            //Arrange
            List<string> expected = new List<string> { "John", "Manel", "Jane", "Michell", "Jehan", "Namal" };

            //Act
            List<Customer> outputResult = _csvRead.ReadCustomers(_csvRead.ReadPackages());
            List<string> output = new List<string>();
            foreach(Customer item in outputResult)
            {
                output.Add(item.FullName);
            }

            //Assert
            Assert.AreEqual(expected.Count, output.Count, "Element count is different");
            CollectionAssert.AreEqual(expected, output, "Element are not matching");
        }

        [TestMethod]
        public void OnFileRead_IfValuesAreMissing_ShouldNotAddtoTheList()
        {
            //Arrange
            List<string> expected = new List<string> { "John", "Manel", "Jane", "Michell", "Jehan", "Namal" };

            //Act
            List<Customer> outputResult = _csvRead.ReadCustomers(_csvRead.ReadPackages());
            List<string> output = new List<string>();
            foreach (Customer item in outputResult)
            {
                output.Add(item.FullName);
            }

            //Assert
            Assert.AreEqual(expected.Count, output.Count, "Element count is different");
            CollectionAssert.AreEqual(expected, output, "Elements are not matching");
        }

        [TestMethod]
        public void OnFileRead_ShouldReturnNumberOfCDRs()
        {
            //Arrange
            List<int> expect = new List<int> { 45, 67, 213, 141, 42, 522, 12, 12, 127, 34, 432, 43, 56, 234, 123 };

            //Act
            List<CDR> outputResult = _csvRead.ReadCDRs();
            List<int> output = new List<int>();
            foreach (CDR item in outputResult)
            {
                output.Add(item.CallDuration);
            }

            //Assert
            Assert.AreEqual(expect.Count, output.Count, "Element count is different");
            CollectionAssert.AreEqual(expect, output);
        }

        /*
        [TestMethod]
        public void OnFileRead_IfNoValuesInCustomers_ShouldReturnNullCustomerException()
        {
            //Act & Assert
            try
            {
                List<string> output = _sut.ReadCustomers("NotValid.csv");
                Assert.Fail();
            }
            catch(Exception ex)
            {
                Assert.IsTrue(ex is FileNotFoundException, "wrong exception");
            }            
        }
        

        [TestMethod]
        public void OnChecking_CallLocation_ShouldReturnLocalOrLongCall()
        {
            //Arrange
            List<Boolean> expect = new List<Boolean> { true, true, true, true, true, true, true, true, false, false };

            //Act
            List<Boolean> output = _sut.IsCallLong();

            //Assert
            Assert.AreEqual(expect.Count, output.Count, "Element count is different");
            CollectionAssert.AreEqual(expect, output, "Element are not matching");
        }
        */

        [TestMethod]
        public void OnCalculating_ShouldRteurnCorrespondingCallValuesForProvidedCustomerID()
        {
            //Arrange
            int cutomerID = 1;
            List<double> expect = new List<double> { 4, 10, 16, 12 };

            //Act
            List<double> output = _sut.ChargersCalculate(cutomerID);

            //Assert
            CollectionAssert.AreEqual(expect, output, "Bill is different");
        }

        [TestMethod]
        public void OnCalculating_WhenCSVAreProvided_EachBillHaveToBeCalculate()
        {
            //Arrange
            int expectedCount = 6;
            List<double> expectedBillAmount = new List<double> { 170.4, 168, 170.4, 133.2, 123.44, 380.4 };

            //Act
            List<Customer> output = _sut.Generate();
            List<double> out_billAmount = new List<double>();
            foreach(Customer item in output)
            {
                foreach(Bill subItem in item.Bills)
                {
                    out_billAmount.Add(subItem.BillAmount);
                }
            }

            //Assert
            Assert.AreEqual(expectedCount, output.Count, "Items number is not matchong");
            CollectionAssert.AreEqual(expectedBillAmount, out_billAmount, "Bill amounts are wrong");
        }

        [TestMethod]
        public void OnCalculating_WhenCustomerPhoneNumberIsProvided_ShouldReturnLastBillOfThatCustomer()
        {
            //Arrange
            List<double> expectedBillAmount = new List<double> { 170.4 };
            List<double> outBillAmount = new List<double> ();

            //Act

            Customer output = _sut.GetBills("011-7883223");
            ICollection<Bill> outBills = output.Bills;
            foreach(Bill item in outBills)
            {
                outBillAmount.Add(item.BillAmount);
            }

            //Assert
            Assert.AreEqual(expectedBillAmount.Count, outBillAmount.Count, "Items number is not matchong");
            CollectionAssert.AreEqual(expectedBillAmount, outBillAmount, "Bill amounts are wrong");
        }

        [TestMethod]
        public void OnCalculating_WhenCallTimeChangesFromPeakToOffPeak_CallChargeWillChange()
        {
            //Arrange
            int expectedCount = 6;
            List<double> expectedBillAmount = new List<double> { 170.4, 168, 170.4, 133.2, 123.44, 380.4 };

            //Act
            List<Customer> output = _sut.Generate();
            List<double> out_billAmount = new List<double>();
            foreach (Customer item in output)
            {
                foreach (Bill subItem in item.Bills)
                {
                    out_billAmount.Add(subItem.BillAmount);
                }
            }

            //Assert
            Assert.AreEqual(expectedCount, output.Count, "Items number is not matchong");
            CollectionAssert.AreEqual(expectedBillAmount, out_billAmount, "Bill amounts are wrong");

        }

        [TestMethod]
        public void OnCalculating_WhenCallTimeChangesFromOffPeakToPeak_CallChargeWillChange()
        {
            //Arrange
            double expected = 67;
            DateTime.TryParse("5/07/2017 7:57:34", out DateTime start);
            CDR input = new CDR
            {
                ID = 1,
                CallingParty = new PhoneNumber { Extention = 081, Unique = 9007214 },
                CalledParty = new PhoneNumber { Extention = 011, Unique = 7883223 },
                StartingTime = start,
                CallDuration = 785
            };

            Package package = new Package
            {
                Name = "Package A",
                MonthlyRental = 100,
                BillingType = BillType.PerMinute,
                ChargeLocalPeak = 3,
                ChargeLocalOffPeak = 2,
                ChargeLongPeak = 5,
                ChargeLongOffPeak = 4,
                PeakStartTime = new TimeSpan(8,0,0),
                PeakEndTime = new TimeSpan(20,0,0)
            };

            //Act
            double output = _calculations.CalculateChargeMinite(input, package);

            //Assert
            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void OnCalculating_IfCallIsMoreThanOneDayLong_ShouldReturnChargeForWholeCall()
        {
            //Arrange
            double expected = 6485;
            DateTime.TryParse("5/07/2017 9:57:34", out DateTime start);
            CDR input = new CDR
            {
                ID = 1,
                CallingParty = new PhoneNumber { Extention = 081, Unique = 9007214 },
                CalledParty = new PhoneNumber { Extention = 011, Unique = 7883223 },
                StartingTime = start,
                CallDuration = 86410
            };

            Package package = new Package
            {
                Name = "Package A",
                MonthlyRental = 100,
                BillingType = BillType.PerMinute,
                ChargeLocalPeak = 3,
                ChargeLocalOffPeak = 2,
                ChargeLongPeak = 5,
                ChargeLongOffPeak = 4,
                PeakStartTime = new TimeSpan(8, 0, 0),
                PeakEndTime = new TimeSpan(20, 0, 0)
            };

            //Act
            double output = _calculations.CalculateChargeMinite(input, package);

            //Assert
            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void OnCalculating_IfCallIsMoreThanOneDayLongAndCallIsLocal_ShouldReturnChargeForWholeCall()
        {
            //Arrange
            double expected = 3603;
            DateTime.TryParse("5/07/2017 9:57:34", out DateTime start);
            CDR input = new CDR
            {
                ID = 1,
                CallingParty = new PhoneNumber { Extention = 011, Unique = 6763241 },
                CalledParty = new PhoneNumber { Extention = 011, Unique = 7883223 },
                StartingTime = start,
                CallDuration = 86410
            };

            Package package = new Package
            {
                Name = "Package A",
                MonthlyRental = 100,
                BillingType = BillType.PerMinute,
                ChargeLocalPeak = 3,
                ChargeLocalOffPeak = 2,
                ChargeLongPeak = 5,
                ChargeLongOffPeak = 4,
                PeakStartTime = new TimeSpan(8, 0, 0),
                PeakEndTime = new TimeSpan(20, 0, 0)
            };

            //Act
            double output = _calculations.CalculateChargeMinite(input, package);

            //Assert
            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void OnCalculating_IfPerSecondPackageAvailable_ChargersForThatPackageWillApplyAccordingToThat()
        {
            //Arrange
            int expectedCount = 6;
            List<double> expectedBillAmount = new List<double> { 170.4, 168, 170.4, 133.2, 123.44, 380.4 };

            //Act
            List<Customer> output = _sut.Generate();
            List<double> out_billAmount = new List<double>();
            foreach (Customer item in output)
            {
                foreach (Bill subItem in item.Bills)
                {
                    out_billAmount.Add(subItem.BillAmount);
                }
            }

            //Assert
            Assert.AreEqual(expectedCount, output.Count, "Items number is not matchong");
            CollectionAssert.AreEqual(expectedBillAmount, out_billAmount, "Bill amounts are wrong");
        }

        [TestMethod]
        public void OnIdentifyingCall_IfCallStartInMorningOffPeakAndEndInEveningOffPeak()
        {
            //Arrange
            double expected = 6;
            DateTime input_startTime = new DateTime(2017, 5, 29, 05, 45, 32);
            DateTime input_endTime = new DateTime(2017, 5, 29, 22, 45, 32);
            Package package = new Package
            {
                PeakStartTime = new TimeSpan(8,0,0),
                PeakEndTime = new TimeSpan(20,0,0)
            };

            //Act
            double output = _calculations.IsPeakCall(input_startTime, input_endTime, package);

            //Assert
            Assert.AreEqual(expected, output, "Not matching");
        }

        [TestMethod]
        public void OnCalculating_IfCallStartInMorningOffPeakAndEndInEveningOffPeak()
        {
            //Arrange
            double expected = 4804;
            DateTime input_startTime = new DateTime(2017, 5, 29, 05, 45, 32);
            DateTime input_endTime = new DateTime(2017, 5, 29, 22, 45, 32);
            int input_duration = 61200;
            Package package = new Package
            {
                Name = "Package A",
                MonthlyRental = 100,
                BillingType = BillType.PerMinute,
                ChargeLocalPeak = 3,
                ChargeLocalOffPeak = 2,
                ChargeLongPeak = 5,
                ChargeLongOffPeak = 4,
                PeakStartTime = new TimeSpan(8, 0, 0),
                PeakEndTime = new TimeSpan(20, 0, 0)
            };

            //Act
            double output = _calculations.GenaratineCallCharge(0, package, input_duration, input_startTime, input_endTime, true, true);

            //Assert
            Assert.AreEqual(expected, output, "Not matching");
        }

        [TestMethod]
        public void OnCalculating_IfCallStartInNoonAndEndNextDayBefore24hours()
        {
            //Arrange
            double expected = 6600;
            DateTime input_startTime = new DateTime(2017, 5, 29, 13, 55, 32);
            DateTime input_endTime = new DateTime(2017, 5, 29, 11, 55, 32);
            int input_duration = 79200;
            Package package = new Package
            {
                Name = "Package A",
                MonthlyRental = 100,
                BillingType = BillType.PerMinute,
                ChargeLocalPeak = 3,
                ChargeLocalOffPeak = 2,
                ChargeLongPeak = 5,
                ChargeLongOffPeak = 4,
                PeakStartTime = new TimeSpan(8, 0, 0),
                PeakEndTime = new TimeSpan(20, 0, 0)
            };

            //Act
            double output = _calculations.GenaratineCallCharge(0, package, input_duration, input_startTime, input_endTime, true, true);

            //Assert
            Assert.AreEqual(expected, output, "Not matching");
        }

        [TestMethod]
        public void OnReadCSV_ReturnAllPackageTypesAvailable()
        {
            //Arrange
            List<string> expected = new List<string> { "Package A", "Package B", "Package C", "Package D" };

            //Act
            List<Package> outputResult = _csvRead.ReadPackages();
            List<string> output = new List<string>();
            foreach (Package item in outputResult)
            {
                output.Add(item.Name);
            }

            //Assert
            CollectionAssert.AreEqual(expected, output, "Elements are not matching");
        }

        [TestMethod]
        public void OnPackages_ShouldReturnOwnPeakAndOffPeakTimesForPackageA()
        {
            //Arrange
            TimeSpan expected_startPeak = new TimeSpan(10, 0, 0);
            TimeSpan expected_endPeak = new TimeSpan(18, 0, 0);

            //Act
            List<Package> output_Result = _csvRead.ReadPackages();
            Package output_Package = output_Result.Find(item => item.Name == "Package A");
            TimeSpan output_startPeak = output_Package.PeakStartTime;
            TimeSpan output_endPeak = output_Package.PeakEndTime;

            //Assert
            Assert.AreEqual(expected_startPeak, output_startPeak, "Start times are not matching");
            Assert.AreEqual(expected_endPeak, output_endPeak, "End times are not matching");
        }

        [TestMethod]
        public void OnPackages_ShouldReturnOwnPeakAndOffPeakTimesPackageC()
        {
            //Arrange
            TimeSpan expected_startPeak = new TimeSpan(9, 0, 0);
            TimeSpan expected_endPeak = new TimeSpan(18, 0, 0);

            //Act
            List<Package> output_Result = _csvRead.ReadPackages();
            Package output_Package = output_Result.Find(item => item.Name == "Package C");
            TimeSpan output_startPeak = output_Package.PeakStartTime;
            TimeSpan output_endPeak = output_Package.PeakEndTime;

            //Assert
            Assert.AreEqual(expected_startPeak, output_startPeak, "Start times are not matching");
            Assert.AreEqual(expected_endPeak, output_endPeak, "End times are not matching");
        }

        [TestMethod]
        public void OnCalculating_InPackageB_RemoveChargesForFirstMinute()
        {
            //Arrange
            int input = 430;
            int expected = 370;
            int freeDuration = 60;

            //Act
            int output = _calculations.FreeCallTime(input, freeDuration);

            //Assert
            Assert.AreEqual(expected, output, "Output is wrong");
        }

        [TestMethod]
        public void OnCalculating_InPackageB_ChargesShouldCalculatedWithDiscounts()
        {
            //Arrange
            double expected = 30.55;
            Package package = new Package
            {
                Name = "Package B",
                MonthlyRental = 100,
                BillingType = BillType.PerSecond,
                ChargeLocalPeak = 5,
                ChargeLocalOffPeak = 3,
                ChargeLongPeak = 6,
                ChargeLongOffPeak = 4,
                PeakStartTime = new TimeSpan(8, 0, 0),
                PeakEndTime = new TimeSpan(20, 0, 0)
            };

            //Act
            double output = _calculations.GenaratineCallCharge(0, package, 671, new DateTime(2017, 5, 20, 4, 52, 21), new DateTime(2017, 5, 20, 5, 3, 32), false, false);

            //Assert
            Assert.AreEqual(expected, output, "Output is wrong");
        }

        [TestMethod]
        public void OnCalculating_InPackageC_ChargesShouldCalculatedWithDiscounts()
        {
            //Arrange
            double expected = 22;

            CDR cdr = new CDR
            {
                StartingTime = new DateTime(2017, 5, 24, 13, 52, 21),
                CallDuration = 671,
                CalledParty = new PhoneNumber
                {
                    Extention = 051,
                    Unique = 6742423
                },
                CallingParty = new PhoneNumber
                {
                    Extention = 051,
                    Unique = 2141232
                }
            };

            Package package = new Package
            {
                Name = "Package C",
                MonthlyRental = 300,
                BillingType = BillType.PerMinute,
                ChargeLocalPeak = 2,
                ChargeLocalOffPeak = 1,
                ChargeLongPeak = 3,
                ChargeLongOffPeak = 2,
                PeakStartTime = new TimeSpan(9, 0, 0),
                PeakEndTime = new TimeSpan(18, 0, 0)
            };

            //Act
            double output = _calculations.CalculateChargeMinite(cdr, package);

            //Assert
            Assert.AreEqual(expected, output, "Output is wrong");
        }
    }
}
