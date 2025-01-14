namespace MetaheuristicsPlatform.Schemas 
{ 
    public delegate double fitnessFunction(params double[] arg);

    // opis pojedynczego parametru algorytmu, wartość jest zmienną typu double
    public class ParamInfo
    {
        public string Name { get; set; } = "Parameter Name";
        public string Description { get; set; } = "Parameter Description";
        public double LowerBoundary { get; set; } = 0.0;
        public double UpperBoundary { get; set; } = double.MaxValue;


        public ParamInfo(string name, string description, double lb = 0.0d, double ub = 0.0d)
        {
            Name = name;
            Description = description;
            LowerBoundary = lb;
            UpperBoundary = ub;
        }

        public ParamInfo() { }
    }
}