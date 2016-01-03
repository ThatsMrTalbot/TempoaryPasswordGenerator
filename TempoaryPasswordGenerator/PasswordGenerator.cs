using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace TempoaryPasswordGenerator
{
    public class PasswordGenerator
    {
        // Password lifetime
        private TimeSpan Lifetime;

        // Secret used to encode passwords
        private byte[] Secret;

        // Encoder to turn the offset to a string, in a larger project this would be an interface
        private LongEncode Encoder;

        // Provider of current time, used so that the time can be mocked for testing
        TimeProvider Time;

        public PasswordGenerator(TimeSpan lifetime, byte[] secret)
        {
            Secret = secret;
            Lifetime = lifetime;
            Encoder = new LongEncode(secret);
            Time = new UtcTimeProvider();
        }

        public PasswordGenerator(TimeSpan lifetime, byte[] secret, TimeProvider timeProvider)
        {
            Secret = secret;
            Lifetime = lifetime;
            Encoder = new LongEncode(secret);
            Time = timeProvider;
        }

        // Calculate password expiry based on the count and offset
        private DateTime Expire(long count, long offset)
        {
            return new DateTime((count * Lifetime.Ticks) + offset);
        }

        // Calculate itteration count from datetime
        private long Count(DateTime expire)
        {
            return expire.Ticks / Lifetime.Ticks;
        }

        // Calculate offset from itteration count
        private long Offset(DateTime expire)
        {
            var trauncate = Lifetime.Ticks * Count(expire);
            return expire.Ticks - trauncate;
        }

        // Generate secret from user id and offset
        private byte[] GetSecret(string uid, long offset)
        {
            var byts = string.Format("{0}:{1}:{2}", uid, offset.ToString(), Secret).Select(x => (byte)x).ToArray();
            return SHA1.Create().ComputeHash(byts);
        }

        // Generate TOTP token (https://tools.ietf.org/html/rfc6238)
        public string Token(string uid, long count, long offset)
        {
            var secret = GetSecret(uid, offset);
            
            var countByts = BitConverter.GetBytes(count);

            var hmacsha1 = new HMACSHA1(secret);
            var hash = hmacsha1.ComputeHash(countByts);

            var o = (hash.Last() & 0xF);
            var t = hash.Skip(o).Take(4).ToArray();
            var i = BitConverter.ToInt32(t, 0) & 0x7FFFFFFF;

            var digits = i.ToString().ToCharArray().Select(x => (int)Char.GetNumericValue(x)).ToArray();
            var lowest = digits.OrderBy(x => x).Take(6);
            var code = digits.Where(x => lowest.Contains(x)).Take(6).Select(x => x.ToString()).ToArray();
            
            return string.Join("", code);
        }

        // Combine token and offset 
        private string CombineTokenAndOffset(string token, long offset)
        {
            var ofssetEncode = Encoder.Encode(offset);
            var len = Math.Max(ofssetEncode.Length, token.Length);

            ofssetEncode = ofssetEncode.PadLeft(len, Encoder.Zero);
            token = token.PadLeft(len, '0');
            var sb = new StringBuilder();

            for(var i = 0; i < len; i++)
            {
                sb.Append(token[i]);
                sb.Append(ofssetEncode[i]);
            }

            return sb.ToString();
        }

        // Extract offset from generated password
        private long ExtractOffset(string password)
        {
            var sb = new StringBuilder();

            for (var i = 1; i < password.Length; i+=2)
            {
                sb.Append(password[i]);
            }
            return Encoder.Decode(sb.ToString());
        }

        // Extract token from generated password
        private string ExtractToken(string password)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < password.Length; i+=2)
            {
                sb.Append(password[i]);
            }
            return sb.ToString().TrimStart(new [] { '0' }); ;
        }

        // Generate tempoary password for uid
        public string Generate(string uid)
        {
            var now = Time.Current;
            var expire = now.Add(Lifetime);
            var count = Count(expire);
            var offset = Offset(expire);
            var token = Token(uid, count, offset);


            return CombineTokenAndOffset(token, offset);
        }

        // Validate password against UID
        public bool Validate(string uid, string password)
        {
            var expected = ExtractToken(password);
            var offset = ExtractOffset(password);
            var count = Count(Time.Current);
                
            // The password may fall into the current or next itteration count
            for(var i = count; i <= (count + 1); i++)
            {
                var token = Token(uid, i, offset);
                if (token == expected)
                {
                    var expire = Expire(i, offset);
                    var now = Time.Current;
                    return expire >= now;
                }
            }

            return false;
        }
    }
}
