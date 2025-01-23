using MetaheuristicsAPI.Interfaces;
using System.Runtime.CompilerServices;

namespace MetaheuristicsAPI.Algorithms
{
    public class ArchimedesOptimization : IOptimizationAlgorithm
    {
        // number of objects and iterations
        int N, I;

        // parameters
        public double C1 = 2.0, C2 = 6.0, C3 = 2.0, C4 = 0.5;

        public ArchimedesOptimization(int numberOfObjects, int numberOfIteratons)
        {
            this.N = numberOfObjects;
            this.I = numberOfIteratons;
        }

        #region IOptimizationAlgorithm implementation
        private string _name = "Archimedes Optimization Algorithm";
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public double[] XBest
        {
            get => BestObject.x;
            set { }
        }

        public double FBest
        {
            get => BestObject.FValue;
            set { }
        }

        private int _numberOfEvaluationFitnessFunction = 0;
        public int NumberOfEvaluationFitnessFunction
        {
            get => _numberOfEvaluationFitnessFunction;
            set => _numberOfEvaluationFitnessFunction = value;
        }

        private ParamInfo[] _paramsInfo =
        [
            new ParamInfo("C1", "Parametr C1, Zalecane wartości: 1.0, 2.0", 1.0, 2.0, 2.0, 2.0),
            new ParamInfo("C2", "Parametr C2, Zalecane wartości: 2.0, 4.0, 6.0", 2.0, 6.0, 6.0, 2.0),
            new ParamInfo("C3", "Parametr C3, Zalecane wartości: 1.0, 2.0", 1.0, 2.0, 2.0, 1.0),
            new ParamInfo("C4", "Parametr C4, Zalecane wartości: 0.5, 1.0", 0.5, 1.0, 0.5, 1.0)
        ];
        public ParamInfo[] ParamsInfo { get => _paramsInfo; set => _paramsInfo = value; }

        #region StateWriter/ StateReadder
        static string StatesFolder = "AlgorithmStates";
        static string StatePath = "archimedes_state.txt";

        private class StateWriter : IStateWriter
        {
            ImmersedObject[] population = default!;
            int N, I;
            int iterations;
            int numberOfEvaluations;

            public void UpdateState(int N, int I, ImmersedObject[] population, int iterations, int numberOfEvaluations)
            {
                this.N = N;
                this.I = I;
                this.population = population;
                this.iterations = iterations;
                this.numberOfEvaluations = numberOfEvaluations;
            }

            public void SaveToFileStateOfAlgorithm(string path)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(path))
                    {
                        sw.WriteLine(N.ToString());
                        sw.WriteLine(I.ToString());
                        sw.WriteLine(iterations.ToString());
                        sw.WriteLine(numberOfEvaluations.ToString());
                        sw.WriteLine(population.Length.ToString());
                        foreach (ImmersedObject obj in population)
                        {
                            sw.WriteLine(string.Join(" ", obj.x));
                            sw.WriteLine(obj.FValue.ToString());
                            sw.WriteLine(string.Join(" ", obj.den));
                            sw.WriteLine(string.Join(" ", obj.vol));
                            sw.WriteLine(string.Join(" ", obj.acc));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to save state of archimedes optimization: " + ex.Message);
                }
            }
        }

        private class StateReader : IStateReader
        {
            List<ImmersedObject> population = new List<ImmersedObject>();
            int? iterations;
            int? numberOfEvaluations;
            int? N, I;

            public (int?, int?, ImmersedObject[]?, int?, int?) GetState()
            {
                return (N, I, population.ToArray(), iterations, numberOfEvaluations);
            }

            public void LoadFromFileStateOfAlgorithm(string path)
            {
                try
                {
                    using (StreamReader sr = new StreamReader(path))
                    {
                        N = int.Parse(sr.ReadLine()!);
                        I = int.Parse(sr.ReadLine()!);
                        iterations = int.Parse(sr.ReadLine()!);
                        numberOfEvaluations = int.Parse(sr.ReadLine()!);
                        int populationLength = int.Parse(sr.ReadLine()!);
                        for (int i = 0; i < populationLength; i++)
                        {
                            double[] x = Array.ConvertAll(sr.ReadLine()!.Split(' '), double.Parse);
                            double[] den = Array.ConvertAll(sr.ReadLine()!.Split(' '), double.Parse);
                            double[] vol = Array.ConvertAll(sr.ReadLine()!.Split(' '), double.Parse);
                            double[] acc = Array.ConvertAll(sr.ReadLine()!.Split(' '), double.Parse); 
                        }
                    }
                } catch (Exception ex)
                {
                    Console.WriteLine("Failed to load state of archimedes optimization: " + ex.Message);
                }
            }
        }
        #endregion

