using System;
using System.Collections.Generic;
using System.Text;

namespace BillingMachine.Entity
{
    public class Bill
    {
        public int ID { get; set; }
        public PhoneNumber Number { get; set; }
        public double TotalCharges { get; set; }
        public double TotalDiscount { get; set; }
        public double Tax { get; set; }
        public double Rental { get; set; }
        public double BillAmount { get; set; }
        public ICollection<CallDetails> CallList { get; set; }
    }

    public class CallDetails
    {
        public int ID { get; set; }
        public DateTime StartTime { get; set; }
        public int DurationSeconds { get; set; }
        public PhoneNumber Destination { get; set; }
        public double Charge { get; set; }
    }

}
