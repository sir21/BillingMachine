﻿using BillingMachine.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingMachine
{
    public interface ICalculations
    {
        double CalculateChargeSecond(CDR item, Package package);
        double CalculateChargeMinite(CDR item, Package package);
        int IsPeakCall(DateTime start, DateTime end, Package package);
        double GenaratineCallCharge(double callCharge, Package package, int duration, DateTime startTime, DateTime endTime, bool isLong, bool isPerMin);
        int FreeCallTime(int duration, int timeToReduce);
    }
}
