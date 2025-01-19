using MetaheuristicsAPI.Interfaces;

namespace MetaheuristicsAPI.Algorithms
{
    public class CircleSearchAlgorithm : IOptimizationAlgorithm
    {
        // number of objects and iterations
        private int SearchAgentsNo, MaxIter;

        // parameters
        public double c = 0.8;

        public CircleSearchAlgorithm(int N, int I)
        {
            this.SearchAgentsNo = N;
            this.MaxIter = I;
        }

        #region IOptimizationAlgorithm implementation
        public string Name { get; set; } = "Circle Search Algorithm";

        public double[] XBest { get; set; } = null!;
        public double FBest { get; set; } = default;
        public int NumberOfEvaluationFitnessFunction { get; set; }

        private ParamInfo[] _paramInfo = [
                new ParamInfo("C", "Parametr C", 0.0, 1.0, 0.8, 0.1)
            ];
        public ParamInfo[] ParamsInfo { get => _paramInfo; set => _paramInfo = value; }
        public IStateWriter writer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IStateReader reader { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IGenerateTextReport stringReportGenerator { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IGeneratePDFReport pdfReportGenerator { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public void Solve(fitnessFunction f, double[][] domain, params double[]? parameters)
        {
            int dim = domain.GetLength(0);
            double[] lb = new double[dim];
            double[] ub = new double[dim];
            for (int i = 0; i < dim; i++)
            {
                lb[i] = domain[i][0];
                ub[i] = domain[i][1];
            }
            if (parameters != null)
            {
                if (parameters.Length == 1)
                {
                    if (parameters[0] >= 0.0 && parameters[0] <= 1.0)
                        this.c = parameters[0];
                    else
                        throw new ArgumentException("Circle search - incorrect parameter value for C");
                }
                else throw new ArgumentException("Circle search - incorrect values for parameters");
            }
            InitAlgorithm(dim, f, lb, ub);
            Compute();
        }
        #endregion

        #region Algorithm variables

        private double[] lb = null!;
        private double[] ub = null!;
        private int dim = default;
        private fitnessFunction fobj = null!;
        private double[][] X_t = null!;
        
        private double[] ResultsHistory = null!;
        #endregion

        #region Helper methods
        public double[] GetResultsHistory()
        {
            return ResultsHistory;
        }
        #endregion

        #region Algorithm
        public void InitAlgorithm(int Dim, fitnessFunction Fun, double[] Lb, double[] Ub)
        {
            this.dim = Dim;
            this.fobj = Fun;
            this.X_t = new double[SearchAgentsNo][];
            this.ResultsHistory = new double[MaxIter];
            this.NumberOfEvaluationFitnessFunction = 0;

            if (Lb.Length == 1)
            {
                this.lb = new double[Dim];
                Array.Fill(this.lb, Lb[0]);
            }
            else
            {
                this.lb = Lb;
            }
            if (Ub.Length == 1)
            {
                this.ub = new double[Dim];
                Array.Fill(this.ub, Ub[0]);
            }
            else
            {
                this.ub = Ub;
            }

            Random rand = new Random();
            for (int i = 0; i < SearchAgentsNo; i++)
            {
                X_t[i] = new double[dim];
                for (int j = 0; j < dim; j++)
                {
                    X_t[i][j] = lb[j] + (ub[j] - lb[j]) * rand.NextDouble();
                }
            }
        }

        public double Compute()
        {
            double[] fitness = new double[SearchAgentsNo];

            for (int i = 0; i < SearchAgentsNo; i++)
            {
                fitness[i] = fobj(X_t[i]);
                NumberOfEvaluationFitnessFunction++;
            }

            double best = fitness[0];
            int indx = 0;
            for (int i = 1; i < SearchAgentsNo; i++)
            {
                if (fitness[i] < best)
                {
                    best = fitness[i];
                    indx = i;
                }
            }

            double[] X_c = new double[dim];
            Array.Copy(X_t[indx], X_c, dim);

            int counter = 0;
            Random rand = new Random();

            while (counter < MaxIter)
            {
                double a = Math.PI - Math.PI * Math.Pow((double)counter / MaxIter, 2);

                for (int i = 0; i < SearchAgentsNo; i++)
                {
                    double w = Math.PI * rand.NextDouble() - Math.PI;
                    double p = 1 - 0.9 * Math.Sqrt((double)counter / MaxIter);

                    for (int j = 0; j < dim; j++)
                    {
                        if (counter > (c * MaxIter))
                        {
                            X_t[i][j] = X_c[j] + (X_c[j] - X_t[i][j]) * Math.Tan(w * rand.NextDouble());
                        }
                        else
                        {
                            X_t[i][j] = X_c[j] + (X_c[j] - X_t[i][j]) * Math.Tan(w * p);
                        }

                        if (X_t[i][j] > ub[j])
                        {
                            X_t[i][j] = ub[j];
                        }
                        else if (X_t[i][j] < lb[j])
                        {
                            X_t[i][j] = lb[j];
                        }
                    }
                }

                for (int i = 0; i < SearchAgentsNo; i++)
                {
                    fitness[i] = fobj(X_t[i]);
                    NumberOfEvaluationFitnessFunction++;

                    if (fitness[i] < best)
                    {
                        best = fitness[i];
                        Array.Copy(X_t[i], X_c, dim);
                    }
                }

                ResultsHistory[counter] = best;
                counter++;
            }

            XBest = new double[dim];
            Array.Copy(X_c, XBest, dim);
            FBest = best;
            return FBest;
        }
    }
    #endregion
}