using BillingMachine.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BillingMachine
{ 
    public class CSVRead: ICSVRead
    {
        private string cusFilePath, cdrFilePath, packageFilePath;

        public void setCSVFils(string cusCSV, string cdrCSV, string pkgCSV)
        {
            if(cusCSV != "")
                cusFilePath = cusCSV;
            if(cdrCSV != "")
                cdrFilePath = cdrCSV;
            if(pkgCSV != "")
                packageFilePath = pkgCSV;
        }

        public CSVRead()
        {
            cusFilePath = "Data/customer.csv";
            cdrFilePath = "Data/cdr.csv";
            packageFilePath = "Data/package.csv";
        }

        //method for read cdr from csv's
        public List<CDR> ReadCDRs()
        {
            List<CDR> cdrs = new List<CDR>();
            //stream reader for read files
            using (var reader = new StreamReader(cdrFilePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    CDR temp = NewCDR(line);
                    //add read cdr to the list
                    if (temp != null)
                        cdrs.Add(temp);
                }
            };

            return cdrs;
        }

        //method for reading packages from csv
        public List<Package> ReadPackages()
        {
            List<Package> packages = new List<Package>();
            try
            {
                //stream reader for read files
                using (var reader = new StreamReader(packageFilePath))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        Package temp = NewPackage(line);
                        //Add customer to the list
                        if (temp != null)
                            packages.Add(temp);
                    }
                };
            }
            //If file not found
            catch (FileNotFoundException ex)
            {
                throw ex;
            }

            return packages;
        }

        //method for reading customer details from csv
        public List<Customer> ReadCustomers(List<Package> packages)
        {
            List<Customer> customers = new List<Customer>();
            if (packages.Count == 0)
                ReadPackages();
            try
            {
                //stream reader for read files
                using (var reader = new StreamReader(cusFilePath))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        Customer temp = NewCustomer(line, packages);
                        //Add customer to the list
                        if (temp != null)
                            customers.Add(temp);
                    }
                };
            }
            //If file not found
            catch (FileNotFoundException ex)
            {
                throw ex;
            }

            return customers;
        }

        //method for creating new customer using csv line
        internal Customer NewCustomer(string line, List<Package> packages)
        {
            try
            {
                //parse values into needed data types
                var values = line.Split(',');
                int.TryParse(values[0], out int id);
                int.TryParse(values[3], out int phExt);
                int.TryParse(values[4], out int phUnq);
                DateTime.TryParse(values[6], out DateTime st);
                Package package = packages.Find(c => c.Name == values[5]);
                PhoneNumber number = new PhoneNumber { Extention = phExt, Unique = phUnq };

                //creating new customer
                Customer customer = new Customer
                {
                    ID = id,
                    FullName = values[1],
                    Number = number,
                    BillingAddress = values[2],
                    PackageType = package,
                    RegisteredDate = st
                };

                return customer;
            }
            //if any error
            catch
            {
                return null;
            }
        }

        //Creating new CDR for the list
        internal CDR NewCDR(string line)
        {
            try
            {
                var values = line.Split(',');

                int.TryParse(values[0], out int id);
                int.TryParse(values[1], out int callerExt);
                int.TryParse(values[2], out int callerUnq);
                int.TryParse(values[3], out int ReciverrExt);
                int.TryParse(values[4], out int ReciverrUnq);
                DateTime.TryParse(values[5], out DateTime stratingTime);
                int.TryParse(values[6], out int duration);
                PhoneNumber calling = new PhoneNumber { Extention = callerExt, Unique = callerUnq };
                PhoneNumber called = new PhoneNumber { Extention = ReciverrExt, Unique = ReciverrUnq };

                CDR cdr = new CDR
                {
                    ID = id,
                    CallingParty = calling,
                    CalledParty = called,
                    StartingTime = stratingTime,
                    CallDuration = duration
                };

                return cdr;
            }
            catch
            {
                return null;
            }
        }

        //Creating new package for the list
        internal Package NewPackage(string line)
        {
            try
            {
                //parse values into needed data types
                var values = line.Split(',');
                double.TryParse(values[1], out double rental);
                BillType billType;
                if (BillType.PerMinute.ToString().Equals(values[2]))
                {
                    billType = BillType.PerMinute;
                }
                else
                {
                    billType = BillType.PerSecond;
                }
                int.TryParse(values[3], out int localPeak);
                int.TryParse(values[4], out int localOffPeak);
                int.TryParse(values[5], out int longPeak);
                int.TryParse(values[6], out int longOffPeak);
                var startTepmString = values[7].Split('-');
                var endTepmString = values[8].Split('-');
                int.TryParse(startTepmString[0], out int startHour);
                int.TryParse(startTepmString[1], out int startMinute);
                int.TryParse(startTepmString[2], out int startSecond);
                int.TryParse(endTepmString[0], out int endHour);
                int.TryParse(endTepmString[1], out int endMinute);
                int.TryParse(endTepmString[2], out int endSecond);
                TimeSpan peakStartTime = new TimeSpan(startHour, startMinute, startSecond);
                TimeSpan peakEndTime = new TimeSpan(endHour, endMinute, endSecond);

                //creating new package
                Package package = new Package
                {
                    Name = values[0],
                    MonthlyRental = rental,
                    BillingType = billType,
                    ChargeLocalPeak = localPeak,
                    ChargeLocalOffPeak = localOffPeak,
                    ChargeLongPeak = longPeak,
                    ChargeLongOffPeak = longOffPeak,
                    PeakStartTime = peakStartTime,
                    PeakEndTime = peakEndTime
                };

                return package;
            }
            //if any error
            catch
            {
                return null;
            }
        }
    }
}
