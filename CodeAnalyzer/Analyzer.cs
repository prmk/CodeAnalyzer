///////////////////////////////////////////////////////////////////////
// Author:         Pramukh Shenoy                                    //
///////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * ------------------
 * This module Analyses the specified file
 * 
 * It requires the following 2 files
 *   Parser  - a collection of IRules
 *   RulesAndActions - collections of Rules and Actions
 */

using CodeAnalysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace CodeAnalyzer
{
    public class Analyzer
    {
        private List<string> files;

        public List<string> Files
        {
            get { return files; }
            set { files = value; }
        }
        private bool relationshipflag;

        public bool Relationshipflag
        {
            get { return relationshipflag; }
            set { relationshipflag = value; }
        }

        //read the list of files, one by one and calls BuildCodeAnalyzer and parser functions
        public void analyze()
        {
            Console.Write("\n  CODE ANALYZER");
            Console.Write("\n ======================\n");

            CSsemi.CSemiExp semi = new CSsemi.CSemiExp();
            semi.displayNewLines = false;

            foreach (object file in files)
            {
                Console.Write("\n\n  Processing file {0}\n", file as string);

                if (!semi.open(file as string))
                {
                    Console.Write("\n  Can't open {0}\n\n", file);
                    return;
                }

                Console.Write("\n  Type and Function Analysis");
                Console.Write("\n ----------------------------\n");

                BuildCodeAnalyzer builder = new BuildCodeAnalyzer(semi);
                CodeAnalysis.Parser parser = builder.build();

                try
                {
                    while (semi.getSemi())
                        parser.parse(semi);
                }
                catch (Exception ex)
                {
                    Console.Write("\n\n  {0}\n", ex.Message);
                }
                semi.close();

                if (relationshipflag)
                {
                    semi = new CSsemi.CSemiExp();
                    semi.displayNewLines = false;
                    if (!semi.open(file as string))
                    {
                        Console.Write("\n  Can't open {0}\n\n", file);
                        return;
                    }

                    BuildCodeAnalyzerRelationships builderreln = new BuildCodeAnalyzerRelationships(semi);
                    parser = builderreln.build();

                    try
                    {
                        while (semi.getSemi())
                            parser.parse(semi);
                    }
                    catch (Exception ex)
                    {
                        Console.Write("\n\n  {0}\n", ex.Message);
                    }
                }
                semi.close();
            }
        }
    }

#if(TEST_ANALYZE)
    class TestAnalyze
    {
        static void Main(string[] args)
        {
            List<string> paths = new List<string>();
            List<string> files = new List<string>();

            for (int i = 0; i < args.Length; i++)
            {
                string[] newFiles = Directory.GetFiles(args[i], "*.cs");
                ArrayList filesAL = new ArrayList();
                for (int j = 0; j < newFiles.Length; ++j)
                {
                    if (!newFiles[j].Contains("Temporary") && !newFiles[j].Contains("AssemblyInfo.cs"))
                        filesAL.Add(Path.GetFullPath(newFiles[j]));
                }
                files.AddRange((String[])filesAL.ToArray(typeof(string)));
            }

            Analyzer analyze = new Analyzer();
            analyze.Files = files;
            analyze.analyze();
        }
    }
#endif
}
