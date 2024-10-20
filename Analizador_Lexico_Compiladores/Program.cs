using Analizador_Lexico_Compiladores;
using System;
using System.IO;
using static Analizador_Lexico_Compiladores.ParserLR1;

namespace Analizador_Lexico_Compiladores
{
    public class Program
    {
        static void Main(string[] args)
        {
            string gramaticatxt = "C:\\Users\\PC\\Documents\\Universidad\\2024\\Compiladores\\Proyecto\\Proyecto-Compis-V2\\.txt's\\GRAMATICA-YOSHI-CORREGIDO - v3 (20241016).txt";
            string prueba1txt = "C:\\Users\\PC\\Documents\\Universidad\\2024\\Compiladores\\Proyecto\\Proyecto-Compis-V2\\.txt's\\prueba1.txt";


            Console.WriteLine("Por favor, introduce la ruta completa del archivo que deseas analizar:");
            //string filePath = Console.ReadLine();
            string filePath = gramaticatxt;

            if (!File.Exists(filePath))
            {
                Console.WriteLine("El archivo no existe. Por favor, verifica la ruta e intente nuevamente.");
                Main(args);
            }

            try
            {
                // Leer y almacenar cada línea del archivo
                string[] lines = File.ReadAllLines(filePath);

                Console.WriteLine("Archivo leído correctamente. Iniciando validación...");

                // Crear una instancia de GrammarValidator y validar la gramática
                GrammarValidator validator = new GrammarValidator();
                if (validator.PrevalidateGrammar(lines))
                {
                    Console.WriteLine("Prevalidación completada sin errores.");
                    
                    //declarar lexer
                    string fullText = string.Join("\n", lines);
                    Lexer lexer = new Lexer(fullText);
                    //call lexer. tokenize

                    Console.WriteLine("Tokenizacion completa");

                    //call escaner (verificar no terminales en tokens)
                    Scanner scanner = new Scanner(lines);
                    //call escaner (verficar no terminal en der, tenga prod en izq)
                    scanner.StartScan();


                    //............................................................CODIGO DE PARSER............................................................

                    List<string> grammarProductions = lines.ToList();
                    ParserLR1 parser = new ParserLR1(grammarProductions);
                    Console.WriteLine("Parser LR1 generado correctamente.");

                    Console.WriteLine("Por favor, introduce la ruta completa del archivo que contiene el input a analizar:");
                    //string inputFilePath = Console.ReadLine();
                    string inputFilePath = prueba1txt;

                     if (!File.Exists(inputFilePath))
                    {
                        Console.WriteLine("El archivo de input no existe. Por favor, verifica la ruta e intenta nuevamente.");
                        Main(args);
                    }

                      // Leer el input del archivo
                    string input = File.ReadAllText(inputFilePath);
                    Console.WriteLine("Archivo de input leído correctamente.");

                    // Generar los tokens a partir del input utilizando el lexer
                    Lexer lexer2 = new Lexer(input);
                    List<Token> tokens = lexer2.GetTokens();
                     Console.WriteLine("Tokens generados:");
                    foreach (var token in tokens)
                    {
                        Console.WriteLine($"{token.Type}: {token.Value}");
                    }
                    // Llamar al parser LR1 para procesar los tokens generados
                    SymbolTable symbolTable = new SymbolTable(); // Crear la tabla de símbolos

                    //convertir token list a string list
                    List<string> tokenList = new List<string>();
                    foreach (var token in tokens)
                    {
                        string tokenTypeString = token.Type.ToString(); // Convertir TokenType a string
                        tokenList.Add(tokenTypeString);

                    }

                    bool isAccepted = parser.Parse(tokenList, symbolTable);

                      if (isAccepted)
                    {
                        Console.WriteLine("El input fue aceptado por la gramática.");
                    }
                    else
                    {
                        Console.WriteLine("El input fue rechazado por la gramática.");
                    }



                }
                else
                {
                    Console.WriteLine("Errores encontrados durante la prevalidación:");
                    foreach (var error in validator.ErrorMessages)
                    {
                        Console.WriteLine(error);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error durante la lectura del archivo: {ex.Message}");
            }
        }
    }
}
