using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USSD
{
    class USSDSender
    {
        public class Encoder
        {
            #region Interface

            public static string EncodeTo7Bits(string data)
            {
                int sourceLength = data.Length;
                string reversedStr = "";

                for (int i = 0; i < sourceLength; i++)
                {
                    byte value = Convert.ToByte(data[i]);
                    string binary = Convert.ToString(value, 2);
                    binary = binary.PadLeft(7, '0');

                    reversedStr += GetInvertString(binary);
                }

                int totalLength = (reversedStr.Length / 8 + 1) * 8;
                reversedStr = reversedStr.PadRight(totalLength, '0');

                string output = "";

                while (reversedStr.Length > 0)
                {
                    string symbolBinary = reversedStr.Substring(0, 8);
                    symbolBinary = GetInvertString(symbolBinary);

                    reversedStr = reversedStr.Substring(8);

                    byte symbolByte = Convert.ToByte(symbolBinary, 2);
                    string symbolHex = Convert.ToString(symbolByte, 16);
                    symbolHex = symbolHex.PadLeft(2, '0');

                    output += symbolHex.ToUpper();
                }

                return output;
            }

            public static string Decode7bit(string source, int length)
            {
                byte[] bytes = GetInvertBytes(source);

                string binary = string.Empty;

                foreach (byte b in bytes)
                    binary += Convert.ToString(b, 2).PadLeft(8, '0');

                string result = string.Empty;

                binary = GetInvertString(binary);

                binary = binary.PadRight((binary.Length / 7 + 1) * 7, '0');

                while (binary.Length > 0)
                {
                    string symbolByte = binary.Substring(0, binary.Length >= 7 ? 7 : binary.Length);
                    symbolByte = GetInvertString(symbolByte);

                    byte byteResult = Convert.ToByte(symbolByte, 2);

                    result += Convert.ToChar(byteResult);

                    binary = binary.Substring(7);
                }

                return RemoveLB(result);
            }

            #endregion

            #region Private routines

            #region Bytes

            private static byte[] GetInvertBytes(string source)
            {
                byte[] bytes = GetBytes(source);

                Array.Reverse(bytes);

                return bytes;
            }

            private static byte[] GetBytes(string source)
            {
                return GetBytes(source, 16);
            }

            private static byte[] GetBytes(string source, int fromBase)
            {
                List<byte> bytes = new List<byte>();

                for (int i = 0; i < source.Length / 2; i++)
                    bytes.Add(Convert.ToByte(source.Substring(i * 2, 2), fromBase));

                return bytes.ToArray();
            }

            #endregion

            #region Strings

            private static string RemoveLB(string source)
            {
                int index = 0;
                while (index >= 0)
                {
                    index = source.IndexOf('\0');

                    if (index < 0)
                        break;

                    source = source.Remove(index, 1);
                }
                return source;
            }

            private static string GetInvertString(string source)
            {
                string output = "";
                int length = source.Length;

                for (int i = 0; i < length; i++)
                {
                    output += source[length - 1 - i];
                }
                return output;
            }

            #endregion

            #endregion
        }
    }
}
