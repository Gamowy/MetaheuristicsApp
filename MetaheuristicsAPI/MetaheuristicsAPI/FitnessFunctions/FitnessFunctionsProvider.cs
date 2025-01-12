using MetaheuristicsAPI.Interfaces;

namespace MetaheuristicsAPI.FitnessFunctions
{
    public class FitnessFunctionsProvider
    {
        public static fitnessFunction? GetFitnessFunction(string functionName)
        {
            switch (functionName.ToLower())
            {
                case "rastrigin":
                    return TestFunctions.Rastrigin;
                case "rosenbrock":
                    return TestFunctions.Rosenbrock;
                case "sphere":
                    return TestFunctions.Sphere;
                case "beale":
                    return TestFunctions.Beale;
                case "bunkinn6":
                    return TestFunctions.BunkinN6;
                case "himmelblau":
                    return TestFunctions.Himmelblau;
                default:
                    return null;
            }
        }

        public static double[,]? GetDomain(string functionName, int dim)
        {
            switch (functionName.ToLower())
            {
                case "rastrigin":
                    return TestFunctions.RastriginDomain(dim);
                case "rosenbrock":
                    return TestFunctions.RosenbrockDomain(dim);
                case "sphere":
                    return TestFunctions.SphereDomain(dim);
                case "beale":
                    return TestFunctions.BealeDomain();
                case "bunkinn6":
                    return TestFunctions.BunkinN6Domain();
                case "himmelblau":
                    return TestFunctions.HimmelblauDomain();
                default:
                    return null;
            }
        }
    }
}
