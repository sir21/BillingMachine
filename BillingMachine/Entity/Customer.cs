using System;
using System.Collections.Generic;
using System.Text;

namespace BillingMachine.Entity
{
    public class Customer
    {
        public int ID { get; set; }
        public string FullName { get; set; }
        public string BillingAddress { get; set; }
        public PhoneNumber Number{ get; set; }
        public Package PackageType { get; set; }
        public DateTime RegisteredDate { get; set; }
        public ICollection<Bill> Bills { get; set; }
    }
}
