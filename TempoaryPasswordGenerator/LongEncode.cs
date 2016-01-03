using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempoaryPasswordGenerator
{
    class LongEncode
    {
        private char[] Values;

        public LongEncode(byte[] secret)
        {
            var r = new Random(secret.GetHashCode());
            Values = "1234567890abcdefghijklmnopqrstuvwxyz".OrderBy(x => r.Next()).ToArray();
        }

        public char Zero
        {
            get
            {
                return Values[0];
            }
        }

        public string Encode(long val)
        {
            var result = new Stack<char>();
            while (val != 0)
            {
                result.Push(Values[val % 36]);
                val /= 36;
            }
            return new string(result.ToArray());
        }

        public long Decode(string val)
        {
            var reversed = val.ToCharArray().Reverse();
            long total = 0;

            int pos = 0;
            foreach (char c in reversed)
            {
                total += Array.IndexOf(Values, c) * (long)Math.Pow(36, pos);
                pos++;
            }

            return total;
        }
    }
}
