using MetaheuristicsAPI.Interfaces;
using MetaheuristicsAPI.Schemas;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Reflection.PortableExecutable;

namespace MetaheuristicsAPI.FileHandlers
{
    public class PdfFileReportWriter : IGeneratePDFReport
    {
        public string fileName = "";
        private int fileIndex = 0;
        private string path;
        private string indexFilePath;
        private TestResults[] results;
        private bool TestMultiple;

        public PdfFileReportWriter(TestResults[] results, string rootPath, bool testMultiple)
        {
            this.results = results;
            this.path = $"{rootPath}/data/pdfReports/";
            this.indexFilePath = $"{rootPath}/data/pdfReportsIndex.txt";
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

        public void GenerateReport()
        {
            fileIndex++;
            fileName = $"raport{fileIndex}-{DateTime.Now:yyyy-MM-dd}.pdf";
            var savePath = Path.Combine(path, fileName);
            var pdf = (TestMultiple) ? ComposePdfFileMultipleAlgorithms() : ComposePdfFileSingleAlgorithm(); 
            pdf.GeneratePdf(savePath);
            File.WriteAllText(indexFilePath, fileIndex.ToString());
        }

        // Create report pdf for testing single algorithm
        private Document ComposePdfFileSingleAlgorithm()
        {
            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    page.Header()
                        .ShowOnce()
                        .PaddingBottom(30)
                        .Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item()
                                    .Text($"Raport nr. {fileIndex}")
                                    .Bold().FontSize(24).FontColor(Colors.DeepPurple.Medium);

                                col.Item()
                                    .Text($"Data raportu: {DateTime.Now}")
                                    .FontSize(14);
                                if (results.Length > 0)
                                {
                                    col.Item()
                                        .Text($"Testowany algorytm: {ToTitleCase(results[0].AlgorithmName)}")
                                        .FontSize(14);
                                    if (results[0].Parameters != null)
                                    {
                                        col.Item()
                                            .Text($"Parametry wewnętrzne: {string.Join(", ", results[0].Parameters ?? [])}")
                                            .FontSize(14);
                                    }
                                }
                            });
                        });

                    page.Content()
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(40);
                                columns.ConstantColumn(75);
                                columns.ConstantColumn(75);
                                columns.ConstantColumn(40);
                                columns.ConstantColumn(40);
                                columns.ConstantColumn(40);
                                columns.RelativeColumn();
                                columns.ConstantColumn(50);
                            });
                            

                            table.Header(header =>
                            {
                                header.Cell().Element(HeaderStyle).AlignCenter().Text("Lp.");
                                header.Cell().Element(HeaderStyle).AlignCenter().Text("Algorytm");
                                header.Cell().Element(HeaderStyle).AlignCenter().Text("Funkcja testowa");
                                header.Cell().Element(HeaderStyle).AlignCenter().Text("Ilość iteracji");
                                header.Cell().Element(HeaderStyle).AlignCenter().Text("Rozmiar populacji");
                                header.Cell().Element(HeaderStyle).AlignCenter().Text("Liczba wywołań funkcji");
                                header.Cell().Element(HeaderStyle).AlignCenter().Text("XBest");
                                header.Cell().Element(HeaderStyle).AlignCenter().Text("Fbest");
                            });
                            
                            for (int i = 0; i < results.Length; i++)
                            {
                                var xBest = results[i].XBest.Select(n => {
                                    double round = Math.Round(n, 4);
                                    return round == 0 ? 0 : round;
                                    });
                                var fBest = Math.Round(results[i].FBest, 4);
                                table.Cell().Element(CellStyle).AlignCenter().Text($"{i+1}");
                                table.Cell().Element(CellStyle).AlignCenter().Text(ToTitleCase(results[i].AlgorithmName));
                                table.Cell().Element(CellStyle).AlignCenter().Text(ToTitleCase(results[i].FunctionName));
                                table.Cell().Element(CellStyle).AlignCenter().Text($"{results[i].I}");
                                table.Cell().Element(CellStyle).AlignCenter().Text($"{results[i].N}");
                                table.Cell().Element(CellStyle).AlignCenter().Text($"{results[i].NumberOfEvaluationFitnessFunction}");
                                table.Cell().Element(CellStyle).AlignCenter().Text($"({string.Join(", ", xBest)})");
                                table.Cell().Element(CellStyle).AlignCenter().Text($"{fBest}");
                            }
                        }
                        );

                    page.Footer()
                        .AlignCenter()
                        .PaddingTop(5)
                        .Text(x =>
                        {
                            x.Span("Strona ");
                            x.CurrentPageNumber();
                            x.Span(" / ");
                            x.TotalPages();
                        });
                });
            });
            return pdf;
        }

        // Create report pdf for testing multiple algorithms
        private Document ComposePdfFileMultipleAlgorithms()
        {
            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    page.Header()
                        .ShowOnce()
                        .PaddingBottom(30)
                        .Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item()
                                    .Text($"Raport nr. {fileIndex}")
                                    .Bold().FontSize(24).FontColor(Colors.DeepPurple.Medium);

                                col.Item()
                                    .Text($"Data raportu: {DateTime.Now}")
                                    .FontSize(14);
                                if (results.Length > 0)
                                {
                                    var testedAlgorithms = results.Select(x => ToTitleCase(x.AlgorithmName)).Distinct().ToArray();
                                    var fitnessFunction = results[0].FunctionName;

                                    col.Item()
                                        .Text($"Testowane algorytmy: {string.Join(", ", testedAlgorithms)}")
                                        .FontSize(14);

                                    col.Item()
                                        .Text($"Funkcja testowa: {ToTitleCase(fitnessFunction)}")
                                        .FontSize(14);
                                }
                            });
                        });

                    page.Content()
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(40);
                                columns.ConstantColumn(75);
                                columns.ConstantColumn(75);
                                columns.ConstantColumn(40);
                                columns.ConstantColumn(40);
                                columns.ConstantColumn(40);
                                columns.RelativeColumn();
                                columns.ConstantColumn(50);
                            });


                            table.Header(header =>
                            {
                                header.Cell().Element(HeaderStyle).AlignCenter().Text("Lp.");
                                header.Cell().Element(HeaderStyle).AlignCenter().Text("Algorytm");
                                header.Cell().Element(HeaderStyle).AlignCenter().Text("Funkcja testowa");
                                header.Cell().Element(HeaderStyle).AlignCenter().Text("Ilość iteracji");
                                header.Cell().Element(HeaderStyle).AlignCenter().Text("Rozmiar populacji");
                                header.Cell().Element(HeaderStyle).AlignCenter().Text("Liczba wywołań funkcji");
                                header.Cell().Element(HeaderStyle).AlignCenter().Text("XBest");
                                header.Cell().Element(HeaderStyle).AlignCenter().Text("Fbest");
                            });

                            for (int i = 0; i < results.Length; i++)
                            {
                                var xBest = results[i].XBest.Select(n => {
                                    double round = Math.Round(n, 4);
                                    return round == 0 ? 0 : round;
                                });
                                var fBest = Math.Round(results[i].FBest, 4);
                                table.Cell().Element(CellStyle).AlignCenter().Text($"{i + 1}");
                                table.Cell().Element(CellStyle).AlignCenter().Text(ToTitleCase(results[i].AlgorithmName));
                                table.Cell().Element(CellStyle).AlignCenter().Text(ToTitleCase(results[i].FunctionName));
                                table.Cell().Element(CellStyle).AlignCenter().Text($"{results[i].I}");
                                table.Cell().Element(CellStyle).AlignCenter().Text($"{results[i].N}");
                                table.Cell().Element(CellStyle).AlignCenter().Text($"{results[i].NumberOfEvaluationFitnessFunction}");
                                table.Cell().Element(CellStyle).AlignCenter().Text($"({string.Join(", ", xBest)})");
                                table.Cell().Element(CellStyle).AlignCenter().Text($"{fBest}");
                            }
                        }
                        );

                    page.Footer()
                        .AlignCenter()
                        .PaddingTop(5)
                        .Text(x =>
                        {
                            x.Span("Strona ");
                            x.CurrentPageNumber();
                            x.Span(" / ");
                            x.TotalPages();
                        });
                });
            });
            return pdf;
        }
        static IContainer HeaderStyle(IContainer container) => 
            container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).PaddingBottom(5).BorderColor(Colors.Black);

        static IContainer CellStyle(IContainer container) => 
            container.BorderBottom(1).BorderColor(Colors.Grey.Lighten1)
            .PaddingVertical(5).PaddingHorizontal(5);

        private static string ToTitleCase(string? str)
        {
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str ?? "");
        }
    }
}
