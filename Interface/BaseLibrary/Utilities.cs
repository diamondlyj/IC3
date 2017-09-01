using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.V2.BaseLibrary
{
    public class Utilities
    {
        //comment
        /// <summary>
        /// constructs a path given a series of objects that can be made
        /// into strings
        /// </summary>
        /// <param name="args">any number of primitive objects like strings, ints or bools. first one should be https://...</param>
        /// <returns></returns>
        public static string MakePath(params object[] args)
        {
            if (!(args[0].ToString().Contains("http://") || (args[0].ToString().Contains("https://"))))
                throw new ArgumentException("path doesn't start with http or https");
            string url = "";
            for (int i = 0; i < args.Length; i++)
            {
                if (i > 0)
                    url += "/";
                url += args[i].ToString();
            }
        //blah blah blah
            return url;
        }

    }
}
