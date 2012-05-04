﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BirdFeed
{
    public static class Extensions
    {
        public static String ToFriendlyString(this TimeSpan timespan)
        {
            double delta = Math.Abs(timespan.TotalSeconds);

            if (delta < 60)
            {
                return timespan.Seconds == 1 ? "one second ago" : timespan.Seconds + " seconds ago";
            }
            if (delta < 120)
            {
                return "a minute ago";
            }
            if (delta < 2700) // 45 * 60
            {
                return timespan.Minutes + " minutes ago";
            }
            if (delta < 5400) // 90 * 60
            {
                return "an hour ago";
            }
            if (delta < 86400) // 24 * 60 * 60
            {
                return timespan.Hours + " hours ago";
            }
            if (delta < 172800) // 48 * 60 * 60
            {
                return "yesterday";
            }
            if (delta < 2592000) // 30 * 24 * 60 * 60
            {
                return timespan.Days + " days ago";
            }
            if (delta < 31104000) // 12 * 30 * 24 * 60 * 60
            {
                int months = Convert.ToInt32(Math.Floor((double)timespan.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }
            int years = Convert.ToInt32(Math.Floor((double)timespan.Days / 365));
            return years <= 1 ? "one year ago" : years + " years ago";
        }
    }
}
