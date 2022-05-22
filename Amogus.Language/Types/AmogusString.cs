using System.Linq;
using System;

namespace Amogus.Language.Types
{
    public struct AmogusString
    {
        public string Value { get; set; }

        public AmogusString(string value)
        {
            Value = value;
        }

        public static implicit operator AmogusString(string value) => new (value);

        public static explicit operator string(AmogusString value) => value.Value;

        public static AmogusString operator +(AmogusString a, AmogusString b) => $"{a}{b}";

        public static AmogusString operator -(AmogusString a, AmogusString b) => throw new NotSupportedException();

        public static AmogusString operator /(AmogusString a, AmogusString b) => throw new NotSupportedException();

        public static AmogusString operator *(AmogusString a, AmogusString b) => throw new NotSupportedException();

        public static bool operator >=(AmogusString a, AmogusString b) => throw new NotSupportedException();

        public static bool operator <=(AmogusString a, AmogusString b) => throw new NotSupportedException();

        public static bool operator >(AmogusString a, AmogusString b) => throw new NotSupportedException();

        public static bool operator <(AmogusString a, AmogusString b) => throw new NotSupportedException();

        public static bool operator ==(AmogusString a, AmogusString b) => a.Equals(b);

        public static bool operator !=(AmogusString a, AmogusString b) => !a.Equals(b);

        public override string ToString()
        {
            return Value;
        }

        public override bool Equals(object? obj)
        {
            return Value.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
