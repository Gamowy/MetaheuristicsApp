namespace MetaheuristicsAPI.FitnessFunctions
{
    public class TestFunctions
    {
        // -5.12 <= x_n <= 5.12, Dim = 2,5,10,30 
        public static double[][] RastriginDomain(int dim)
        {
            double[][] domain = new double[dim][];
            for (int i = 0; i < dim; i++)
            {
                domain[i] = new double[2];
                domain[i][0] = -5.12;
                domain[i][1] = 5.12;
            }
            return domain;
        }

        public static double Rastrigin(double[] x)
        {
            double y = 0.0;
            for (int i = 0; i < x.Length; i++)
            {
                y += x[i] * x[i] - 10 * Math.Cos(2 * Math.PI * x[i]);
            }
            y += 10 * x.Length;
            return y;
        }

        // -inf <= x_n <= inf, Dim = 2,5,10,30
        public static double[][] RosenbrockDomain(int dim)
        {
            double[][] domain = new double[dim][];
            for (int i = 0; i < dim; i++)
            {
                domain[i] = new double[2];
                domain[i][0] = -2;
                domain[i][1] = 2;
            }
            return domain;
        }

        public static double Rosenbrock(double[] x)
        {
            double y = 0.0;
            for (int i = 0; i < x.Length - 1; i++)
            {
                y += 100 * Math.Pow(x[i + 1] - x[i] * x[i], 2) + Math.Pow(1 - x[i], 2);
            }
            return y;
        }

        // -inf <= x_n <= inf, Dim = 2,5,10,30,50
        public static double[][] SphereDomain(int dim)
        {
            double[][] domain = new double[dim][];
            for (int i = 0; i < dim; i++)
            {
                domain[i] = new double[2];
                domain[i][0] = -2;
                domain[i][1] = 2;
            }
            return domain;
        }

        public static double Sphere(double[] x)
        {
            double y = 0.0;
            for (int i = 0; i < x.Length; i++)
            {
                y += x[i] * x[i];
            }
            return y;
        }

        // -4.5 <= x,y <= 4.5, Dim = 2
        public static double[][] BealeDomain()
        {
            double[][] domain = new double[2][];
            for (int i = 0; i < 2; i++)
            {
                domain[i] = new double[2];
                domain[i][0] = -4.5;
                domain[i][1] = 4.5;
            }
            return domain;
        }

        public static double Beale(double[] x)
        {
            return Math.Pow(1.5 - x[0] + x[0] * x[1], 2) + Math.Pow(2.25 - x[0] + x[0] * x[1] * x[1], 2) + Math.Pow(2.625 - x[0] + x[0] * x[1] * x[1] * x[1], 2);
        }

        // -15 <= x <= -5, -3 <= y <= 3 Dim = 2
        public static double[][] BunkinN6Domain()
        {
            double[][] domain = new double[2][];
            domain[0] = new double[2];
            domain[1] = new double[2];
            domain[0][0] = -15;
            domain[0][1] = -5;
            domain[1][0] = -3;
            domain[1][1] = 3;
            return domain;
        }

        public static double BunkinN6(double[] x)
        {
            return 100 * Math.Sqrt(Math.Abs(x[1] - 0.01 * x[0] * x[0])) + 0.01 * Math.Abs(x[0] + 10);
        }

        // -5 <= x,y <= 5, Dim = 2
        public static double[][] HimmelblauDomain()
        {
            double[][] domain = new double[2][];
            for (int i = 0; i < 2; i++)
            {
                domain[i] = new double[2];
                domain[i][0] = -5;
                domain[i][1] = 5;
            }
            return domain;
        }

        public static double Himmelblau(double[] x)
        {
            return Math.Pow(x[0] * x[0] + x[1] - 11, 2) + Math.Pow(x[0] + x[1] * x[1] - 7, 2);
        }
    }
}