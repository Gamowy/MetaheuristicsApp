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
        public double DefaultValue { get; set; } = 0.0;
        public double BestValue { get; set; } = 0.0;

        public ParamInfo(string name, string description, double lb, double ub, double defaultValue, double bestValue)
        {
            this.Name = name;
            this.Description = description;
            this.LowerBoundary = lb;
            this.UpperBoundary = ub;
            this.DefaultValue = defaultValue;
            this.BestValue = bestValue;
        }

        public ParamInfo() { }
    }
}