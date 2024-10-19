using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analizador_Lexico_Compiladores
{
    internal class ParserLR1
    {
        private List<string> productions;
        private List<Dictionary<string, List<string>>> states;
        private Dictionary<(int, string), string> parsingTable;

        public ParserLR1(List<string> grammarProductions)
        {
            productions = grammarProductions;
            states = new List<Dictionary<string, List<string>>>();
            parsingTable = new Dictionary<(int, string), string>();

            GenerateStates();
            BuildParsingTable();
        }

        private void GenerateStates()
        {
            Console.WriteLine("Generando los estados del parser...");

            var initialState = Closure(new Dictionary<string, List<string>>
        {
            { "S'", new List<string> { "● <program>" } }
        });

            states.Add(initialState);

            bool addedNewState;
            do
            {
                addedNewState = false;

                for (int i = 0; i < states.Count; i++)
                {
                    var state = states[i];

                    foreach (string symbol in GetSymbols(state))
                    {
                        var newState = Goto(state, symbol);

                        if (newState.Count > 0 && !states.Any(s => StateExists(s, newState)))
                        {
                            states.Add(newState);
                            addedNewState = true;
                        }
                    }
                }
            } while (addedNewState);
        }

        private Dictionary<string, List<string>> Closure(Dictionary<string, List<string>> items)
        {
            bool addedNewItem;

            do
            {
                addedNewItem = false;

                var newItems = new Dictionary<string, List<string>>(items);

                foreach (var item in items)
                {
                    string nonTerminal = item.Key;

                    foreach (string production in item.Value)
                    {
                        string[] parts = production.Split(' ');

                        int dotIndex = Array.IndexOf(parts, "●");
                        if (dotIndex != -1 && dotIndex < parts.Length - 1)
                        {
                            string nextSymbol = parts[dotIndex + 1];

                            if (IsNonTerminal(nextSymbol))
                            {
                                foreach (var prod in GetProductionsFor(nextSymbol))
                                {
                                    string newProduction = $"● {prod}".Trim();

                                    if (!newItems.ContainsKey(nextSymbol))
                                        newItems[nextSymbol] = new List<string>();

                                    if (!newItems[nextSymbol].Contains(newProduction))
                                    {
                                        newItems[nextSymbol].Add(newProduction);
                                        addedNewItem = true;
                                    }
                                }
                            }
                        }
                    }
                }

                items = newItems;
            } while (addedNewItem);

            return items;
        }

        private Dictionary<string, List<string>> Goto(Dictionary<string, List<string>> state, string symbol)
        {
            var newState = new Dictionary<string, List<string>>();

            foreach (var item in state)
            {
                string nonTerminal = item.Key;

                foreach (string production in item.Value)
                {
                    string[] parts = production.Split(' ');

                    int dotIndex = Array.IndexOf(parts, "●");
                    if (dotIndex != -1 && dotIndex < parts.Length - 1 && parts[dotIndex + 1] == symbol)
                    {
                        parts[dotIndex] = parts[dotIndex + 1];
                        parts[dotIndex + 1] = "●";

                        string newProduction = string.Join(" ", parts);

                        if (!newState.ContainsKey(nonTerminal))
                            newState[nonTerminal] = new List<string>();

                        if (!newState[nonTerminal].Contains(newProduction))
                            newState[nonTerminal].Add(newProduction);
                    }
                }
            }

            return Closure(newState);
        }

        private HashSet<string> GetSymbols(Dictionary<string, List<string>> state)
        {
            var symbols = new HashSet<string>();

            foreach (var item in state)
            {
                foreach (string production in item.Value)
                {
                    string[] parts = production.Split(' ');

                    int dotIndex = Array.IndexOf(parts, "●");
                    if (dotIndex != -1 && dotIndex < parts.Length - 1)
                    {
                        symbols.Add(parts[dotIndex + 1]);
                    }
                }
            }

            return symbols;
        }

        private bool IsNonTerminal(string symbol)
        {
            return symbol.StartsWith("<") && symbol.EndsWith(">");
        }

        private List<string> GetProductionsFor(string nonTerminal)
        {
            return productions
                .Where(p => p.StartsWith(nonTerminal + " ="))
                .Select(p => p.Substring(nonTerminal.Length + 2).Trim())
                .ToList();
        }

        private bool StateExists(Dictionary<string, List<string>> state1, Dictionary<string, List<string>> state2)
        {
            return state1.Count == state2.Count &&
                   state1.All(kv => state2.ContainsKey(kv.Key) && state2[kv.Key].SequenceEqual(kv.Value));
        }

        private void BuildParsingTable()
        {
            Console.WriteLine("Construyendo la tabla de parsing...");

            for (int i = 0; i < states.Count; i++)
            {
                var state = states[i];

                foreach (var item in state)
                {
                    string nonTerminal = item.Key;

                    foreach (string production in item.Value)
                    {
                        string[] parts = production.Split(' ');

                        int dotIndex = Array.IndexOf(parts, "●");

                        // Caso 1: Shift
                        if (dotIndex != -1 && dotIndex < parts.Length - 1)
                        {
                            string nextSymbol = parts[dotIndex + 1];
                            int nextState = GetNextState(state, nextSymbol);

                            if (IsTerminal(nextSymbol))
                            {
                                parsingTable[(i, nextSymbol)] = $"Shift {nextState}";
                            }
                            else
                            {
                                parsingTable[(i, nextSymbol)] = $"GoTo {nextState}";
                            }
                        }
                        // Caso 2: Reduce
                        else if (dotIndex == parts.Length - 1)
                        {
                            string reducedProduction = string.Join(" ", parts.Take(parts.Length - 1));
                            int productionIndex = productions.FindIndex(p => p.Contains(reducedProduction));

                            if (productionIndex != -1)
                            {
                                parsingTable[(i, nonTerminal)] = $"Reduce {productionIndex}";
                            }
                        }

                        // Caso 3: Aceptación
                        if (nonTerminal == "S'" && production == "● <program>")
                        {
                            parsingTable[(i, "EOF")] = "Accept";
                        }
                    }
                }
            }
        }
        // Obtener el siguiente estado
        private int GetNextState(Dictionary<string, List<string>> state, string symbol)
        {
            var newState = Goto(state, symbol);

            for (int i = 0; i < states.Count; i++)
            {
                if (StateExists(states[i], newState))
                    return i;
            }

            return -1;
        }

        private bool IsTerminal(string symbol) => !symbol.StartsWith("<") && !symbol.EndsWith(">");


        public bool Parse(List<string> tokens, SymbolTable symbolTable)
        {
            Stack<int> stateStack = new Stack<int>(); // Pila de estados
            Stack<string> symbolStack = new Stack<string>(); // Pila de símbolos
            stateStack.Push(0); // Estado inicial

            int currentIndex = 0;
            while (currentIndex < tokens.Count)
            {
                int currentState = stateStack.Peek();
                string currentToken = tokens[currentIndex];

                // Obtener la acción de la tabla de parsing
                if (parsingTable.TryGetValue((currentState, currentToken), out string action))
                {
                    Console.WriteLine($"Estado: {currentState}, Token: {currentToken}, Acción: {action}");

                    if (action.StartsWith("Shift"))
                    {
                        // Shift: Avanzar al siguiente token
                        int nextState = int.Parse(action.Split(' ')[1]);
                        stateStack.Push(nextState);
                        symbolStack.Push(currentToken);

                        // Si es un identificador, agregarlo a la tabla de símbolos
                        if (currentToken == "identifier" && currentIndex + 1 < tokens.Count)
                        {
                            string identifierName = tokens[currentIndex + 1]; // El nombre real del identificador
                            symbolTable.AddSymbol(identifierName, "unknown"); // Tipo desconocido por ahora
                        }

                        currentIndex++;
                    }
                    else if (action.StartsWith("Reduce"))
                    {
                        // Reduce: Aplicar una regla de la gramática
                        int ruleIndex = int.Parse(action.Split(' ')[1]);
                        string production = productions[ruleIndex];

                        var parts = production.Split('=');
                        string nonTerminal = parts[0].Trim();
                        string[] rhs = parts[1].Trim().Split(' ');

                        // Sacar los símbolos correspondientes de la pila
                        for (int i = 0; i < rhs.Length; i++)
                        {
                            symbolStack.Pop();
                            stateStack.Pop();
                        }

                        // Poner el no terminal en la pila de símbolos
                        symbolStack.Push(nonTerminal);

                        // GoTo: Cambiar al nuevo estado
                        int gotoState = int.Parse(parsingTable[(stateStack.Peek(), nonTerminal)].Split(' ')[1]);
                        stateStack.Push(gotoState);
                    }
                    else if (action == "Accept")
                    {
                        Console.WriteLine("Cadena aceptada.");
                        return true;
                    }

                }
                else
                {
                    Console.WriteLine($"Error: No se encontró una acción para el estado {currentState} con el token {currentToken}.");
                    return false;
                }
            }

            Console.WriteLine("Error: Cadena incompleta.");
            return false;
        }


        public class SymbolTable
        {
            private Dictionary<string, string> symbols = new Dictionary<string, string>();

            public void AddSymbol(string identifier, string type)
            {
                if (!symbols.ContainsKey(identifier))
                {
                    symbols[identifier] = type;
                    Console.WriteLine($"Símbolo agregado: {identifier} - {type}");
                }
            }

            public bool ContainsSymbol(string identifier)
            {
                return symbols.ContainsKey(identifier);
            }
        }
    }
}
