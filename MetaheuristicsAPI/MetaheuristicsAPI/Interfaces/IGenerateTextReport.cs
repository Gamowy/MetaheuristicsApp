namespace MetaheuristicsAPI.Interfaces
{
    public interface IGenerateTextReport
    {

        // Tworzy raport w postaciłań cucha znaków
        // w raporcie powinny znale źćsię informacje o:
        // najlepszym osobniku wraz z wartością funkcji celu ,
        // liczbie wywołań funkcji celu ,
        // parametrach algorytmu
        string ReportString { get; }
    }
}