        StateWriter _writer = new StateWriter();
        IStateWriter IOptimizationAlgorithm.writer { get => _writer; set => throw new NotImplementedException(); }

        StateReader _reader = new StateReader();
        IStateReader IOptimizationAlgorithm.reader { get => _reader; set => throw new NotImplementedException(); }
        IGenerateTextReport IOptimizationAlgorithm.stringReportGenerator { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        IGeneratePDFReport IOptimizationAlgorithm.pdfReportGenerator { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
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
                if (parameters.Length == 4)
                {
                    if (parameters[0] >= 1.0 && parameters[0] <= 2.0)
                        this.C1 = parameters[0];
                    else
                        throw new ArgumentException("Archimedes optimization - incorrect parameter value for C1");

                    if (parameters[1] >= 2.0 && parameters[1] <= 6.0)
                        this.C2 = parameters[1];
                    else
                        throw new ArgumentException("Archimedes optimization - incorrect parameter value for C2");

                    if (parameters[2] >= 1.0 && parameters[2] <= 2.0)
                        this.C3 = parameters[2];
                    else
                        throw new ArgumentException("Archimedes optimization - incorrect parameter value for C3");

                    if (parameters[3] >= 0.5 && parameters[3] <= 1.0)
                        this.C4 = parameters[3];
                    else
                        throw new ArgumentException("Archimedes optimization - incorrect parameter value for C4");
                }
                else throw new ArgumentException("Archimedes optimization - incorrect values for parameters");
            }
            InitAlgroithm(dim, f, lb, ub);
            Compute();
        }
        #endregion

        #region Algorithm variables
        // dimension of fitness function input
        private int Dim = default;

        // fitness function
        private fitnessFunction Fun = null!;

        // lower and upper bounds of fitness function for each dimension
        private double[] Lb = null!;
        private double[] Ub = null!;

        // population of objects
        private ImmersedObject[] ObjectPopulation = null!;

        // best object
        private ImmersedObject BestObject = null!;
        #endregion

        #region Helper methods 
        private class ImmersedObject
        {
            public double[] x;
            public double FValue;
            public double[] den;
            public double[] vol;
            public double[] acc;

            public ImmersedObject(double[] x, double FValue, double[] den, double[] vol, double[] acc)
            {
                this.x = x;
                this.FValue = FValue;
                this.den = den;
                this.vol = vol;
                this.acc = acc;
            }
            public ImmersedObject(int dim, double[] lb, double[] ub)
            {
                Random rand = new Random();
                x = new double[dim];
                den = new double[dim];
                vol = new double[dim];
                acc = new double[dim];
                for (int i = 0; i < dim; i++)
                {
                    x[i] = lb[i] + rand.NextDouble() * (ub[i] - lb[i]);
                    den[i] = rand.NextDouble();
                    vol[i] = rand.NextDouble();
                    acc[i] = lb[i] + rand.NextDouble() * (ub[i] - lb[i]);
                }
            }
        }

        // selects best object based on fitness function
        private void SelectBestObject()
        {
            foreach (ImmersedObject obj in ObjectPopulation)
            {
                double F = Fun(obj.x);
                NumberOfEvaluationFitnessFunction++;

                if (F < FBest)
                {
                    BestObject = obj;
                }
            }
        }

