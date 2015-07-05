///////////////////////////////////////////////////////////////////////
// Author:         Pramukh Shenoy                                    //
///////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * ------------------
 * This module parses the command line arguements
 * and searches for /S, /X, /R, pattern type and path of the file to be analyzed
 *   
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeAnalyzer
{
    public class CommandLineParser
    {
        private bool recurse;

        public bool Recurse
        {
            get { return recurse; }
            set { recurse = value; }
        }
        private bool relationships;

        public bool Relationships
        {
            get { return relationships; }
            set { relationships = value; }
        }
        private bool xml;

        public bool Xml
        {
            get { return xml; }
            set { xml = value; }
        }
        private List<string> patterns;

        public List<string> Patterns
        {
            get { return patterns; }
            set { patterns = value; }
        }
        private List<string> paths;

        public List<string> Paths
        {
            get { return paths; }
            set { paths = value; }
        }
        private string[] args;

        public string[] Args
        {
            get { return args; }
            set { args = value; }
        }

        public CommandLineParser()
        {
            recurse = false;
            relationships = false;
            xml = false;
            patterns = new List<string>();
            paths = new List<string>();
        }

        /// <summary>
        /// reads the commands line arguements provided by the user
        /// it then parses the same and sends it back to the calling method
        /// </summary>
        /// <returns>returns failure if no arguements are specified, else returns true</returns>
        public bool parseCommandLines()
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No Command Lines specified");
                return false;
            }
            int indexofS = IndexOf(args, "/S");
            int indexofX = IndexOf(args, "/X");
            int indexofR = IndexOf(args, "/R");
            int indexofpattern = IndexOf(args, "*.");

            if (indexofS != -1)
                recurse = true;
            if (indexofX != -1)
                xml = true;
            if (indexofR != -1)
                relationships = true;
            if (indexofpattern != -1)
                patterns.Add(args[indexofpattern]);
            else
                patterns.Add("*.cs");

            int index = MoreMath.Max(indexofR, indexofS, indexofX, indexofpattern);
            for (int i = index + 1; i < args.Length; i++)
                paths.Add(args[i]);

            return true;
        }


        /// <summary>
        /// looks for the index of value in args
        /// </summary>
        /// <param name="args">the array which has to be searched in</param>
        /// <param name="value">the value which has to be searched for</param>
        /// <returns>the index of value in args[]</returns>
        public int IndexOf(string[] args, string value)
        {
            int itemIndex = -1;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Contains(value))
                {
                    itemIndex = i;
                    break;
                }
            }
            return itemIndex;
        }

        public override string ToString()
        {
            StringBuilder temp = new StringBuilder();
            temp.Append(String.Format("\n/X present: {0}", this.xml));
            temp.Append(String.Format("\n/S present: {0}", this.recurse));
            temp.Append(String.Format("\n/R present: {0}", this.relationships));
            temp.Append("\npattern name specified:");
            foreach (string pattern in this.patterns)
                temp.Append(String.Format(" {0} ", pattern));
            temp.Append("\nPaths are:");
            foreach (string path in this.paths)
                temp.Append(String.Format("{0} ", path));

            return temp.ToString();
        }
    }

    /// <summary>
    /// searches for maximum value in a given integer array
    /// </summary>
    public static class MoreMath
    {
        public static int Max(params int[] values)
        {
            return Enumerable.Max(values);
        }
    }


#if(TEST_CLPARSER)
    class TestCLParser
    {
        static void Main(string[] args)
        {
            CommandLineParser clp = new CommandLineParser();
            clp.Args = args;
            Console.WriteLine("Command Line Arguements ");
            foreach (string arg in args)
            {
                Console.Write("  {0}", arg);
            }
            Console.Write("\n");
            if (clp.parseCommandLines())
            {
                Console.WriteLine("\nParsed Command Line Data");
                Console.WriteLine(clp.ToString());
            }
            else
                Console.WriteLine("Nothing to Parse");

            Console.ReadLine();
        }
    }
#endif
}
