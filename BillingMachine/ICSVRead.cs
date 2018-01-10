using BillingMachine.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingMachine
{
    public interface ICSVRead
    {
        List<CDR> ReadCDRs();
        List<Package> ReadPackages();
        List<Customer> ReadCustomers(List<Package> packages);
    }
}
