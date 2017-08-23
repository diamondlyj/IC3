using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MIX2.Acquisition
{
    public class Token
    {
        private string lexeme;
        private TokenType tokenType;

        public Token(string lexeme, TokenType tokenType)
        {
            this.lexeme = lexeme;
            this.tokenType = tokenType;
        }

        public string Lexeme
        {
            get
            {
                return this.lexeme;
            }
        }

        public TokenType TokenType
        {
            get { return this.tokenType; }
        }
    }

    public enum TokenType
    {
        Move, Attribute, Function, String, Separator, OpenBracket, CloseBracket, EOL
    }

    public class Tokenizer
    {
        public static Token[] Tokenize( string script )
        {
            char[] c = script.ToCharArray();

            List<Token> tokens = new List<Token>();

            int i = 0;

            while (i < c.Length)
            {
                string lexeme = string.Empty;
                
                switch( c[i] )
                {
                    case '(':
                        tokens.Add(new Token("(", TokenType.OpenBracket));
                        break;

                    case ')':
                        tokens.Add(new Token(")", TokenType.CloseBracket));
                        break;

                    case '/':
                        tokens.Add(new Token("/", TokenType.EOL));
                        break;

                    case ',':
                        tokens.Add(new Token(",", TokenType.Separator));
                        break;

                    case '.':
                        while (i < c.Length && c[i] == '.')
                        {
                            lexeme += c[i].ToString();

                            i++;
                        }

                        i--;

                        tokens.Add(new Token(lexeme, TokenType.Move));
                        break;

                    case '@':
                        i++;

                        //Would be better to just check if c is an alphanumeric character (A-z or 0-9)
                        while (i < c.Length && c[i] != '/' && c[i] != '(' && c[i] != ')' && c[i] != ',')
                        {
                            lexeme += c[i].ToString();
                            i++;
                        }

                        i--;

                        tokens.Add(new Token(lexeme,TokenType.Attribute));
                        break;

                    case '\'':
                        i++;
                        
                        while (i < c.Length && ( c[i] != '\'' || ( c[i] =='\'' && (i-1) >= 0 && c[i-1] == '\\' ) ))
                        {
                            lexeme += c[i].ToString();
                            i++;
                        }

                        tokens.Add(new Token(lexeme, TokenType.String));
                        break;

                        
                    default:

                        //Would be better to just check if c is an alphanumeric character (A-z or 0-9)
                        while (i < c.Length && c[i] != '.' && c[i] != '/' && c[i] != '(' && c[i] != ')' && c[i] != ',')
                        {
                            lexeme += c[i].ToString();
                            i++;
                        }



                        i--;

                        tokens.Add(new Token(lexeme, TokenType.Function));

                        /*
                        while (i < c.Length && c[i] != '/')
                        {
                            lexeme = string.Empty;

                            while (i < c.Length && c[i] != ',' && c[i] != ')')
                            {
                                lexeme += c[i].ToString();
                                i++;
                            }

                            tokens.Add(new Token(lexeme, TokenType.Argument));
                            i++;
                        }
                        */

                        break;

                        //throw new Exception("Failed to map " + prop.Name + ". Could not interpret query:" + query.Substring(i);
                        
                }

                i++;
            }

            return tokens.ToArray();
        }
    }
}
