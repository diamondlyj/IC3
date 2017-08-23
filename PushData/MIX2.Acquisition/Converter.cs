using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MIX2.Acquisition
{
    public class Converter
    {
        public static string ToString(object val)
        {
            string outVal = string.Empty;

            Type t = val.GetType();

            //Console.WriteLine(t.FullName);

            switch (t.FullName)
            {
                case "System.Byte[]":
                    Byte[] byteVal = (byte[])val;

                    for (int i = 0; i < byteVal.Length; i++)
                    {
                        if (i > 0)
                            outVal += ":";

                        outVal += byteVal[i].ToString("x2");
                    }

                    break;

                default:
                    outVal = val.ToString();
                    break;
            }

            return outVal;
        }
    }
}
