using Analizador_Lexico_Compiladores;
using System;
using System.IO;

namespace Analizador_Lexico_Compiladores
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Por favor, introduce la ruta completa del archivo que deseas analizar:");
            string filePath = Console.ReadLine();

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
                    //decalrar lexer
                    //call lexer. tokenize
                    //call escaner (verificar terminales en tokens)
                    //call escaner (verficar no terminal en der, tenga prod en izq)
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
