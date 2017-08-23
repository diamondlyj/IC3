using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using ActiveDs;

namespace MIX2.Acquisition.LDAP
{

    public class Expression
    {
        protected List<Expression> children;
        protected object value;

        public Expression()
        {
            this.children = new List<Expression>();
        }

        virtual public void Evaluate(DirectoryEntry entry)
        {
            this.value = string.Empty;

            foreach (Expression exp in this.children)
                exp.Evaluate(entry);

            if (this.children.Count == 1)
                this.value = this.children[0].Value;
            else
            {
                object[] vals = new object[this.children.Count];

                for (int i = 0; i < this.children.Count; i++)
                    vals[i] = this.children[i].Value;

                this.value = vals;
            }
        }

        public List<Expression> Children
        {
            get { return this.children; }
        }

        public object Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
    }

    public class AttributeExpression: Expression
    {
        override public void Evaluate(DirectoryEntry entry)
        {
            string propName = this.children[0].Value.ToString();

            try
            {
                if (entry.Properties.Contains(propName))
                {
                    this.value = entry.Properties[propName].Value;
                }
                else
                    this.value = string.Empty;
            }
            catch
            {
                this.value = string.Empty;
            }
        }
    }

    public class ConvertExpression : Expression
    {
        override public void Evaluate(DirectoryEntry entry)
        {
            if (this.children.Count > 2)
                throw new Exception("Convert only takes 2 arguments, the value to transform and the original format of the value. Example: convert(@myvalue,FileTime)");

            base.Evaluate(entry);

            string dataFormat = this.children[1].Value.ToString();

            //Console.WriteLine("Data format: " + dataFormat);
            //Console.WriteLine("Converting " + this.children[0].Value.ToString());

            switch (dataFormat)
            {
                case "FileTime":
                    long val = 0;

                    try
                    {
                        LargeInteger li = (LargeInteger)this.children[0].Value;
                        val = (((long)(li.HighPart) << 32) + (long)li.LowPart);
                    }
                    catch
                    {
                        this.value = this.children[0].Value;
                        return;
                    }

                    this.value = DateTime.FromFileTime(val);
                    break;

                default:
                    throw new Exception("Cannot convert from type " + dataFormat + ".");
            }
        
        }
    }

    public class ExtractExpression : Expression
    {
        override public void Evaluate(DirectoryEntry entry)
        {
            if (this.children.Count > 2)
                throw new Exception("Extract only takes 2 arguments, the value to extract from and a regular expression of what to extract. Example: extract(@myvalue,'@/w+^')");

            base.Evaluate(entry);

            object[] vals = new object[1];

            Type t = this.children[0].Value.GetType();

            if (t == typeof(object[]))
                vals = (object[])this.children[0].Value;
            else
                vals[0] = this.children[0].Value;

            System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex(this.children[1].Value.ToString());

            for (int i = 0; i < vals.Length; i++)
            {
                string val = Converter.ToString(vals[i]);

                //Console.WriteLine(val);

                System.Text.RegularExpressions.Match ma = re.Match(val);

                //Console.WriteLine(ma.Value);

                if (ma.Success)
                    vals[i] = ma.ToString();
                else
                    vals[i] = string.Empty;
            }

            if (t == typeof(object[]))
                this.value = vals;
            else
                this.value = vals[0];
        }
    }


    public class MoveUpExpression : Expression
    {
        override public void Evaluate(DirectoryEntry entry)
        {
            //Console.WriteLine("Moving up from " + entry.Name);

            try
            {
                entry = entry.Parent;
            }
            catch
            {
            }

            //Console.WriteLine("To " + entry.Name);
            //Console.WriteLine("Number of children:" + this.children.Count.ToString());

            base.Evaluate(entry);
        }
    }

    public class StringExpression : Expression
    {
        override public void Evaluate(DirectoryEntry entry)
        {            
        }
    }

