using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Globalization;

namespace BazaDanychElementow.GlobalFunctions
{
    class CheckFunctions
    {
        /// <summary>
        /// Funkcja słuąca do sprawdzania poprawności parametru
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool ValidateParameterType(string value, string type)
        {
            int intNumericValue;
            double floatNumericValue;
            switch (type)
            {
                case "int":
                    if (!int.TryParse(value, out intNumericValue)) return false;
                    else return true;
                case "uint":
                    if (!int.TryParse(value, out intNumericValue)) return false;
                    else if (intNumericValue < 0) return false;
                    else return true;
                case "float":
                    value = value.Replace(',', '.');
                    if (!double.TryParse(value, out floatNumericValue)) return false;
                    else return true;
                case "string":
                    return true;
                default:
                    Console.WriteLine("Default test :(");
                    return false;
            }
        }

        /// <summary>
        /// Funkcja sprawdzająca czy element powinien przejść przez filtr.
        /// </summary>
        /// <param name="Element">Sprawdzany przez filtr element</param>
        /// <param name="ValueFilter">Filtr głównego parametru: ["<", "<=", "=", ">=", ">", null] (null - brak filtra)</param>
        /// <param name="CountFilter">Filtr ilościowy          : ["<", "<=", "=", ">=", ">", null] (null - brak filtra)</param>
        /// <param name="ValueFilterContent">Wartość porównawcza dla filtra parametru głównego</param>
        /// <param name="CountFilterContent">Wartość porównawcza dla filtra ilościowego</param>
        /// <returns>Czy element przeszedł.</returns>
        public static bool ElementFilter(DataTemplates.ElementTemplate Element, string ValueFilter = null, string CountFilter = null, string ValueFilterContent = " ", int CountFilterContent = 0)
        {
            bool valueFilterFlag;
            bool countFilterFlag;

            int mainParameterCompareOut = string.Compare(Element.MainParametrValue, ValueFilterContent);
            int countCompareOut = Element.Count.CompareTo(CountFilterContent);

            // Kontrola filtru wartości
            if(ValueFilter == null)
            {
                valueFilterFlag = true;
            }
            else if(ValueFilter.Equals("<"))
            {
                valueFilterFlag = mainParameterCompareOut == -1;
            }
            else if (ValueFilter.Equals("<="))
            {
                valueFilterFlag = (mainParameterCompareOut == -1) || (mainParameterCompareOut == 0);
            }
            else if (ValueFilter.Equals("="))
            {
                valueFilterFlag = mainParameterCompareOut == 0;
            }
            else if (ValueFilter.Equals(">="))
            {
                valueFilterFlag = (mainParameterCompareOut == 1) || (mainParameterCompareOut == 0);
            }
            else if (ValueFilter.Equals(">"))
            {
                valueFilterFlag = mainParameterCompareOut == 1;
            }
            else
            {
                // Zakładamy błąd
                valueFilterFlag = false;
            }

            // Kontrola filtru wartości
            if(CountFilter == null)
            {
                countFilterFlag = true;
            }
            else if (CountFilter.Equals("<"))
            {
                countFilterFlag = countCompareOut == -1;
            }
            else if (CountFilter.Equals("<="))
            {
                countFilterFlag = (countCompareOut == -1) || (countCompareOut == 0);
            }
            else if (CountFilter.Equals("="))
            {
                countFilterFlag = countCompareOut == 0;
            }
            else if (CountFilter.Equals(">="))
            {
                countFilterFlag = (countCompareOut == 1) || (countCompareOut == 0);
            }
            else if (CountFilter.Equals(">"))
            {
                countFilterFlag = countCompareOut == 1;
            }
            else
            {
                // Zakładamy błąd
                countFilterFlag = false;
            }

            // Zwracanie wartości wynikowej
            return valueFilterFlag && countFilterFlag;
        }
    }
}
