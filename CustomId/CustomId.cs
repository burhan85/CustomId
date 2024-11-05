namespace CustomIdGeneration
{
    public class CustomId : IEquatable<CustomId>
    {
        public static readonly CustomId Empty = new(0, 0, 0, 0);

        readonly int _a;
        readonly int _b;
        readonly int _c;
        readonly int _d;

        public CustomId(int a, int b, int c, int d)
        {
            _a = a;
            _b = b;
            _c = c;
            _d = d;
        }

        public bool Equals(CustomId other)
        {
            return other._a == _a && other._b == _b && other._c == _c && other._d == _d;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (obj.GetType() != typeof(CustomId))
                return false;
            return Equals((CustomId)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_a, _b, _c, _d);
        }

        public override string ToString()
        {
            var bytes = new byte[16];

            bytes[15] = (byte)(_b >> 16);
            bytes[14] = (byte)(_b >> 24);
            bytes[13] = (byte)_a;
            bytes[12] = (byte)(_a >> 8);
            bytes[11] = (byte)(_a >> 16);
            bytes[10] = (byte)(_a >> 24);
            bytes[9] = (byte)_b;
            bytes[8] = (byte)(_b >> 8);
            bytes[7] = (byte)(_c >> 16);
            bytes[6] = (byte)(_c >> 24);
            bytes[5] = (byte)_c;
            bytes[4] = (byte)(_c >> 8);
            bytes[3] = (byte)(_d >> 8);
            bytes[2] = (byte)_d;
            bytes[1] = (byte)(_d >> 16);
            bytes[0] = (byte)(_d >> 24);

            return Convert.ToHexString(bytes);
        }

        public static bool operator ==(in CustomId left, in CustomId right)
        {
            return left._a == right._a && left._b == right._b && left._c == right._c && left._d == right._d;
        }

        public static bool operator !=(in CustomId left, in CustomId right)
        {
            return !(left == right);
        }
    }
}
