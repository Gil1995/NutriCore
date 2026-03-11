using System.Xml.Linq;

namespace NutriCore.src
{
    /// <summary>
    /// Objekt zur Abbildung eines BLS Datenbankeintrags
    /// </summary>
    internal class EssentialData
    {
        private string _blsCode;
        private string _foodName;
        private double _enercj;
        private double _enercc;
        private double _fat;
        private double _cho;
        private double _sugar;
        private double _fibt;
        private double _prot625;
        private double _nacl;

        public EssentialData(string _blsCode, string _foodName, double _enercj, double _enercc, double _fat, double _cho, double _sugar, double _fibt, double _prot625, double _nacl)
        {
            this.BlsCode = _blsCode;
            this.FoodName = _foodName;
            this.Enercj = _enercj;
            this.Enercc = _enercc;
            this.Fat = _fat;
            this.Cho = _cho;
            this.Sugar = _sugar;
            this.Fibt = _fibt;
            this.Prot625 = _prot625;
            this.Nacl = _nacl;
        }

        public string BlsCode { get => _blsCode; set => _blsCode = value; }
        public string FoodName { get => _foodName; set => _foodName = value; }
        public double Enercj { get => _enercj; set => _enercj = value; }
        public double Enercc { get => _enercc; set => _enercc = value; }
        public double Fat { get => _fat; set => _fat = value; }
        public double Cho { get => _cho; set => _cho = value; }
        public double Sugar { get => _sugar; set => _sugar = value; }
        public double Fibt { get => _fibt; set => _fibt = value; }
        public double Prot625 { get => _prot625; set => _prot625 = value; }
        public double Nacl { get => _nacl; set => _nacl = value; }

        public override bool Equals(object? obj)
        {
            if (obj is not EssentialData other)
                return false;

            return BlsCode == other.BlsCode;
        }

        public string Show
        {
            get
            {
                if (FoodName.Length > 25)
                {
                    return FoodName.Substring(0, 25) + "... ";
                }
                return FoodName;
            }
        }

        public override int GetHashCode()
        {
            return BlsCode.GetHashCode();
        }
    }
}
