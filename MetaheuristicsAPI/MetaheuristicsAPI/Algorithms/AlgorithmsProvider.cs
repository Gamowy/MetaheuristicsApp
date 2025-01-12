using MetaheuristicsAPI.Algorithms;
using MetaheuristicsAPI.Interfaces;

namespace MetaheuristicsAPI
{
    public class AlgorithmsProvider
    {
        public static IOptimizationAlgorithm? GetAlgorithm(string algorithmName, int numberOfObjects, int numberOfIterations)
        {

            switch (algorithmName.ToLower())
            {
                case "aoa":
                case "archimedes optimization":
                    return new ArchimedesOptimization(numberOfObjects, numberOfIterations);
                default:
                    return null;
            }
        }
    }
}
