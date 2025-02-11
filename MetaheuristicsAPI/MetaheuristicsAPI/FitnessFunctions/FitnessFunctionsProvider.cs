using MetaheuristicsAPI.FitnessFunctions.ZadanieAA;
using MetaheuristicsAPI.Interfaces;
using MetaheuristicsAPI.Schemas;

namespace MetaheuristicsAPI.FitnessFunctions
{
    public class FitnessFunctionsProvider
    {
        public static FitnessFunctionSchema[] GetFitnessfunctionsSchemas()
        {
            return
            [
                new FitnessFunctionSchema("rastrigin", true),
                new FitnessFunctionSchema("rosenbrock", true),
                new FitnessFunctionSchema("sphere", true),
                new FitnessFunctionSchema("beale", false, [2]),
                new FitnessFunctionSchema("bunkinn6", false, [2]),
                new FitnessFunctionSchema("himmelblau", false, [2]),
                new FitnessFunctionSchema("tsfde fractional boundary", false, [7]),
                new FitnessFunctionSchema("zadanie aa", false, [3])
            ];
        }

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
                case "tsfde fractional boundary":
                    return new TSFDE.TSFDE_fractional_boundary().fintnessFunction;
                case "zadanie aa":
                    return new ObjectiveFunction().FunkcjaCelu.Wartosc;
                default:
                    return null;
            }
        }

        public static double[][]? GetDomain(string functionName, int dim)
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
                case "tsfde fractional boundary":
                    return new TSFDE.TSFDE_fractional_boundary().fitnessFunctionDomain();
                case "zadanie aa":
                    return new ObjectiveFunction().FunkcjaCelu.Domena();
                default:
                    return null;
            }
        }
    }
}
