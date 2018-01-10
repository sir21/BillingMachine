using System;
using System.Collections.Generic;
using System.Text;
using BillingMachine.Entity;

namespace BillingMachine
{
    public class Calculations: ICalculations
    {

        public Calculations()
        {
        }

        public double GenaratineCallCharge(double callCharge, Package package, int duration, DateTime startTime, DateTime endTime, bool isLong, bool isPerMin)
        {
            double peak, offPeak;
            if (isLong)
            {
                peak = package.ChargeLongPeak;
                offPeak = package.ChargeLongOffPeak;
            }
            else
            {
                peak = package.ChargeLocalPeak;
                offPeak = package.ChargeLocalOffPeak;
            }
            // check call duration is more than 1 day
            if ((endTime - startTime).Days > 0)
            {
                callCharge = (12 * 60 * peak + 12 * 60 * offPeak) * (endTime - startTime).Days;
                startTime = startTime.AddDays((endTime - startTime).Days);
                duration -= 86400;
            }

            /*
            In here if 1 then it's peak
            2 - it is a off - peak call
            3 - it satrt in peak and end in off peak
            4 - it start in off peak and end in peak
            5 - it start in peek and end in peak after 12 hours
            6 - it start in off - peek and end in off - peak after 12 hours
            */

            if (IsPeakCall(startTime, endTime, package) == 1)
            {
                if (isPerMin)
                    return callCharge + CalculateChargeForCallperMin(duration, peak);
                else
                    return callCharge + CalculateChargeForCallperSecond(duration, peak);
            }
            else if (IsPeakCall(startTime, endTime, package) == 2)
                if (isPerMin)
                    return callCharge + CalculateChargeForCallperMin(duration, offPeak);
                else
                    return callCharge + CalculateChargeForCallperSecond(duration, offPeak);
            else if (IsPeakCall(startTime, endTime, package) == 3)
            {
                /*
                 * first calculate the peak time period
                 * then calculate the off-peak time period
                 * after that calculate charges for both separately
                 */
                if (isPerMin)
                {
                    double peakTime = MathF.Ceiling(Convert.ToSingle((package.PeakEndTime - startTime.TimeOfDay).TotalMinutes));
                    double offPeakTime = MathF.Ceiling(Convert.ToSingle((endTime.TimeOfDay - package.PeakEndTime).TotalMinutes));
                    return callCharge + peakTime * peak + offPeakTime * offPeak;
                }
                else
                {
                    double peakTime = MathF.Ceiling(Convert.ToSingle((package.PeakEndTime - startTime.TimeOfDay).TotalSeconds));
                    double offPeakTime = MathF.Ceiling(Convert.ToSingle((endTime.TimeOfDay - package.PeakEndTime).TotalSeconds));
                    return callCharge + peakTime * peak + offPeakTime * offPeak;
                }
            }
            else if(IsPeakCall(startTime, endTime, package) == 4)
            {
                /*
                 * first calculate the peak time period
                 * then calculate the off-peak time period
                 * after that calculate charges for both separately
                 */
                double peakTime, offPeakTime;
                if (isPerMin)
                {
                    peakTime = MathF.Ceiling(Convert.ToSingle((endTime.TimeOfDay - package.PeakStartTime).TotalMinutes));
                    offPeakTime = MathF.Ceiling(Convert.ToSingle((package.PeakStartTime - startTime.TimeOfDay).TotalMinutes));
                }
                else
                {
                    peakTime = MathF.Ceiling(Convert.ToSingle((endTime.TimeOfDay - package.PeakStartTime).TotalSeconds));
                    offPeakTime = MathF.Ceiling(Convert.ToSingle((package.PeakStartTime - startTime.TimeOfDay).TotalSeconds));
                }
                return callCharge + peakTime * peak + offPeakTime * offPeak;
            }
            else if(IsPeakCall(startTime, endTime, package) == 5)
            {
                /*
                 * If start in peak and end peak and call is more than 12 hours for sure call has been passed through
                 * a off-peak period for 12 hours. 
                 * we can get peak time value and genarate charges
                 */ 
                double peakTime;
                callCharge += offPeak * 12 * 60;
                if (isPerMin)
                {
                    peakTime = MathF.Ceiling(Convert.ToSingle((package.PeakEndTime - startTime.TimeOfDay).TotalMinutes));
                    peakTime += MathF.Ceiling(Convert.ToSingle((endTime.TimeOfDay - package.PeakStartTime).TotalMinutes));
                }
                else
                {
                    peakTime = MathF.Ceiling(Convert.ToSingle((package.PeakEndTime - startTime.TimeOfDay).TotalSeconds));
                    peakTime += MathF.Ceiling(Convert.ToSingle((endTime.TimeOfDay - package.PeakStartTime).TotalSeconds));
                }
                return callCharge + peakTime * peak;
            }
            else
            {
                /*
                 * If start in off-peak and end off-peak and call is more than 12 hours for sure call has been passed through
                 * a peak period for 12 hours. 
                 * we can get off-peak time value and genarate charges
                 */
                double offPeakTime;
                callCharge += peak * 12 * 60;
                if (isPerMin)
                {
                    offPeakTime = MathF.Ceiling(Convert.ToSingle((package.PeakStartTime - startTime.TimeOfDay).TotalMinutes));
                    offPeakTime += MathF.Ceiling(Convert.ToSingle((endTime.TimeOfDay - package.PeakEndTime).TotalMinutes));
                }
                else
                {
                    offPeakTime = MathF.Ceiling(Convert.ToSingle((package.PeakStartTime - startTime.TimeOfDay).TotalSeconds));
                    offPeakTime += MathF.Ceiling(Convert.ToSingle((endTime.TimeOfDay - package.PeakEndTime).TotalSeconds));
                }
                return callCharge + offPeakTime * offPeak;
            }
        }

