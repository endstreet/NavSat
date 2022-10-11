using System;

namespace NavSat.Core.Abstrations.Models {
    public class Satellite : IComparable {

        public Satellite() { }

        public Satellite(int id, int prn, string system) {
            Id = id;
            Prn = prn;
            Constellation = system;
        }

        public int Id { get; private set; }

        public int Prn { get; private set; }

        public string Constellation { get; private set; }

        public string DisplayName {
            get {
                return ToString();
            }
        }

        #region Overrides

        public override bool Equals(object obj) {
            Satellite rhs = obj as Satellite;
            if (rhs == null)
                return false;
            return Id.Equals(rhs.Id);
        }

        public override int GetHashCode() {
            return Id.GetHashCode();
        }

        public override string ToString() {
            return string.Format("{0}{1:00}", Constellation, Prn);
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj) {
            Satellite rhs = obj as Satellite;
            if (rhs == null)
                throw new InvalidOperationException();

            if (Constellation == rhs.Constellation)
                return Id.CompareTo(rhs.Id);

            return Constellation.CompareTo(rhs.Constellation);
        }

        #endregion
    }
}
