using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
            {"VAR", TokenType.Keyword},
            {"INTEGER", TokenType.Keyword},
            {"REAL", TokenType.Keyword},
            {"BOOLEAN", TokenType.Keyword},
            {"STRING", TokenType.Keyword},
            {"BEGIN", TokenType.Keyword},
            {"END", TokenType.Keyword},
            // Agregar el resto de tus keywords aquí...
        };

        Tokenize();
    }

    private void Tokenize()
    {
        while (_position < _input.Length)
        {
            if (Char.IsWhiteSpace(_input[_position]))
            {
                _position++; // Ignora espacios en blanco
            }
            else if (Char.IsDigit(_input[_position]))
            {
                string number = "";
                while (_position < _input.Length && Char.IsDigit(_input[_position]))
                {
                    number += _input[_position++];
                }
                _tokens.Add(new Token(TokenType.Number, number));
            }
            else if (Char.IsLetter(_input[_position]))
            {
                string identifier = "";
                while (_position < _input.Length && (Char.IsLetterOrDigit(_input[_position]) || _input[_position] == '_'))
                {
                    identifier += _input[_position++];
                }

                if (_keywords.ContainsKey(identifier))
                {
                    _tokens.Add(new Token(_keywords[identifier], identifier));
                }
                else
                {
                    _tokens.Add(new Token(TokenType.Identifier, identifier));
                }
            }
            else
            {
                char current = _input[_position];

                switch (current)
                {
                    case '+':
                    case '-':
                    case '*':
                    case '/':
                    case '=':
                    case ';':
                    case ':':
                        _tokens.Add(new Token(TokenType.Operator, current.ToString()));
                        break;
                    case '(':
                        _tokens.Add(new Token(TokenType.LeftParen, "("));
                        break;
                    case ')':
                        _tokens.Add(new Token(TokenType.RightParen, ")"));
                        break;
                    default:
                        Console.WriteLine($"Símbolo no reconocido: {current}");
                        break;
                }

                _position++;
            }
        }
        _tokens.Add(new Token(TokenType.EOF, "EOF"));
    }

    public List<Token> GetTokens() => _tokens;
}

public enum TokenType
{
    Number, Identifier, Keyword, Operator, LeftParen, RightParen, EOF
}

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
