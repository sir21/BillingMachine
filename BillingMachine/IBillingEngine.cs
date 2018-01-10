using BillingMachine.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingMachine
{
    public interface IBillingEngine
    {
        List<Customer> Generate();
        List<double> ChargersCalculate(int cutomerID);
        Customer GetBills(string number);
    }
}