    public class SubstringExpression : Expression
    {
        override public void Evaluate(DirectoryEntry entry)
        {
            if (this.children.Count > 3)
                throw new Exception("Extract takes 2 or 3 arguments, the value to extract the substring from, the start index and the length of the substring (optional). Example: substring(@myvalue,2,6)");

            base.Evaluate(entry);

            object[] vals = new object[1];

            Type t = this.children[0].Value.GetType();

            if (t == typeof(object[]))
                vals = (object[])this.children[0].Value;
            else
                vals[0] = this.children[0].Value;

            int offset = Convert.ToInt32( this.children[1].Value );
            int length = -1;

            if( this.Children.Count == 3 )
                length = Convert.ToInt32(this.children[2].Value);

            for (int i = 0; i < vals.Length; i++)
            {
                string val = Converter.ToString(vals[i]);

                if (length > -1 && offset + length <= val.Length)
                    vals[i] = val.Substring(offset, length);
                else if (offset < val.Length)
                    vals[i] = val.Substring(offset);
                else
                    vals[i] = string.Empty;
            }

            if (t == typeof(object[]))
                this.value = vals;
            else
                this.value = vals[0];
        }
    }

    public class Parser
    {

        public Parser()
        {
        }

        public Expression Parse(Token[] tokens)
        {
            Expression exp = new Expression();

            int n = 0;

            while (n < tokens.Length)
            {
                /*
                Console.WriteLine();
                Console.WriteLine(n.ToString());
                Console.WriteLine(tokens[n].TokenType.ToString());
                Console.WriteLine(tokens[n].Lexeme);
                Console.WriteLine();
                */

                if (tokens[n].TokenType == TokenType.Move)
                {
                    string lexeme = tokens[n].Lexeme;

                    if (lexeme == "..")
                    {
                        exp = new MoveUpExpression();
                    }

                    n++;

                    if (n < tokens.Length)
                    {

                        int l = tokens.Length - n;

                        Token[] subTokens = new Token[l];

                        int j = 0;

                        for (int i = n; i < tokens.Length; i++)
                        {
                            subTokens[j] = tokens[i];
                            j++;
                        }

                        exp.Children.Add(Parse(subTokens));
                    }

                    return exp;
                }

                else if (tokens[n].TokenType == TokenType.String)
                {
                    exp = new StringExpression();
                    exp.Value = tokens[n].Lexeme;

                    return exp;
                }
                else if (tokens[n].TokenType == TokenType.Attribute)
                {
                    exp = new AttributeExpression();

                    StringExpression child = new StringExpression();
                    child.Value = tokens[n].Lexeme;
                    exp.Children.Add(child);

                    return exp;
                }
                else if (tokens[n].TokenType == TokenType.Function)
                {

                    switch (tokens[n].Lexeme)
                    {
                        case "extract":
                            exp = new ExtractExpression();
                            break;

                        case "convert":
                            exp = new ConvertExpression();
                            break;

                        case "substring":
                            exp = new SubstringExpression
                                ();
                            break;


                        default:
                            break;
                    }

                    n++;

                    if (n == tokens.Length || tokens[n].TokenType != TokenType.OpenBracket)
                    {
                        exp = new StringExpression();
                        exp.Value = tokens[n - 1].Lexeme;

                        return exp;
                    }

                    bool end = false;

                    while ( n < tokens.Length && !end )
                    {
                        n++;

                        int p = 1;
                        int start = n;

                        //Console.WriteLine();

                        while (n < tokens.Length && p > 0)
                        {
                            //Console.WriteLine(tokens[n].TokenType.ToString() + "; " + tokens[n].Lexeme);
                            //Console.WriteLine(p.ToString());

                            if (tokens[n].TokenType == TokenType.OpenBracket)
                                p++;
                            else if (tokens[n].TokenType == TokenType.CloseBracket || (p == 1 && tokens[n].TokenType == TokenType.Separator))
                                p--;

                            if (p > 0)
                                n++;
                            else if (tokens[n].TokenType == TokenType.CloseBracket)
                                end = true;
                        }

                        //Console.WriteLine("Argument " + argCount.ToString() + "; start=" + start.ToString() + "; n=" + n.ToString());

                        int len = n-start;

                        Token[] argTokens = new Token[len];
                        
                        int j=0;

                        for (int i = start; i < n; i++)
                        {
                            argTokens[j] = tokens[i];
                            //Console.WriteLine("Adding: " + tokens[i].Lexeme);

                            j++;
                        }

                        exp.Children.Add(Parse(argTokens));

                        //Console.WriteLine();
                    }

                    return exp;
                }


                n++;
            }

            throw new Exception( "Failed to parse script at " + tokens[n - 1].TokenType + ": " + tokens[n - 1].Lexeme);
        }
    }

}