        //Calculate Chargers for per second call
        public double CalculateChargeSecond(CDR item, Package package)
        {
            DateTime startTime = item.StartingTime;
            int duration = item.CallDuration;
            DateTime endTime = startTime.AddSeconds(duration);

            //Checking call is long or not and call genarate call charge using the genaratineCallCharge method
            if (IsCallLong(item))
            {
                return GenaratineCallCharge(0, package, duration, startTime, endTime, true, false);
            }
            else
            {
                return GenaratineCallCharge(0, package, duration, startTime, endTime, false, false);
            }
        }

        //mathematical calculations for per second call
        internal double CalculateChargeForCallperSecond(int duration, double charge)
        {
            return Convert.ToDouble(duration * (charge/60));
        }

        //Calculate Chargers for per minite call
        public double CalculateChargeMinite(CDR item, Package package)
        {
            DateTime startTime = item.StartingTime;
            int duration = item.CallDuration;
            DateTime endTime = item.StartingTime.AddSeconds(duration);

            //Checking call is long or not and call genarate call charge using the genaratineCallCharge method
            if (IsCallLong(item))
            {
                return GenaratineCallCharge(0, package, duration, startTime, endTime, true, true);
            }
            else
            {
                return GenaratineCallCharge(0, package, duration, startTime, endTime, false, true);
            }
        }

        //mathematical calculations for per minite call
        internal double CalculateChargeForCallperMin(int seconds, double charge)
        {
            return Convert.ToDouble(MathF.Ceiling(Convert.ToSingle(seconds) / 60) * charge);
        }

        //checking whether call is long distance or local by checking extention of caller and called numbers
        internal bool IsCallLong(CDR item)
        {
            if (item.CallingParty.Extention == item.CalledParty.Extention)
                return false;
            else
                return true;
        }

        //check whether call start is peak or not
        internal bool IsStartPeak(TimeSpan time, Package package)
        {
            if (time >= package.PeakStartTime && time < package.PeakEndTime)
                return true;
            else
                return false;
        }

        //check whether call end in peak or not
        internal bool IsEndPeak(TimeSpan time, Package package)
        {
            if (time >= package.PeakStartTime && time < package.PeakEndTime)
                return true;
            else
                return false;
        }

        /*
        This method will return 1 if it is a peak call
        it will return 2 if it is a off-peak call
        it will return 3 if it satrt in peak and end in off peak
        it will return 4 if it start in off peak and end in peak
        it will return 5 if it start in peek and end in peak after 12 hours
        it will return 6 if it start in off-peek and end in off-peak after 12 hours
        */
        public int IsPeakCall(DateTime start, DateTime end, Package package)
        {
            if (IsStartPeak(start.TimeOfDay, package))
            {
                if (IsEndPeak(end.TimeOfDay, package))
                {
                    if ((end - start).Hours >= 12)
                        return 5;
                    else
                        return 1;
                }
                else
                    return 3;
            }
            else
            {
                if (IsEndPeak(end.TimeOfDay, package))
                    return 4;
                else
                {
                    if ((end - start).Hours >= 12)
                        return 6;
                    else
                        return 2;
                }
            }
        }
    }
}
