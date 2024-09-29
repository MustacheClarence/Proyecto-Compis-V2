using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Analizador_Lexico_Compiladores
{
    public class Scanner
    {
        private List<string> _productions;
        private List<string> _nonTerminalsRight;
        private List<string> _nonTerminalsLeft;
        private string _mainProduction;  // Producción principal que se debe ignorar


        public Scanner(string[] lines, string mainProduction = "<program>")
        {
            _productions = new List<string>();
            _nonTerminalsRight = new List<string>();
            _nonTerminalsLeft = new List<string>();
            _mainProduction = mainProduction; // Definimos la producción principal

            // Filtrar el array para que solo tenga las producciones
            FilterProductions(lines);
        }

        public void StartScan() 
        { 
            CollectRightSideNonTerminals();
            CollectLeftSideNonTerminals();
            CompareNonTerminals();
        }


        // Filtrar las líneas para quedarse solo con las producciones
        private void FilterProductions(string[] lines)
        {
            bool inProductions = false;
            foreach (string line in lines)
            {
                if (line.Trim().StartsWith("PRODUCTIONS"))
                {
                    inProductions = true;
                    continue; // Saltar la línea "PRODUCTIONS"
                }

                if (inProductions)
                {
                    _productions.Add(line.Trim());
                }
            }
        }

        // Método para leer las producciones y obtener los no terminales a la derecha del "="
        public void CollectRightSideNonTerminals()
        {
            string nonTerminal = @"<\w+>";

            foreach (var production in _productions)
            {
                // Dividir en lado izquierdo y derecho
                var parts = production.Split('=');
                if (parts.Length == 2)
                {
                    string rightSide = parts[1].Trim();

                    // Encontrar todos los no terminales en el lado derecho
                    MatchCollection matches = Regex.Matches(rightSide, nonTerminal);
                    foreach (Match match in matches)
                    {
                        if (!_nonTerminalsRight.Contains(match.Value))
                        {
                            _nonTerminalsRight.Add(match.Value);
                        }
                    }
                }
            }
        }
        // Método para leer las producciones y obtener los no terminales a la izquierda del "="
        public void CollectLeftSideNonTerminals()
        {
            string nonTerminal = @"<\w+>";

            foreach (var production in _productions)
            {
                // Buscar el no terminal en el lado izquierdo de la producción
                Match match = Regex.Match(production, nonTerminal);
                if (match.Success)
                {
                    // Agregar solo si no está ya en la lista (evitar duplicados)
                    if (!_nonTerminalsLeft.Contains(match.Value))
                    {
                        _nonTerminalsLeft.Add(match.Value);
                    }
                }
            }
        }

        public void CompareNonTerminals()
        {
            List<string> missingProductions = new List<string>();
            List<string> undefinedNonTerminals = new List<string>();

            // Verificar no terminales en el lado derecho que no tengan producción en el lado izquierdo
            foreach (string nonTerminal in _nonTerminalsRight)
            {
                // Si no está en el lado izquierdo y no es la producción principal
                if (!_nonTerminalsLeft.Contains(nonTerminal) && nonTerminal != _mainProduction)
                {
                    missingProductions.Add(nonTerminal);
                }
            }

            // Verificar no terminales en el lado izquierdo que no estén en el lado derecho
            foreach (string nonTerminal in _nonTerminalsLeft)
            {
                if (!_nonTerminalsRight.Contains(nonTerminal) && nonTerminal != _mainProduction)
                {
                    undefinedNonTerminals.Add(nonTerminal);
                }
            }

            // Mostrar resultados de las comparaciones
            if (missingProductions.Count > 0)
            {
                Console.WriteLine("No terminales sin producción:");
                foreach (string nt in missingProductions)
                {
                    Console.WriteLine(nt);
                }
            }
            else
            {
                Console.WriteLine("Todos los no terminales tienen producción.");
            }

            if (undefinedNonTerminals.Count > 0)
            {
                Console.WriteLine("No terminales con producción pero no usados:");
                foreach (string nt in undefinedNonTerminals)
                {
                    Console.WriteLine(nt);
                }
            }
            else
            {
                Console.WriteLine("Todos los no terminales utilizados tienen producción.");
            }
        }

    }
}