using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace MIX2.Acquisition
{
    public abstract class BaseConnection
    {
        protected NetworkCredential credential;
        protected Dictionary<string, object> parameters;

        protected  void ExtractArgs(string ConnectionString)
        {
            if( this.parameters == null )
                this.parameters = new Dictionary<string, object>();

            string[] args = ConnectionString.Split(';');

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Trim() != string.Empty)
                {
                    int off = args[i].IndexOf('=');

                    if (off > -1)
                    {
                        string arg = args[i].Substring(0, off).Trim();
                        string val = args[i].Substring(off + 1).Trim();

                        string param = string.Empty;

                        switch (val)
                        {
                            case "[$Domain]":
                                param = this.credential.Domain;
                                break;

                            case "[$Password]":
                                param = this.credential.Password;
                                break;

                            case "[$Username]":
                                param = this.credential.UserName;
                                break;

                            default:
                                param = val;
                                break;
                        }

                        this.parameters.Add(arg, param);
                    }
                }

            }
        }

        protected object[] ReplaceParameters(object[] parameters)
        {
            if (parameters == null)
                throw new Exception("Parameters cannot be null.");

            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] == null)
                    throw new Exception("Parameter " + i.ToString() + " is null.");

                Type t = parameters[i].GetType();

                if (t.FullName == "System.String")
                {
                    string s = (string)parameters[i];

                    foreach (string key in this.parameters.Keys)
                    {
                        if (this.parameters[key] == null)
                            throw new Exception("The parameter " + key + " is not defined in the conection string of the configuration file.");

                        s = s.Replace("[$" + key + "]", this.parameters[key].ToString());
                    }

                    parameters[i] = s;
                }


                //Console.WriteLine(Parameters[i].ToString());
            }

            return parameters;
        }
    }
}
