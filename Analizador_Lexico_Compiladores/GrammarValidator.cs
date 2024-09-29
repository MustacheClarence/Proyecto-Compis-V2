using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Analizador_Lexico_Compiladores
{
    public class GrammarValidator
    {
        public List<string> ErrorMessages { get; private set; }

        public GrammarValidator()
        {
            ErrorMessages = new List<string>();
        }

        public bool PrevalidateGrammar(string[] lines)
        {
            bool isValid = true;

            // Convertir el array de líneas en un string completo para aplicar regex
            string fullText = string.Join("\n", lines);

            // Definir los patrones para cada sección de la gramática
            string pattern_compiler = @"^\s*COMPILER\s+\w+\s*\.\s*$";
            string patter_units = @"^\s*UNITS\s+((Uinclude|Ugenera|ValConst|ValExp|RegArray|Proc|ValCond)\s*,\s*)*(Uinclude|Ugenera|ValConst|ValExp|RegArray|Proc|ValCond)\s*;\s*$";
            string pattern_sets = @"^\s*SETS\s+letter\s*=\s*'A'\.\.'Z'\s*\+\s*'a'\.\.'z'\s*\+\s*'_'\s*;\s*digit\s*=\s*'0'\.\.'9'\s*;\s*charset\s*=\s*chr\(32\)\.\.chr\(254\)\s*;\s*$";
            string pattern_tokens = @"^\s*TOKENS\s+"
                                  + @"(number\s*=\s*digit\s+digit\*;\s*)?"
                                  + @"(identifier\s*=\s*letter(\s*\|\s*digit)*\*\s*check\s*;\s*)?"
                                  + @"(str\s*=\s*charset\s+charset\s*\*;\s*)?"
                                  + @"((?:'=', '<>', '<', '>', '>=', '<='"
                                  + @"|\+|'-'|'OR'|';'|'*'|'AND'|'MOD'|'DIV'|'NOT')\s+(Left|Right)\s*;\s*)*$";
            string pattern_keywords = @"^\s*KEYWORDS\s*"
                                    + @"('(?:PROGRAM|INCLUDE|CONST|TYPE|VAR|RECORD|ARRAY|OF|PROCEDURE|FUNCTION|IF|THEN|ELSE|FOR|TO|WHILE|DO|EXIT|END|PRINTLN|READLN|CASE|BREAK|DOWNTO|INTEGER|REAL|BOOLEAN|STRING|BEGIN)')"
                                    + @"(\s*,\s*'(?:PROGRAM|INCLUDE|CONST|TYPE|VAR|RECORD|ARRAY|OF|PROCEDURE|FUNCTION|IF|THEN|ELSE|FOR|TO|WHILE|DO|EXIT|END|PRINTLN|READLN|CASE|BREAK|DOWNTO|INTEGER|REAL|BOOLEAN|STRING|BEGIN)')*\s*;";
            string pattern_productions = @"^\s*PRODUCTIONS\s*"
                                       + @"((<[\w\s]+>\s*=\s*(.*?\n?)+?\s*)+)$";

            // Valida secciones requeridas con expresiones regulares usando las variables definidas
            // Valida "COMPILER" y que tenga su identificador
            if (!Regex.IsMatch(fullText, pattern_compiler, RegexOptions.Multiline))
            {
                isValid = false;
                ErrorMessages.Add("Error: Se esperaba la sección 'COMPILER <nombre_compilador>.'");
            }
            // Valida "UNITS" y que tenga por lo menos una unidad valida
            if (!Regex.IsMatch(fullText, pattern_compiler, RegexOptions.Multiline))
            {
                isValid = false;
                ErrorMessages.Add("Error: Se esperaba la sección 'UNITS' seguido por lo menos una unidad valida.");
            }
            // Valida "SETS" y que tenga por lo menos una unidad valida
            if (!Regex.IsMatch(fullText, pattern_sets, RegexOptions.Multiline))
            {
                isValid = false;
                ErrorMessages.Add("Error: Se esperaba la sección 'SETS' seguido por lo menos un set valido.");
            }
            // Valida "TOKENS" y que tenga por lo menos una unidad valida
            if (!Regex.IsMatch(fullText, pattern_tokens, RegexOptions.Multiline))
            {
                isValid = false;
                ErrorMessages.Add("Error: Se esperaba la sección 'TOKENS' seguido por lo menos un token valido.");
            }
            // Valida "KEYWORDS" y que tenga por lo menos una unidad valida
            if (!Regex.IsMatch(fullText, pattern_keywords, RegexOptions.Multiline))
            {
                isValid = false;
                ErrorMessages.Add("Error: Se esperaba la sección 'KEYWORDS' seguido por lo menos una keyword valida.");
            }
            // Valida "PRODUCTIONS" y que tenga por lo menos una unidad valida
            if (!Regex.IsMatch(fullText, pattern_productions, RegexOptions.Multiline))
            {
                isValid = false;
                ErrorMessages.Add("Error: Se esperaba la sección 'PRODUCTIONS' seguido por lo menos una produccion valida.");
            }

            return isValid;
        }
    }
}
