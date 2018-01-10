using BillingMachine.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingMachine
{
    public interface ICalculations
    {
        double CalculateChargeSecond(CDR item, Package package);
        double CalculateChargeMinite(CDR item, Package package);
        int IsPeakCall(DateTime start, DateTime end);
        double GenaratineCallCharge(double callCharge, float peak, float offPeak, int duration, DateTime startTime, DateTime endTime, bool isPerMin);
    }
}
