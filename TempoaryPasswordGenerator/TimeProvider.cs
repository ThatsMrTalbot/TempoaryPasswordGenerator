using System;

namespace TempoaryPasswordGenerator
{
    public abstract class TimeProvider
    {
        public abstract DateTime Current
        {
            get;
        }
    }

    public class DefaultTimeProvider : TimeProvider
    {
        public override DateTime Current
        {
            get
            {
                return DateTime.Now;
            }
        }
    }

    public class UtcTimeProvider : TimeProvider
    {
        public override DateTime Current
        {
            get
            {
                return DateTime.UtcNow;
            }
        }
    }
}
