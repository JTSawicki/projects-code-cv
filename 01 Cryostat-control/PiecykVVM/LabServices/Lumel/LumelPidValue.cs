using Serilog;
using System;
using System.Diagnostics.CodeAnalysis;

namespace LabServices.Lumel
{
    public readonly struct LumelPidValue
    {
        public ushort ParamP { get; init; }
        public ushort ParamI { get; init; }
        public ushort ParamD { get; init; }

        /// <summary>
        /// Konstruktor wykonuje kontrolę granicznej wartości 9999
        /// Przy jej przekroczeniu ustawia ją jako wartość parametru
        /// </summary>
        /// <param name="paramP"></param>
        /// <param name="paramI"></param>
        /// <param name="paramD"></param>
        public LumelPidValue(ushort paramP, ushort paramI, ushort paramD)
        {
            if(paramP > 9999 || paramI > 9999 || paramD > 9999)
            {
                Log.Error($"LumelPidValue-TriedToSetToBigValue:{{{paramP},{paramI},{paramD}}}");
                paramP = paramP <= 9999 ? paramP : (ushort)9999;
                paramI = paramI <= 9999 ? paramI : (ushort)9999;
                paramD = paramD <= 9999 ? paramD : (ushort)9999;
            }
            ParamP = paramP;
            ParamI = paramI;
            ParamD = paramD;
        }

        public override string ToString() =>
            $"LumelPidValue{{{ParamP},{ParamI},{ParamD}}}";

        public override bool Equals(object? obj)
        {
            return obj is LumelPidValue value &&
                   ParamP == value.ParamP &&
                   ParamI == value.ParamI &&
                   ParamD == value.ParamD;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ParamP, ParamI, ParamD);
        }
    }
}
