namespace MetaheuristicsAPI.Interfaces
{
    public interface IStateReader
    {
        // Metoda wczytująca z pliku stan algorytmu (w odpowiednim formacie ).
        // Stan algorytmu : numer iteracji , liczba wywołań funkcji
        void LoadFromFileStateOfAlgorithm(string path);
    }
}