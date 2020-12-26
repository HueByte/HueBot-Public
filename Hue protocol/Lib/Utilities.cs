using System;
using System.Threading.Tasks;
using HueProtocol.Entities;

namespace HueProtocol.Lib
{
    public static class Utilities
    {
        public static Task<bool> ComapreDatesYearly(DateTime firstDate, DateTime secondDate)
        {
            if(firstDate.Day == secondDate.Day && firstDate.Month == secondDate.Month && firstDate.Year == secondDate.Year)
            {
                return Task.FromResult(true);
            }
            else
                return Task.FromResult(false);
        }

        public static Task<bool> ComapreDatesMontly(DateTime firstDate, DateTime secondDate)
        {
            if(firstDate.Day == secondDate.Day && firstDate.Month == secondDate.Month)
            {
                return Task.FromResult(true);
            }
            else
            return Task.FromResult(false);
        }
    }
}