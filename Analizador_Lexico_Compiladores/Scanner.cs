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


        public Scanner(string[] lines)
        {
            _productions = new List<string>();
            _nonTerminalsRight = new List<string>();
            _nonTerminalsLeft = new List<string>();

            // Filtrar el array para que solo tenga las producciones
            FilterProductions(lines);
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

        public void RemoveLeftSideNonTerminals()
        {
            string nonTerminal = @"<\w+>";

            foreach (var production in _productions)
            {
                // Buscar el no terminal en el lado izquierdo de la producción
                Match match = Regex.Match(production, nonTerminal);
                if (match.Success)
                {
                    // Almacenar el no terminal encontrado en la izquierda
                    _nonTerminalsLeft.Add(match.Value);

                    // Eliminar de la lista de no terminales de la derecha si lo encontramos en la izquierda
                    _nonTerminalsRight.RemoveAll(nt => nt == match.Value);
                }
            }

            foreach (var production in _productions)
            {
                // Dividir en lado izquierdo y derecho
                var parts = production.Split('=');
                if (parts.Length == 2)
                {
                    string leftSide = parts[0].Trim();

                    // Encontrar todos los no terminales en el lado derecho
                    MatchCollection matches = Regex.Matches(leftSide, nonTerminal);
                    foreach (Match match in matches)
                    {
                        
                    }
                }
            }
        }

        // Obtener los no terminales que no tienen producción
        public List<string> GetNonTerminalsWithoutProductions()
        {
            return _nonTerminalsRight;
        }
    }

   
}
