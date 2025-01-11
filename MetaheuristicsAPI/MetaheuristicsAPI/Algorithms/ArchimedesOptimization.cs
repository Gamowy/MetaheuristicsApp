/* Archimedes Optimization Algorithm
* Parametry zewnętrzne:
* N - wielkość populacji
* I - ilość iteracji
* Dim - wymiar dziedziny funkcji testowej
* Fun - funkcja testowa
* Lb - tablica dolnych ograniczeń dziedziny funkcji testowej (jeżeli ograniczenie jest takie same dla każdej zmiennej to wystarczy podać jedno)
* Ub - tablica górnych ograniczeń dziedziny funkcji testowej (jeżeli ograniczenie jest takie same dla każdej zmiennej to wystarczy podać jedno)
* 
* Domyślne wartości parametrów wewnętrznych:
* C1 = 2.0
* C2 = 6.0
* C3 = 2.0
* C4 = 0.5
*/
namespace MetaheuristicsAPI.Algorithms
{
    public class ArchimedesOptimization
    {
        private class ImmersedObject
        {
            public double[] x;
            public double FValue;
            public double[] den;
            public double[] vol;
            public double[] acc;
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

        // parameters
        public double C1 = 2.0, C2 = 6.0, C3 = 2.0, C4 = 0.5;
        // number of objects and iterations
        int N, I;
        // dimension of fitness function input
        int Dim;
        // fitness function
        Func<double[], double> Fun;
        // lower and upper bounds of fitness function for each dimension
        double[] Lb, Ub;
        // population of objects
        ImmersedObject[] ObjectPopulation;
        // best object
        ImmersedObject BestObject;

        public ArchimedesOptimization(int N, int I, int Dim, Func<double[], double> Fun, double[] Lb, double[] Ub)
        {
            this.N = N;
            this.I = I;
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

        private double NormalizeAcceleration(int index, double acc)
        {
            double minAcc = ObjectPopulation.Min(obj => obj.acc[index]);
            double maxAcc = ObjectPopulation.Max(obj => obj.acc[index]);
            return 0.9 * ((acc - minAcc) / (maxAcc - minAcc)) + 0.1;
        }


        // IOptimizationAlgorithm implementation
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

        public double Solve()
        {
            Random rand = new Random();
            int t = 1;
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
                            obj.acc[i] = NormalizeAcceleration(i, obj.acc[i]);
                            xNew[i] = obj.x[i] + C1 * rand.NextDouble() * obj.acc[i] * d * (xRand[i] - obj.x[i]);
                        }
                    }
                    // exploitation phase
                    else
                    {
                        for (int i = 0; i < Dim; i++)
                        {
                            obj.acc[i] = (BestObject.den[i] + BestObject.vol[i] * BestObject.acc[i]) / (obj.den[i] * obj.vol[i]);
                            obj.acc[i] = NormalizeAcceleration(i, obj.acc[i]);

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
                t++;
            }
            return FBest;
        }
    }
}