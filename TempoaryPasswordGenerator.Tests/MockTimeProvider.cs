using System;

namespace TempoaryPasswordGenerator.Tests
{
    public class MockTimeProvider : TimeProvider
    {

        public MockTimeProvider()
        {
            current = DateTime.UtcNow;
        }

        private DateTime current { get; set; }

        public override DateTime Current
        {
            get
            {
                return current;
            }
        }

        public void Set(DateTime time)
        {
            current = time;
        }

    }
}
