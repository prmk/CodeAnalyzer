///////////////////////////////////////////////////////////////////////
// Author:         Pramukh Shenoy                                    //
///////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * ------------------
 * This module is responsible for handling all the events responsible in the project
 * 
 * It acts as an interaction between the different packages
 *  
 */
using System;

namespace CodeAnalyzer
{
    public class Executive
    {
        private CommandLineParser clp;
        private FileMngr fm;
        private Analyzer analyzer;
        private ConsoleDisplay consoleDisplay;
        private string[] args;

        public string[] Args
        {
            get { return args; }
            set { args = value; }
        }

        public Executive()
        {
            clp = new CommandLineParser();
            fm = new FileMngr();
            analyzer = new Analyzer();
            consoleDisplay = new ConsoleDisplay();
        }

        /// <summary>
        /// calls all the core logic of the tool
        /// </summary>
        /// <returns></returns>
        public bool runExecutive()
        {
            clp.Args = args;
            clp.parseCommandLines();

            fm.Patterns = clp.Patterns;
            fm.Recurse = clp.Recurse;
            foreach (string path in clp.Paths)
                fm.findFiles(path);

            analyzer.Files = fm.getFiles();
            analyzer.Relationshipflag = clp.Relationships;
            analyzer.analyze();

            consoleDisplay.showComplexity();

            if (clp.Relationships)
                consoleDisplay.showRelationships();

            return true;
        }
    }

#if(TEST_EXECUTIVE)
    class TestExecutive
    {
        static void Main(string[] args)
        {
            Executive ex = new Executive();
            ex.Args = args;
            ex.runExecutive();

            Console.WriteLine("\n\nPress any key to continue");
            Console.ReadLine();
        }
    }
#endif
}
