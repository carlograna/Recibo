using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReceiptExport
{
    public static class StringExtension
    {
        public static string ToSafeString(this string str)
        {
            if (str == null)
            {
                return string.Empty;
            }
            else
            {
                return str.ToString();
            }
        }
    }
}
