using System;
using System.Collections.Generic;
using System.Text;

namespace BillingMachine.Entity
{
    public class CDR
    {
        public int ID { get; set; }
        public PhoneNumber CallingParty { get; set; }
        public PhoneNumber CalledParty { get; set; }
        public DateTime StartingTime { get; set; }
        public int CallDuration { get; set; }
    }
}
