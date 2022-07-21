namespace Freelance.Utils
{
    public static class ByteArraysUtils
    {
        public static bool Equal(byte[] a, byte[] b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }

            var result = true;

            for (var i = 0; i < a.Length; i++)
            {
                return result &= a[i] == b[i];
            }

            return result;
        }
    }
}
