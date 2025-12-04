namespace Itaiji.Text
{
    internal class stringPolyfill
    {
        // This is only intended to be used by char.ToString.
        // It is necessary to put the code in this class instead of Char, since _firstChar is a private member.
        // Making _firstChar internal would be dangerous since it would make it much easier to break String's immutability.
        internal static string CreateFromChar(char c)
        {
            //string result = FastAllocateString(1);
            //result._firstChar = c;
            //return result;

            return c.ToString();
        }

        internal static string CreateFromChar(char c1, char c2)
        {
            //string result = FastAllocateString(2);
            //result._firstChar = c1;
            //Unsafe.Add(ref result._firstChar, 1) = c2;
            //return result;
            return new string([c1, c2]);
        }
    }
}
