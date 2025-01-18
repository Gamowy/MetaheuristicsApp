using MetaheuristicsAPI.Interfaces;
using MetaheuristicsAPI.Schemas;


namespace MetaheuristicsAPI.FileHadlers
{
    public class TextFileReportWriter : IGenerateTextReport
    {
        private int fileIndex = 0;
        private string path;
        private string indexFilePath;
        private TestResults[] results;

        public string _reportString = "";
        public string ReportString => _reportString;


        public TextFileReportWriter(TestResults[] results, string rootPath)
        {
            this.results = results;
            this.path = $"{rootPath}/data/txtReports/";
            this.indexFilePath = $"{rootPath}/data/txtReportsIndex.txt";
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
            // Write report string
            fileIndex++;
            _reportString += $"Raport nr. {fileIndex}, Data raportu: {DateTime.Now}\r\n\r\n";
            for (int i = 0; i < results.Length; i++)
            {
                _reportString += $"Test nr. {i}\r\n";
                _reportString += $"Algorytm: {ToTitleCase(results[i].AlgorithmName)}\r\n";
                _reportString += $"Funkcja testowa: {ToTitleCase(results[i].FunctionName)}\r\n";
                _reportString += $"Ilość iteracji: {results[i].I}\r\n";
                _reportString += $"Rozmiar populacji: {results[i].N}\r\n";
                _reportString += $"Ilość wywołań funkcji: {results[i].NumberOfEvaluationFitnessFunction}\r\n";
                _reportString += $"XBest: {string.Join(", ", results[i].XBest)}\r\n";
                _reportString += $"FBest: {results[i].FBest}\r\n\r\n";
            }

            var savePath = Path.Combine(path, $"raport{fileIndex}-{DateTime.Now:yyyy-MM-dd}.txt");
            using (StreamWriter writer = File.CreateText(savePath))
            {
                writer.WriteLine(_reportString);
            };
            File.WriteAllText(indexFilePath, fileIndex.ToString());
        }

        private static string ToTitleCase(string? str)
        {
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str ?? "");
        }
    }
}
