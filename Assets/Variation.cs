using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Variation
{
    public int SelectVariation(string[] indicators)
    {
        if (indicators.Length == 0)
        {
            return 1;
        }

        Dictionary<string, int> variation = new Dictionary<string, int>
        {
            {"SND", 1}, {"CLR", 1}, {"CAR", 1},
            {"IND", -1}, {"FRQ", -1}, {"SIG", -1},
            {"NSA", 2}, {"MSA", 2}, {"TRN", 2},
            {"BOB", -2}, {"FRK", -2}
        };

        Dictionary<string, string> categorias = new Dictionary<string, string>
        {
            {"SND", "A"}, {"CLR", "A"}, {"CAR", "A"},
            {"IND", "B"}, {"FRQ", "B"}, {"SIG", "B"},
            {"NSA", "C"}, {"MSA", "C"}, {"TRN", "C"},
            {"BOB", "D"}, {"FRK", "D"}
        };

        HashSet<string> categoriasVisitadas = new HashSet<string>();
        int total_variation = 0;

        foreach (string indicador in indicators)
        {
            string categoria = categorias.ContainsKey(indicador) ? categorias[indicador] : null;
            if (categoria != null)
            {
                if (!categoriasVisitadas.Contains(categoria))
                {
                    total_variation += variation.ContainsKey(indicador) ? variation[indicador] : 0;
                    categoriasVisitadas.Add(categoria);
                }
            }
        }

        return total_variation;
    }
}