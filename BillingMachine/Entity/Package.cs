using System;
using System.Collections.Generic;
using System.Text;

namespace BillingMachine.Entity
{
    public class Package
    {
        public string Name { get; set; }
        public double MonthlyRental { get; set; }
        public BillType BillingType { get; set; }
        public double ChargeLocalPeak { get; set; }
        public double ChargeLocalOffPeak { get; set; }
        public double ChargeLongPeak { get; set; }
        public double ChargeLongOffPeak { get; set; }
        public TimeSpan PeakStartTime { get; set; }
        public TimeSpan PeakEndTime { get; set; }
    }

    public enum BillType
    {
        PerMinute, 
        PerSecond
    }
}
