﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Analizador_Lexico_Compiladores
{
    public class Lexer
    {
        private string _input;
        private int _position;
        private List<Token> _tokens;
        private Dictionary<string, TokenType> _keywords;
    //porfavor ayudenme
        public Lexer(string input)
        {
            _input = input;
            _tokens = new List<Token>();
            _position = 0;

            // Definir los keywords de la gramática
            _keywords = new Dictionary<string, TokenType> 
            {
                {"PROGRAM", TokenType.PROGRAM},
                {"INCLUDE", TokenType.INCLUDE},
                {"CONST", TokenType.CONST},
                {"TYPE", TokenType.TYPE},
                {"VAR", TokenType.VAR},
                {"RECORD", TokenType.RECORD},
                {"ARRAY", TokenType.ARRAY},
                {"OF", TokenType.OF},
                {"PROCEDURE", TokenType.PROCEDURE},
                {"FUNCTION", TokenType.FUNCTION},
                {"IF", TokenType.IF},
                {"THEN", TokenType.THEN},
                {"ELSE", TokenType.ELSE},
                {"FOR", TokenType.FOR},
                {"TO", TokenType.TO},
                {"WHILE", TokenType.WHILE},
                {"DO", TokenType.DO},
                {"EXIT", TokenType.EXIT},
                {"END", TokenType.END},
                {"PRINTLN", TokenType.PRINTLN},
                {"READLN", TokenType.READLN},
                {"CASE", TokenType.CASE},
                {"BREAK", TokenType.BREAK},
                {"DOWNTO", TokenType.DOWNTO},
                {"INTEGER", TokenType.INTEGER},
                {"REAL", TokenType.REAL},
                {"BOOLEAN", TokenType.BOOLEAN},
                {"STRING", TokenType.STRING},
                {"BEGIN", TokenType.BEGIN}
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
                        _position++;
                        current =_input[_position];
                    }                    
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
                        _position++;
                        current = _input[_position];
                    }                    
                    // Verificar si es una keyword
                    if (_keywords.ContainsKey(identifier))
                    {
                        _tokens.Add(new Token(_keywords[identifier], identifier));
                    }
                    // Verificar si es un operador (MOD, DIV, NOT)
                    else if (identifier == "MOD" || identifier == "DIV" || identifier == "NOT")
                    {
                        _tokens.Add(new Token(TokenType.multiplicative_operator, identifier));
                    }
                    // Verificar si es un operador OR
                    else if (identifier == "OR")
                    {
                        _tokens.Add(new Token(TokenType.additive_operator, identifier));
                    }
                    // Verificar si es un operador AND
                    else if (identifier == "AND")
                    {
                        _tokens.Add(new Token(TokenType.multiplicative_operator, identifier));
                    }
                    // Verificar si es una constante booleana
                    else if (identifier == "TRUE" || identifier == "FALSE")
                    {
                        _tokens.Add(new Token(TokenType.boolean_constant, identifier));
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
                // Procesar strings entre comillas simples
                else if (Convert.ToString(current) == "'")
                {
                    _position++; // Saltar la primera comilla
                    string str = "";
                    // Recolectar el contenido del string hasta encontrar la comilla de cierre
                    while (_position < _input.Length && Convert.ToString(current) != "'")
                    {
                        str += current;
                        current = _input[_position];
                        _position++;
                    }
                    // Verificar si se llegó al final del string sin cerrar
                    if (_position == _input.Length - 1 && Convert.ToString(_input[_position++]) != "'")
                    {
                        Console.WriteLine("Se espera cerrar string con comilla.");
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
                        _tokens.Add(new Token(TokenType.relational_operator, "<>"));
                    }
                    else if (current == '>' && nextChar == "=")
                    {
                        _position += 2;
                        _tokens.Add(new Token(TokenType.relational_operator, ">="));
                    }
                    else if (current == '<' && nextChar == "=")
                    {
                        _position += 2;
                        _tokens.Add(new Token(TokenType.relational_operator, "<="));
                    }
                    else
                    {
                        _tokens.Add(new Token(TokenType.relational_operator, current.ToString()));
                        _position++;
                    }
                }
                // Procesar operadores aritméticos simples (+, -, *)
                else if (current == '+')
                {
                    _tokens.Add(new Token(TokenType.additive_operator, "+"));
                    _position++;
                }
                else if (current == '-')
                {
                    _tokens.Add(new Token(TokenType.additive_operator, "-"));
                    _position++;
                }
                else if (current == '*')
                {
                    _tokens.Add(new Token(TokenType.multiplicative_operator, "*"));
                    _position++;
                }
                else if (current == '/')
                {
                    _tokens.Add(new Token(TokenType.multiplicative_operator, "/"));
                    _position++;
                }
                else if (current == '|')
                {
                    _tokens.Add(new Token(TokenType.additive_operator, "OR"));
                    _position++;
                }
                else if (current == '&' || current == 'ε')
                {
                    _tokens.Add(new Token(TokenType.Nullable, "ε"));
                    _position++;
                }
                else if (".,:".Contains(current))
                {
                    _tokens.Add(new Token(TokenType.Punctuation, current.ToString()));
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
        Number, String, Identifier, Keyword, Delimiter, LeftParen, RightParen, LeftBrace, RightBrace, EOF, Punctuation,
        Nullable, relational_operator, additive_operator, multiplicative_operator, boolean_constant, PROGRAM, END, BEGIN, 
        INCLUDE, CONST, TYPE, VAR, RECORD, ARRAY, OF, PROCEDURE, FUNCTION, IF, THEN, ELSE, FOR, TO, WHILE, DO, EXIT, PRINTLN, 
        READLN, CASE, BREAK, DOWNTO, INTEGER, REAL, BOOLEAN, STRING
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
