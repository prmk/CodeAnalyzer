using CodeAnalysis;
using System;
using System.Collections.Generic;


namespace CodeAnalyzer
{
    public class ConsoleDisplay
    {
        /// <summary>
        /// displays the scope count and the lines count for the analyzed data
        /// </summary>
        public void showComplexity()
        {
            try
            {
                Console.WriteLine("\n\n  COMPLEXITY SUMMARY");
                Console.WriteLine("===============");
                Repository rep = Repository.getInstance();
                List<Elem> table = rep.analyzedata;
                Console.WriteLine("\nType             Name                Begin       End         Lines      Scopes");
                Console.Write("==============================================================================");
                foreach (Elem e in table)
                {
                    if (e.type.Equals("namespace") || e.type.Equals("class") || e.type.Equals("interface"))
                    {
                        Console.Write("\n{0,10} {1,20} {2,8} {3,11}", e.type, e.name, e.begin, e.end);
                        Console.Write("{0,13}", (e.end - e.begin + 1));
                    }
                    else if (e.type.Equals("function"))
                    {
                        Console.Write("\n{0,10} {1,20} {2,8} {3,11}", e.type, e.name, e.begin, e.end);
                        Console.Write("{0,13} {1,8}", (e.end - e.begin + 1), e.scopecount);
                    }
                }
                Console.WriteLine();
                Console.WriteLine();
            }
            catch (Exception)
            {
                Console.WriteLine("Error occurred while displaying complexity");
            }
        }

        /// <summary>
        /// displays all the relationships analyzed for the provided file
        /// </summary>
        public void showRelationships()
        {
            try
            {
                Repository rep = Repository.getInstance();
                Console.WriteLine("RELATIONSHIP ANALYSIS");
                Console.WriteLine("=====================");
                showInheritance(rep);
                showAggregation(rep);
                showComposition(rep);
                showUsing(rep);
            }
            catch (Exception)
            {
                Console.WriteLine("Error occurred while displaying relationships");
            }
        }

        public void showInheritance(Repository rep)
        {
            List<InheritanceElem> inheritancetable = rep.inheritancedata;
            Console.WriteLine("\n\nInheritance Analysis");
            Console.WriteLine("\nParent       Children Count      Children");
            Console.Write("========================================");
            foreach (InheritanceElem ie in inheritancetable)
            {
                Console.Write("\n{0,2} {1,15}           ", ie.parent, ie.childcount);
                foreach (string child in ie.children)
                    Console.Write("{0,4}, ", child);
            }
            Console.WriteLine();
        }

        public void showAggregation(Repository rep)
        {
            List<AggregationElem> aggregatedtable = rep.aggregationdata;
            Console.WriteLine("\n\nAggregation Analysis");
            Console.WriteLine("\nAggregator       Type           Aggregated");
            Console.Write("========================================");
            foreach (AggregationElem ae in aggregatedtable)
            {
                Console.Write("\n{0,2} {1,15}           ", ae.aggregator, ae.type);
                foreach (string agg in ae.aggregated)
                    Console.Write("{0,4}, ", agg);
            }
            Console.WriteLine();
        }

        public void showComposition(Repository rep)
        {
            List<CompositionElem> compositiontable = rep.compositiondata;
            Console.WriteLine("\n\nComposition Analysis");
            Console.WriteLine("\nCompositor       Type        Composed Element");
            Console.Write("========================================");
            foreach (CompositionElem ce in compositiontable)
            {
                Console.Write("\n{0,2} {1,15}           ", ce.compositor, ce.type);
                foreach (string comp in ce.composedelement)
                    Console.Write("{0,4}, ", comp);
            }
            Console.WriteLine();
        }

        public void showUsing(Repository rep)
        {
            List<UsingElem> usingtable = rep.usingdata;
            Console.WriteLine("\n\nUsing Analysis");
            Console.WriteLine("\nClass Name     in Function         Using(type)");
            Console.Write("========================================");
            foreach (UsingElem ue in usingtable)
            {
                Console.Write("\n{0,2} {1,15}           ", ue.parent, ue.containingfunction);
                foreach (TypeDetails elt in ue.typeslist)
                    Console.Write("{0,1}({1,2}), ", elt.type, elt.usedtypename);
            }
            Console.WriteLine();
        }
    }

    public class TestConsoleDisplay
    {
        public static void main(String[] args)
        {

        }
    }
}
