using MetaheuristicsAPI.Algorithms;
using MetaheuristicsAPI.Interfaces;

namespace MetaheuristicsAPI
{
    public class AlgorithmsProvider
    {
        public static string[] GetAlgorithmNames()
        {
            return new string[] {
                "archimedes optimization",
                "circle search"
            };
        }
 
        public static IOptimizationAlgorithm? GetAlgorithm(string algorithmName, int numberOfObjects, int numberOfIterations)
        {
            switch (algorithmName.ToLower())
            {
                case "aoa":
                case "archimedes optimization":
                    return new ArchimedesOptimization(numberOfObjects, numberOfIterations);
                case "csa":
                case "circle search":
                    return new CircleSearchAlgorithm(numberOfObjects, numberOfIterations);
                default:
                    return null;
            }
        }
    }
}
