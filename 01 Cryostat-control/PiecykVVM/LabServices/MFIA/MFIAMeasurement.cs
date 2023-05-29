using System;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

namespace LabServices.MFIA
{
    public struct MFIAMeasurement
    {
        /// <summary>Częstotliwość z którą był wykonywany pomiar</summary>
        public double[] Freq { get; init; }
        /// <summary>Wartość bezwzględna impedancji</summary>
        public double[] ABS { get; init; }
        /// <summary>Część urojona impedancji</summary>
        public double[] Im { get; init; }
        /// <summary>Część rzeczywista impedancji</summary>
        public double[] Re { get; init; }
        /// <summary>Faza impedancji</summary>
        public double[] Phase { get; init; }
        /// <summary>Wartość tangensa delta -> Re/Im</summary>
        public double[] TanDelta { get; init; }
        /// <summary>Czas zakończenia pomiaru</summary>
        public DateTime TimeStamp { get; init; }
        /// <summary>Ilość próbek</summary>
        public int Length { get; init; }
        /// <summary>Temperatura w której wykonano pomiar</summary>
        public double Temperature { get; init; }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != this.GetType())
                return false;

            // Porównanie zawartości
            MFIAMeasurement tmp = (MFIAMeasurement)obj;
            if (!Enumerable.SequenceEqual(tmp.Freq, Freq))
                return false;
            if (!Enumerable.SequenceEqual(tmp.ABS, ABS))
                return false;
            if (!Enumerable.SequenceEqual(tmp.Im, Im))
                return false;
            if (!Enumerable.SequenceEqual(tmp.Re, Re))
                return false;
            if (!Enumerable.SequenceEqual(tmp.Phase, Phase))
                return false;
            if (!Enumerable.SequenceEqual(tmp.TanDelta, TanDelta))
                return false;
            if (!tmp.TimeStamp.Equals(TimeStamp))
                return false;
            if (tmp.Length != Length)
                return false;
            if (tmp.Temperature != Temperature)
                return false;

            // Wartości równe
            return true;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                Freq,
                ABS,
                Im,
                Re,
                Phase,
                TanDelta,
                TimeStamp,
                Temperature
                );
        }
    }
}
