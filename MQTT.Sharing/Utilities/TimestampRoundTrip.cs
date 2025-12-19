using System;
using System.Collections.Generic;
using System.Text;

namespace MQTT.Sharing.Utilities
{
    public static class TimestampRoundTrip
    {
        public static string GetTimeStamp()
        {
            DateTime nowUtc = DateTime.UtcNow;
            return nowUtc.ToString("O");
        }
    }
}
