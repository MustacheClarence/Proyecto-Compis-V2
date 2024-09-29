using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ProyectoCompilador
{
    public class Lexer
    {
        private string _input;
        private int _position;
        private List<Token> _tokens;
        private Dictionary<string, TokenType> _keywords;

        public Lexer(string input)
        {
            _input = input;
            _tokens = new List<Token>();
            _position = 0;

            // Definir los keywords de la gramática
            _keywords = new Dictionary<string, TokenType>
            {
                {"PROGRAM", TokenType.Keyword},
                {"INCLUDE", TokenType.Keyword},
                {"CONST", TokenType.Keyword},
                {"TYPE", TokenType.Keyword},
                {"VAR", TokenType.Keyword},
                {"RECORD", TokenType.Keyword},
                {"ARRAY", TokenType.Keyword},
                {"OF", TokenType.Keyword},
                {"PROCEDURE", TokenType.Keyword},
                {"FUNCTION", TokenType.Keyword},
                {"IF", TokenType.Keyword},
                {"THEN", TokenType.Keyword},
                {"ELSE", TokenType.Keyword},
                {"FOR", TokenType.Keyword},
                {"TO", TokenType.Keyword},
                {"WHILE", TokenType.Keyword},
                {"DO", TokenType.Keyword},
                {"EXIT", TokenType.Keyword},
                {"END", TokenType.Keyword},
                {"PRINTLN", TokenType.Keyword},
                {"READLN", TokenType.Keyword},
                {"CASE", TokenType.Keyword},
                {"BREAK", TokenType.Keyword},
                {"DOWNTO", TokenType.Keyword},
                {"INTEGER", TokenType.Keyword},
                {"REAL", TokenType.Keyword},
                {"BOOLEAN", TokenType.Keyword},
                {"STRING", TokenType.Keyword},
                {"BEGIN", TokenType.Keyword}
            };

            Tokenize();
        }

        private void Tokenize()
        {
            while (_position < _input.Length)
            {
                // Obtener el carácter actual
                char current = _input[_position];
                // Obtener el siguiente carácter si no estamos al final de la entrada
                string nextChar = _position + 1 < _input.Length ? _input[_position + 1].ToString() : string.Empty;

                // Ignorar espacios en blanco
                if (Char.IsWhiteSpace(current))
                {
                    _position++;
                }
                // Procesar números
                else if (Char.IsDigit(current))
                {
                    string number = "";
                    // Mientras haya dígitos, formar el número completo
                    while (_position < _input.Length && Char.IsDigit(_input[_position]))
                    {
                        number += current;
                    }
                    _position++
                    _tokens.Add(new Token(TokenType.Number, number));
                }
                // Procesar identificadores y keywords
                else if (Char.IsLetter(current))
                {
                    string identifier = "";
                    // Recolectar letras, dígitos o guiones bajos
                    while (_position < _input.Length && (Char.IsLetterOrDigit(_input[_position]) || _input[_position] == '_'))
                    {
                        identifier += current;
                    }
                    _position++;
                    // Verificar si es una keyword
                    if (_keywords.ContainsKey(identifier))
                    {
                        _tokens.Add(new Token(_keywords[identifier], identifier));
                    }
                    // Verificar si es un operador (OR, AND, MOD, DIV, NOT)
                    else if (identifier == "OR" || identifier == "AND" || identifier == "MOD" || identifier == "DIV" || identifier == "NOT")
                    {
                        _tokens.Add(new Token(TokenType.Operator, identifier));
                    }
                    // Si no es keyword ni operador, es un identificador
                    else
                    {
                        _tokens.Add(new Token(TokenType.Identifier, identifier));
                    }
                }
                // Procesar delimitadores (punto y coma)
                else if (current == ';')
                {
                    _tokens.Add(new Token(TokenType.Delimiter, ";"));
                    _position++;
                }
                // Procesar comentarios (inicio con '(*' y fin con '*)')
                else if (current == '(' && nextChar == "*")
                {
                    _position += 2; // Saltar '(*'
                    // Ignorar el comentario hasta que se encuentre '*)'
                    while (_position < _input.Length && (_input[_position] != '*' || _input[_position + 1] != ')'))
                    {
                        _position++;
                    }
                    _position += 2; // Saltar '*)'
                }
                // Procesar paréntesis abiertos
                else if (current == '(')
                {
                    _tokens.Add(new Token(TokenType.LeftParen, "("));
                    _position++;
                }
                // Procesar paréntesis cerrados
                else if (current == ')')
                {
                    _tokens.Add(new Token(TokenType.RightParen, ")"));
                    _position++;
                }
                // Procesar llaves abiertas
                else if (current == '{')
                {
                    _tokens.Add(new Token(TokenType.LeftBrace, "{"));
                    _position++;
                }
                // Procesar llaves cerradas
                else if (current == '}')
                {
                    _tokens.Add(new Token(TokenType.RightBrace, "}"));
                    _position++;
                }
                // Procesar strings entre comillas dobles
                else if (current == '"')
                {
                    _position++; // Saltar la primera comilla
                    string str = "";
                    // Recolectar el contenido del string hasta encontrar la comilla de cierre
                    while (_position < _input.Length && _input[_position] != '"')
                    {
                        str += current;
                    }
                    // Verificar si se llegó al final del string sin cerrar
                    if (_position == _input.Length - 1 && nextChar != '"')
                    {
                        Console.WriteLine("Se espera cerrar string con doble comilla.");
                    }
                    _position++; // Saltar la comilla de cierre
                    _tokens.Add(new Token(TokenType.String, str));
                }
                // Procesar operadores como '=', '<', '>', '<=', '>=', '<>'
                else if (current == '=' || current == '<' || current == '>')
                {
                    if (current == '<' && nextChar == ">")
                    {
                        _position += 2;
                        _tokens.Add(new Token(TokenType.Operator, "<>"));
                    }
                    else if (current == '>' && nextChar == "=")
                    {
                        _position += 2;
                        _tokens.Add(new Token(TokenType.Operator, ">="));
                    }
                    else if (current == '<' && nextChar == "=")
                    {
                        _position += 2;
                        _tokens.Add(new Token(TokenType.Operator, "<="));
                    }
                    else
                    {
                        _tokens.Add(new Token(TokenType.Operator, current.ToString()));
                        _position++;
                    }
                }
                // Procesar operadores aritméticos simples (+, -, *)
                else if (current == '+')
                {
                    _tokens.Add(new Token(TokenType.Operator, "+"));
                    _position++;
                }
                else if (current == '-')
                {
                    _tokens.Add(new Token(TokenType.Operator, "-"));
                    _position++;
                }
                else if (current == '*')
                {
                    _tokens.Add(new Token(TokenType.Operator, "*"));
                    _position++;
                }
                // Manejo de símbolos no reconocidos
                else
                {
                    Console.WriteLine($"Símbolo no reconocido: {current}");
                }
            }

            // Agregar el token EOF al final del input
            _tokens.Add(new Token(TokenType.EOF, "EOF"));
        }

        // Obtener la lista de tokens generados
        public List<Token> GetTokens() => _tokens;
    }

    // Definición de los tipos de tokens
    public enum TokenType
    {
        Number, String, Identifier, Keyword, Operator, Delimiter, LeftParen, RightParen, LeftBrace, RightBrace, EOF
    }

    // Clase para representar un token
    public class Token
    {
        public TokenType Type { get; }
        public string Value { get; }

        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }
    }
}
