namespace MetaheuristicsAPI.Interfaces
{
    public interface IOptimizationAlgorithm
    {
        // Nazwa algorytmu
        string Name { get; set; }

        // Metoda zaczynająca rozwiązywanie zagadnienia poszukiwania minimum funkcji
        // Jako argument przyjmuje :
        // funkcję celu ,
        // dziedzinę zadania w postaci tablicy 2D,
        // listę pozostałych wymaganych parametrów algorytmu ( tylko wartości , w kolejno
        // ści takiej jak w ParamsInfo ).
        // Po wykonaniu ustawia odpowiednie właściwości: XBest , Fbest , NumberOfEvaluationFitnessFunction
        void Solve(fitnessFunction f, double[][] domain, params double[]? parameters);

        // Lista informacji o kolejnych parametrach algorytmu
        ParamInfo[] ParamsInfo { get; set; }

        // Obiekt odpowiedzialny za zapis stanu algorytmu do pliku
        // Po każdej iteracji algorytmu , powinno się wywołać metodę
        // SaveToFileStateOfAlghoritm tego obiektu w celu zapisania stanu algorytmu
        IStateWriter writer { get; set; }

        // Obiekt odpowiedzialny za odczyt stanu algorytmu z pliku
        // Na początku metody Solve, obiekt ten powinien wczytać stan algorytmu
        // jeśli stan został zapisany
        IStateReader reader { get; set; }

        // Obiekt odpowiedzialny za generowanie napisu z raportem
        IGenerateTextReport stringReportGenerator { get; set; }

        IGeneratePDFReport pdfReportGenerator { get; set; }

        // Właściwość zwracająca tablicę z najlepszym osobnikiem
        double[] XBest { get; set; }

        // Właściwość zwracająca wartość funkcji dopasowania dla najlepszego osobnika
        double FBest { get; set; }

        // Właściwość zwracająca liczbę wywołań funkcji dopasowania
        int NumberOfEvaluationFitnessFunction { get; set; }
    }
}