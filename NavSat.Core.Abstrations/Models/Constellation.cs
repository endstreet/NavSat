using System;

namespace NavSat.Core.Abstrations.Models
{
    public class Constellation : IComparable
    {

        private readonly string _system;
        private readonly int _rank;
        private readonly char _prefix;
        private readonly int _minSatId;
        private readonly int _maxSatId;

        public Constellation() { }

        public Constellation(string system, int rank, char prefix, int minSatId, int maxSatId)
        {
            this._system = system;
            this._rank = rank;
            this._prefix = prefix;
            this._minSatId = minSatId;
            this._maxSatId = maxSatId;
        }

        public string Prefix { get { return _prefix.ToString(); } }


        public int MinSatID { get { return _minSatId; } }

        public int MaxSatID { get { return _maxSatId; } }


        #region Overrides

        public override bool Equals(object obj)
        {
            Constellation rhs = obj as Constellation;
            if (rhs == null)
                return false;
            return _system.Equals(rhs._system);
        }

        public override int GetHashCode()
        {
            return _system.GetHashCode();
        }

        public override string ToString()
        {
            return _system;
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            Constellation rhs = obj as Constellation;
            if (rhs == null)
                throw new InvalidOperationException();

            return _rank.CompareTo(rhs._rank);
        }

        #endregion
    }
}
