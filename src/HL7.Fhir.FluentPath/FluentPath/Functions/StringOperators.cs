﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hl7.Fhir.FluentPath.Functions
{
    internal static class StringOperators
    {
        public static string FpSubstring(this string me, long start)
        {
            return me.FpSubstring(start, me.Length);
        }

        public static string FpSubstring(this string me, long start, long length)
        {
            if (start < 0 || start >= me.Length) return null;
            length = Math.Min(length, me.Length - start);

            return me.Substring((int)start, (int)length);
        }

        public static IValueProvider FpIndexOf(this string me, string fragment)
        {
            var result = me.IndexOf(fragment);

            if (result == -1)
                return null;
            else
                return new ConstantValue(result);
        }

        public static string FpReplace(this string me, string find, string replace)
        {
            if (find == String.Empty)
            {
                // weird, but as specified:  "abc".replace("","x") = "xaxbxc"
                return replace + String.Join(replace, me.AsEnumerable());
            }
            else
                return me.Replace(find, replace);
        }
    }
}
