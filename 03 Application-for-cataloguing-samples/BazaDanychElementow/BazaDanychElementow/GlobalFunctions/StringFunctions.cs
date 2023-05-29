using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaDanychElementow.GlobalFunctions
{
    public static class StringFunctions
    {
        /// <summary>
        /// Funkcja dzieli łańcuchy znaków typu: "element1 element2 element3 element4"
        /// na List(string) {"element1", "element2", "element3", "element4"}
        /// </summary>
        /// <param name="value">Łańcuch wejściowy.</param>
        /// <param name="toRemove">Usuwany podłańcuch.</param>
        /// <returns>Podzielony łańcuch</returns>
        public static List<string> SplitBySpace(string value, string toRemove = null)
        {
            if(toRemove != null)
            {
                value = value.Replace(toRemove, "");
            }
            string[] words = value.Split(' ');

            return new List<string>(words);
        }

        /// <summary>
        /// Funkcja kompresuje string podmieniając białe znaki na znaki nie problematyczne.
        /// spacja -> ﹏
        /// enter  -> ⏎
        /// tab    -> ⭾
        /// </summary>
        /// <param name="value">Kompresowany string.</param>
        /// <returns>String po kompresji.</returns>
        public static string SerializeCompression(string value)
        {
            string output = value;
            output = output.Replace(" ", "﹏");
            output = output.Replace("\n", "⏎");
            output = output.Replace("\t", "⭾");
            return output;
        }

        /// <summary>
        /// Funkcja kompresuje string podmieniając znaki kompresji z powrotem na znaki białe.
        /// ﹏ -> spacja
        /// ⏎ -> enter
        /// ⭾ -> tab
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SerializeDecompression(string value)
        {
            string output = value;
            output = output.Replace("﹏", " ");
            output = output.Replace("⏎", "\n");
            output = output.Replace("⭾", "\t");
            return output;
        }

        /// <summary>
        /// Standaryzacja łączenia typu i jednostki na potrzeby GUI.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static string ConnectTypeAndUnit(string type, string unit)
        {
            return unit + " (" + type + ")";
        }
    }
}
