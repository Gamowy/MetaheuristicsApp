using MetaheuristicsAPI.Interfaces;
using MetaheuristicsAPI.Schemas;
using System.Text;


namespace MetaheuristicsAPI.FileHadlers
{
    public class TextFileReportWriter : IGenerateTextReport
    {
        private int fileIndex = 0;
        private string path;
        private string indexFilePath;
        private TestResults[] results;
        private bool TestMultiple;

        public string _reportString = "";
        public string ReportString => _reportString;


        public TextFileReportWriter(TestResults[] results, string rootPath, bool testMultiple)
        {
            this.results = results;
            this.path = $"{rootPath}/data/txtReports/";
            this.indexFilePath = $"{rootPath}/data/txtReportsIndex.txt";
            this.TestMultiple = testMultiple;
            // Read or create index file
            if (File.Exists(indexFilePath))
            {
                fileIndex = int.Parse(File.ReadAllText(indexFilePath));
            }
            else
            {
                File.WriteAllText(indexFilePath, fileIndex.ToString());
            }
        }

        public void WriteTxt()
        {
            _reportString = (TestMultiple) ? ComposeTxtMultipleAlgorithms() : ComposeTxtSingleAlgorithm();

            var savePath = Path.Combine(path, $"raport{fileIndex}-{DateTime.Now:yyyy-MM-dd-}.txt");
            using (StreamWriter writer = File.CreateText(savePath))
            {
                writer.WriteLine(_reportString);
            };
            File.WriteAllText(indexFilePath, fileIndex.ToString(), Encoding.UTF8);
        }

        // Write report string for testing single algorithm
        private string ComposeTxtSingleAlgorithm()
        {
            string output = "";
            if (results.Length > 0)
            {
                fileIndex++;
                output += $"Raport nr. {fileIndex}, Data raportu: {DateTime.Now}\r\n";
                output += $"Testowany algorytm: {results[0].AlgorithmName}\r\n";
                for (int i = 0; i < results.Length; i++)
                {
                    output += $"Test nr. {i + 1}\r\n";
                    output += $"Algorytm: {ToTitleCase(results[i].AlgorithmName)}\r\n";
                    output += $"Funkcja testowa: {ToTitleCase(results[i].FunctionName)}\r\n";
                    output += $"Ilość iteracji: {results[i].I}\r\n";
                    output += $"Rozmiar populacji: {results[i].N}\r\n";
                    output += $"Parametry wewnętrzne: {string.Join(", ", results[i].Parameters ?? [])}\r\n\r\n";
                    output += $"Liczba wywołań funkcji: {results[i].NumberOfEvaluationFitnessFunction}\r\n";
                    output += $"XBest: {string.Join(", ", results[i].XBest)}\r\n";
                    output += $"FBest: {results[i].FBest}\r\n\r\n";
                }
            }
            return output;
        }

        // Write report string for testing multiple algorithms
        private string ComposeTxtMultipleAlgorithms()
        {
            string output = "";
            if (results.Length > 0)
            {
                var testedAlgorithms = results.Select(x => ToTitleCase(x.AlgorithmName)).Distinct().ToArray();
                var fitnessFunction = results[0].FunctionName;

                fileIndex++;
                output += $"Raport nr. {fileIndex}, Data raportu: {DateTime.Now}\r\n";
                output += $"Test porównawczy wielu algorytmów\r\n";
                output += $"Testowane algorytmy: {string.Join(", ", testedAlgorithms)}\r\n";
                output += $"Funkcja testowa: {ToTitleCase(fitnessFunction)}\r\n";
                for (int i = 0; i < results.Length; i++)
                {
                    output += $"Test nr. {i + 1}\r\n";
                    output += $"Algorytm: {ToTitleCase(results[i].AlgorithmName)}\r\n";
                    output += $"Funkcja testowa: {ToTitleCase(results[i].FunctionName)}\r\n";
                    output += $"Ilość iteracji: {results[i].I}\r\n";
                    output += $"Rozmiar populacji: {results[i].N}\r\n";
                    output += $"Liczba wywołań funkcji: {results[i].NumberOfEvaluationFitnessFunction}\r\n";
                    output += $"XBest: {string.Join(", ", results[i].XBest)}\r\n";
                    output += $"FBest: {results[i].FBest}\r\n\r\n";
                }
            }
            return output;
        }

        private static string ToTitleCase(string? str)
        {
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str ?? "");
        }
    }
}
