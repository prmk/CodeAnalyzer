///////////////////////////////////////////////////////////////////////
// Parser.cs - Parser detects code constructs defined by rules       //
// ver 1.3                                                           //
// Language:    C#, 2008, .Net Framework 4.0                         //
// Platform:    Dell Precision T7400, Win7, SP1                      //
// Application: Demonstration for CSE681, Project #2, Fall 2011      //
// Author:      Jim Fawcett, CST 4-187, Syracuse University          //
//              (315) 443-3948, jfawcett@twcny.rr.com                //
///////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * ------------------
 * This module defines the following class:
 *   Parser  - a collection of IRules
 */
/* Required Files:
 *   IRulesAndActions.cs, RulesAndActions.cs, Parser.cs, Semi.cs, Toker.cs
 *   
 * Build command:
 *   csc /D:TEST_PARSER Parser.cs IRulesAndActions.cs RulesAndActions.cs \
 *                      Semi.cs Toker.cs
 *   
 * Maintenance History:
 * --------------------
 * ver 1.3 : 24 Sep 2011
 * - Added exception handling for exceptions thrown while parsing.
 *   This was done because Toker now throws if it encounters a
 *   string containing @".
 * - RulesAndActions were modified to fix bugs reported recently
 * ver 1.2 : 20 Sep 2011
 * - removed old stack, now replaced by ScopeStack
 * ver 1.1 : 11 Sep 2011
 * - added comments to parse function
 * ver 1.0 : 28 Aug 2011
 * - first release
 */
using System;
using System.Collections.Generic;
using System.IO;

namespace CodeAnalysis
{
    /////////////////////////////////////////////////////////
    // rule-based parser used for code analysis

    public class Parser
    {
		for(;;)
		{
			while()
			{
				try
				{}
				catch(){}
				catch
				{}
			}
			// switch()
			// {
				// case a:
				// case b: if
				// {}
						
			// }
		}
        private List<IRule> Rules;

        public Parser()
        {
            Rules = new List<IRule>();
			{ test; }
        }
       
        public void parse(CSsemi.CSemiExp semi)
        {
			for(;;)
			{}
            foreach (IRule rule in Rules)
            {
                if (rule.test(semi))
                    break;
            }
        }
    }
}