        // checks if new values are inside fitness function bounds and adjusts if needed
        private double[] checkBounds(double[] x)
        {
            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] < Lb[i])
                {
                    x[i] = Lb[i];
                }
                else if (x[i] > Ub[i])
                {
                    x[i] = Ub[i];
                }
            }
            return x;
        }

        private void NormalizeAcceleration(int index)
        {
            double minAcc = ObjectPopulation.Min(obj => obj.acc[index]);
            double maxAcc = ObjectPopulation.Max(obj => obj.acc[index]);
            foreach (var obj in ObjectPopulation)
            {
                obj.acc[index] = 0.9 * ((obj.acc[index] - minAcc) / (maxAcc - minAcc)) + 0.1;
            }
        }
        #endregion

        #region Algorithm

        private void InitAlgroithm(int Dim, fitnessFunction Fun, double[] Lb, double[] Ub)
        {
            this.Dim = Dim;
            this.Fun = Fun;

            if (Lb.Length == 1)
            {
                this.Lb = new double[Dim];
                Array.Fill(this.Lb, Lb[0]);
            }
            else
            {
                this.Lb = Lb;
            }
            if (Ub.Length == 1)
            {
                this.Ub = new double[Dim];
                Array.Fill(this.Ub, Ub[0]);
            }
            else
            {
                this.Ub = Ub;
            }

            // initalize new population with random values
            ObjectPopulation = new ImmersedObject[N];
            for (int j = 0; j < N; j++)
            {
                ObjectPopulation[j] = new ImmersedObject(Dim, this.Lb, this.Ub);
                ObjectPopulation[j].FValue = Fun(ObjectPopulation[j].x);
            }
            BestObject = ObjectPopulation[0];
            SelectBestObject();
        }

        private double Compute()
        {
            Random rand = new Random();
            int t = 1;
            //if (File.Exists(Path.Join(StatesFolder, StatePath))) 
            //{
            //    _reader.LoadFromFileStateOfAlgorithm(Path.Join(StatesFolder, StatePath));
            //    (int? n, int? i, ImmersedObject[]? savedObjects, int? savedT, int? savedNumberOfEval) = _reader.GetState();
            //    if (n != null && i != null && savedObjects != null && savedT != null && savedNumberOfEval != null)
            //    {
            //        if (N == n && I == i) 
            //        {
            //            ObjectPopulation = savedObjects;
            //            t = savedT.Value + 1;
            //            NumberOfEvaluationFitnessFunction = savedNumberOfEval.Value;
            //            SelectBestObject();
            //        }
            //    }
            //}

            while (t <= I)
            {
                double TF = Math.Exp((Double)(t - I) / I);
                TF = (TF > 1.0) ? 1.0 : TF;
                double d = Math.Exp((Double)(I - t) / I) - (Double)(t / I);

                foreach (ImmersedObject obj in ObjectPopulation)
                {
                    for (int i = 0; i < Dim; i++)
                    {
                        obj.den[i] = obj.den[i] + rand.NextDouble() * (BestObject.den[i] - obj.den[i]);
                        obj.vol[i] = obj.vol[i] + rand.NextDouble() * (BestObject.vol[i] - obj.vol[i]);
                    }

                    double[] xNew = new Double[Dim];
                    // exploration phase
                    if (TF <= 0.5)
                    {
                        ImmersedObject mr = ObjectPopulation[rand.Next(0, ObjectPopulation.Length)];
                        double[] xRand = ObjectPopulation[rand.Next(0, ObjectPopulation.Length)].x;
                        for (int i = 0; i < Dim; i++)
                        {
                            obj.acc[i] = (mr.den[i] + mr.vol[i] * mr.acc[i]) / (obj.den[i] * obj.vol[i]);
                            NormalizeAcceleration(i);
                            xNew[i] = obj.x[i] + C1 * rand.NextDouble() * obj.acc[i] * d * (xRand[i] - obj.x[i]);
                        }
                    }
                    // exploitation phase
                    else
                    {
                        for (int i = 0; i < Dim; i++)
                        {
                            obj.acc[i] = (BestObject.den[i] + BestObject.vol[i] * BestObject.acc[i]) / (obj.den[i] * obj.vol[i]);
                            NormalizeAcceleration(i);

                            double P = 2 * rand.NextDouble() - C4;
                            double F = (P <= 0.5) ? 1.0 : -1.0;
                            double T = C3 * TF;
                            T = (T > 1.0) ? 1.0 : T;

                            xNew[i] = BestObject.x[i] + F * C2 * rand.NextDouble() * obj.acc[i] * d * (T * BestObject.x[i] - obj.x[i]);
                        }
                    }
                    // update object if new values are better
                    xNew = checkBounds(xNew);
                    double FValueNew = Fun(xNew);
                    NumberOfEvaluationFitnessFunction++;
                    if (FValueNew < obj.FValue)
                    {
                        obj.x = xNew;
                        obj.FValue = FValueNew;
                    }
                }
                SelectBestObject();
                //_writer.UpdateState(N, I, ObjectPopulation, t, NumberOfEvaluationFitnessFunction);
                //_writer.SaveToFileStateOfAlgorithm(Path.Join(StatesFolder, StatePath));
                t++;
            }
            //if (File.Exists(Path.Join(StatesFolder, StatePath)))
            //    File.Delete(Path.Join(StatesFolder, StatePath));
            return FBest;
        }
        #endregion
    }
}