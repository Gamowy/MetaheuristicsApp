namespace MetaheuristicsAPI.Interfaces
{
    public delegate double fitnessFunction(params double[] arg);

    // opis pojedynczego parametru algorytmu , wartość jest zmienną typu double
    public class ParamInfo
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public double LowerBoundary { get; set; }
        public double UpperBoundary { get; set; }


        public ParamInfo(string name, string description, double lb = 0.0d, double ub = 0.0d)
        {
            Name = name;
            Description = description;
            LowerBoundary = lb;
            UpperBoundary = ub;
        }
    }
}