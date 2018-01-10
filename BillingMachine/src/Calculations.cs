using System;
using System.Collections.Generic;
using System.Text;
using BillingMachine.Entity;

namespace BillingMachine
{
    public class Calculations: ICalculations
    {
        //Peak start and end
        private TimeSpan _peakStart, _peakEnd;

        public Calculations()
        {
            _peakStart = new TimeSpan(8, 0, 0);
            _peakEnd = new TimeSpan(20, 0, 0);
        }

        public double GenaratineCallCharge(double callCharge, float peak, float offPeak, int duration, DateTime startTime, DateTime endTime, bool isPerMin)
        {
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

            if (IsPeakCall(startTime, endTime) == 1)
            {
                if (isPerMin)
                    return callCharge + CalculateChargeForCallperMin(duration, peak);
                else
                    return callCharge + CalculateChargeForCallperSecond(duration, peak);
            }
            else if (IsPeakCall(startTime, endTime) == 2)
                if (isPerMin)
                    return callCharge + CalculateChargeForCallperMin(duration, offPeak);
                else
                    return callCharge + CalculateChargeForCallperSecond(duration, offPeak);
            else if (IsPeakCall(startTime, endTime) == 3)
            {
                /*
                 * first calculate the peak time period
                 * then calculate the off-peak time period
                 * after that calculate charges for both separately
                 */
                if (isPerMin)
                {
                    double peakTime = MathF.Ceiling(Convert.ToSingle((_peakEnd - startTime.TimeOfDay).TotalMinutes));
                    double offPeakTime = MathF.Ceiling(Convert.ToSingle((endTime.TimeOfDay - _peakEnd).TotalMinutes));
                    return callCharge + peakTime * peak + offPeakTime * offPeak;
                }
                else
                {
                    double peakTime = MathF.Ceiling(Convert.ToSingle((_peakEnd - startTime.TimeOfDay).TotalSeconds));
                    double offPeakTime = MathF.Ceiling(Convert.ToSingle((endTime.TimeOfDay - _peakEnd).TotalSeconds));
                    return callCharge + peakTime * peak + offPeakTime * offPeak;
                }
            }
            else if(IsPeakCall(startTime, endTime) == 4)
            {
                /*
                 * first calculate the peak time period
                 * then calculate the off-peak time period
                 * after that calculate charges for both separately
                 */
                double peakTime, offPeakTime;
                if (isPerMin)
                {
                    peakTime = MathF.Ceiling(Convert.ToSingle((endTime.TimeOfDay - _peakStart).TotalMinutes));
                    offPeakTime = MathF.Ceiling(Convert.ToSingle((_peakStart - startTime.TimeOfDay).TotalMinutes));
                }
                else
                {
                    peakTime = MathF.Ceiling(Convert.ToSingle((endTime.TimeOfDay - _peakStart).TotalSeconds));
                    offPeakTime = MathF.Ceiling(Convert.ToSingle((_peakStart - startTime.TimeOfDay).TotalSeconds));
                }
                return callCharge + peakTime * peak + offPeakTime * offPeak;
            }
            else if(IsPeakCall(startTime, endTime) == 5)
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
                    peakTime = MathF.Ceiling(Convert.ToSingle((_peakEnd - startTime.TimeOfDay).TotalMinutes));
                    peakTime += MathF.Ceiling(Convert.ToSingle((endTime.TimeOfDay - _peakStart).TotalMinutes));
                }
                else
                {
                    peakTime = MathF.Ceiling(Convert.ToSingle((_peakEnd - startTime.TimeOfDay).TotalSeconds));
                    peakTime += MathF.Ceiling(Convert.ToSingle((endTime.TimeOfDay - _peakStart).TotalSeconds));
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
                    offPeakTime = MathF.Ceiling(Convert.ToSingle((_peakStart - startTime.TimeOfDay).TotalMinutes));
                    offPeakTime += MathF.Ceiling(Convert.ToSingle((endTime.TimeOfDay - _peakEnd).TotalMinutes));
                }
                else
                {
                    offPeakTime = MathF.Ceiling(Convert.ToSingle((_peakStart - startTime.TimeOfDay).TotalSeconds));
                    offPeakTime += MathF.Ceiling(Convert.ToSingle((endTime.TimeOfDay - _peakEnd).TotalSeconds));
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
                return GenaratineCallCharge(0, Convert.ToSingle(package.ChargeLongPeak), Convert.ToSingle(package.ChargeLongOffPeak), duration, startTime, endTime, false);
            }
            else
            {
                return GenaratineCallCharge(0, Convert.ToSingle(package.ChargeLocalPeak), Convert.ToSingle(package.ChargeLocalOffPeak), duration, startTime, endTime, false);
            }
        }

        //mathematical calculations for per second call
        internal double CalculateChargeForCallperSecond(int duration, float charge)
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
                return GenaratineCallCharge(0, Convert.ToSingle(package.ChargeLongPeak), Convert.ToSingle(package.ChargeLongOffPeak), duration, startTime, endTime, true);
            }
            else
            {
                return GenaratineCallCharge(0, Convert.ToSingle(package.ChargeLocalPeak), Convert.ToSingle(package.ChargeLocalOffPeak), duration, startTime, endTime, true);
            }
        }

        //mathematical calculations for per minite call
        internal double CalculateChargeForCallperMin(int seconds, float charge)
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
        internal bool IsStartPeak(TimeSpan time)
        {
            if (time >= _peakStart && time < _peakEnd)
                return true;
            else
                return false;
        }

        //check whether call end in peak or not
        internal bool IsEndPeak(TimeSpan time)
        {
            if (time >= _peakStart && time < _peakEnd)
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
        public int IsPeakCall(DateTime start, DateTime end)
        {
            if (IsStartPeak(start.TimeOfDay))
            {
                if (IsEndPeak(end.TimeOfDay))
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
                if (IsEndPeak(end.TimeOfDay))
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
