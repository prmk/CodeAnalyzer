///////////////////////////////////////////////////////////////////////
// RulesAndActions.cs - Parser rules specific to an application      //
// Author:         Pramukh Shenoy                                    //
// Original Code:  Jim Fawcett, CST 4-187, Syracuse University       //
//              (315) 443-3948, jfawcett@twcny.rr.com                //
///////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * RulesAndActions package contains all of the Application specific
 * code required for most analysis tools.
 *
 * It defines the following Four rules which each have a
 * grammar construct detector and also a collection of IActions:
 *   - DetectNameSpace rule
 *   - DetectClass rule
 *   - DetectFunction rule
 *   - DetectScopeChange
 *   
 *   Three actions - some are specific to a parent rule:
 *   - Print
 *   - PrintFunction
 *   - PrintScope
 * 
 * The package also defines a Repository class for passing data between
 * actions and uses the services of a ScopeStack, defined in a package
 * of that name.
 *
 * Note:
 * This package does not have a test stub since it cannot execute
 * without requests from Parser.
 *  
 */
/* Required Files:
 *   IRuleAndAction.cs, RulesAndActions.cs, Parser.cs, ScopeStack.cs,
 *   Semi.cs, Toker.cs
 *   
 * Build command:
 *   csc /D:TEST_PARSER Parser.cs IRuleAndAction.cs RulesAndActions.cs \
 *                      ScopeStack.cs Semi.cs Toker.cs
 *   
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CodeAnalysis
{
    public class Elem  // holds scope information
    {
        public string type { get; set; }
        public string name { get; set; }
        public int begin { get; set; }
        public int end { get; set; }
        public int scopecount { get; set; }

        public Elem()
        {
            scopecount = 1;
        }

        public override string ToString()
        {
            StringBuilder temp = new StringBuilder();
            temp.Append("{");
            temp.Append(String.Format("{0,-10}", type)).Append(" : ");
            temp.Append(String.Format("{0,-10}", name)).Append(" : ");
            temp.Append(String.Format("{0,-5}", begin.ToString()));  // line of scope start
            temp.Append(String.Format("{0,-5}", end.ToString()));    // line of scope end
            temp.Append(string.Format("{0,-5}", scopecount.ToString()));
            temp.Append("}");
            return temp.ToString();
        }
    }

    // holds inheritance data
    public class InheritanceElem
    {
        public string parent { get; set; }
        public ArrayList children { get; set; }
        public int childcount { get; set; }

        public InheritanceElem()
        {
            childcount = 0;
            children = new ArrayList();
        }

        public override string ToString()
        {
            StringBuilder temp = new StringBuilder();
            temp.Append("{");
            temp.Append(String.Format("{0,10}", parent)).Append(" : ");
            temp.Append(String.Format("{0,10}", children)).Append(" : ");
            temp.Append(String.Format("{0,10}", childcount));
            temp.Append("}");
            return temp.ToString();
        }
    }

    // holds aggregation data
    public class AggregationElem
    {
        public string aggregator { get; set; }
        public ArrayList aggregated { get; set; }
        public string type { get; set; }

        public AggregationElem()
        {
            aggregated = new ArrayList();
        }
    }

    public class CompositionElem
    {
        public string compositor { get; set; }
        public ArrayList composedelement
        { get; set; }
        public string type { get; set; }

        public CompositionElem()
        {
            composedelement = new ArrayList();
        }
    }

    public class TypeDetails
    {
        public string usedtypename { get; set; }
        public string type { get; set; }
    }

    public class UsingElem
    {
        public string parent { get; set; }
        public List<TypeDetails> typeslist = new List<TypeDetails>();
        public string containingfunction { get; set; }

        public UsingElem()
        {

        }
    }

    public class Repository
    {
        ScopeStack<Elem> stack_ = new ScopeStack<Elem>();
        List<Elem> analyzedata_ = new List<Elem>();
        List<InheritanceElem> inheritancedata_ = new List<InheritanceElem>();
        List<AggregationElem> aggregationdata_ = new List<AggregationElem>();
        List<CompositionElem> compositiondata_ = new List<CompositionElem>();
        List<UsingElem> usingdata_ = new List<UsingElem>();
        static Repository instance;

        public Repository()
        {
            instance = this;
        }

        public static Repository getInstance()
        {
            return instance;
        }
        // provides all actions access to current semiExp

        public CSsemi.CSemiExp semi
        {
            get;
            set;
        }

        // semi gets line count from toker who counts lines
        // while reading from its source

        public int lineCount  // saved by newline rule's action
        {
            get { return semi.lineCount; }
        }
        public int prevLineCount  // not used in this demo
        {
            get;
            set;
        }
        // enables recursively tracking entry and exit from scopes

        public ScopeStack<Elem> stack  // pushed and popped by scope rule's action
        {
            get { return stack_; }
        }

        public List<Elem> analyzedata
        {
            get { return analyzedata_; }
        }

        //contains the analyzed inheritance data 
        public List<InheritanceElem> inheritancedata
        {
            get { return inheritancedata_; }
        }

        public List<AggregationElem> aggregationdata
        {
            get { return aggregationdata_; }
        }

        public List<CompositionElem> compositiondata
        {
            get { return compositiondata_; }
        }

        public List<UsingElem> usingdata
        {
            get { return usingdata_; }
        }
    }
    /////////////////////////////////////////////////////////
    // pushes scope info on stack when entering new scope

    public class PushStack : AAction
    {
        Repository repo_;

        public PushStack(Repository repo)
        {
            repo_ = repo;
        }

        public override void doAction(CSsemi.CSemiExp semi)
        {
            Elem elem = new Elem();
            elem.type = semi[0];  // expects type
            elem.name = semi[1];  // expects name
            elem.begin = repo_.semi.lineCount - 1;
            elem.end = 0;
            repo_.stack.push(elem);
            repo_.analyzedata.Add(elem);

            if (AAction.displaySemi)
            {
                Console.Write("\n  line# {0,-5}", repo_.semi.lineCount - 1);
                Console.Write("entering ");
                string indent = new string(' ', 2 * repo_.stack.count);
                Console.Write("{0}", indent);
                this.display(semi); // defined in abstract action
            }
            if (AAction.displayStack)
                repo_.stack.display();
        }

        public override void doActionInheritance(CSsemi.CSemiExp semi)
        {
            InheritanceElem inheritanceelem = new InheritanceElem();
            Elem elem = new Elem();
            bool existingParent = false;

            for (int i = 0; i < repo_.analyzedata.Count; i++)
            {
                elem = repo_.analyzedata[i];
                if (elem.type == "class" && semi[1] == elem.name)
                {
                    for (int j = 0; j < repo_.inheritancedata.Count; j++)
                    {
                        inheritanceelem = repo_.inheritancedata[j];
                        if (semi[1] == inheritanceelem.parent)
                        {
                            existingParent = true;
                            int index = repo_.inheritancedata.IndexOf(inheritanceelem);
                            inheritanceelem.children.Add(semi[0]);
                            inheritanceelem.childcount++;
                            repo_.inheritancedata.Remove(inheritanceelem);
                            repo_.inheritancedata.Insert(index, inheritanceelem);
                        }
                    }

                    if (!existingParent)
                    {
                        inheritanceelem = new InheritanceElem();
                        inheritanceelem.children.Add(semi[0]);
                        inheritanceelem.parent = semi[1];
                        inheritanceelem.childcount++;
                        //repo_.inheritancestack.push(elem);
                        repo_.inheritancedata.Add(inheritanceelem);
                    }
                }
            }
        }

        public override void doActionAggregation(CSsemi.CSemiExp semi)
        {
            AggregationElem aggregationelem = new AggregationElem();
            Elem elem = new Elem();
            bool existingAggregator = false;

            for (int i = 0; i < repo_.analyzedata.Count; i++)
            {
                elem = repo_.analyzedata[i];
                if (elem.type == "class" && semi[0] == elem.name)
                {
                    for (int j = 0; j < repo_.aggregationdata.Count; j++)
                    {
                        aggregationelem = repo_.aggregationdata[j];
                        if (semi[2] == aggregationelem.aggregator)
                        {
                            existingAggregator = true;
                            int index = repo_.aggregationdata.IndexOf(aggregationelem);
                            aggregationelem.aggregated.Add(semi[1]);
                            aggregationelem.type = semi[0];
                            repo_.aggregationdata.Remove(aggregationelem);
                            repo_.aggregationdata.Insert(index, aggregationelem);
                        }
                    }

                    if (!existingAggregator)
                    {
                        aggregationelem = new AggregationElem();
                        aggregationelem.aggregated.Add(semi[1]);
                        aggregationelem.aggregator = semi[2];
                        aggregationelem.type = semi[0];
                        repo_.aggregationdata.Add(aggregationelem);
                    }
                }
            }
        }

        public override void doActionComposition(CSsemi.CSemiExp semi)
        {
            CompositionElem compositionelem = new CompositionElem();
            Elem elem = new Elem();
            bool existingCompositor = false;

            for (int i = 0; i < repo_.analyzedata.Count; i++)
            {
                elem = repo_.analyzedata[i];
                if ((elem.type == "struct" && semi[0] == elem.name) || (elem.type == "enum" && semi[0] == elem.name))
                {
                    for (int j = 0; j < repo_.compositiondata.Count; j++)
                    {
                        compositionelem = repo_.compositiondata[j];
                        if (semi[2] == compositionelem.compositor)
                        {
                            existingCompositor = true;
                            int index = repo_.compositiondata.IndexOf(compositionelem);
                            compositionelem.composedelement.Add(semi[1]);
                            compositionelem.type = semi[0];
                            repo_.compositiondata.Remove(compositionelem);
                            repo_.compositiondata.Insert(index, compositionelem);
                        }
                    }

                    if (!existingCompositor)
                    {
                        compositionelem = new CompositionElem();
                        compositionelem.composedelement.Add(semi[1]);
                        compositionelem.compositor = semi[2];
                        compositionelem.type = semi[0];
                        repo_.compositiondata.Add(compositionelem);
                    }
                }
            }
        }

        public override void doActionUsing(CSsemi.CSemiExp semi)
        {
            UsingElem usingelem = new UsingElem();
            Elem elem = new Elem();
            bool existingfunction = false;
            string classname = semi[0];
            string functionname = semi[1];
            string type = semi[2];
            string typename = semi[3];

            for (int i = 0; i < repo_.analyzedata.Count; i++)
            {
                elem = repo_.analyzedata[i];
                if ((elem.type == "struct" || elem.type == "enum" || elem.type == "class" || elem.type == "interface")
                    && type == elem.name)
                {
                    for (int j = 0; j < repo_.usingdata.Count; j++)
                    {
                        usingelem = repo_.usingdata[j];
                        if (functionname == usingelem.containingfunction)
                        {
                            existingfunction = true;
                            TypeDetails typeDetails = new TypeDetails();
                            usingelem.parent = classname;
                            usingelem.containingfunction = functionname;
                            typeDetails.type = type;
                            typeDetails.usedtypename = typename;
                            usingelem.typeslist.Add(typeDetails);
                            int index = repo_.usingdata.IndexOf(usingelem);
                            repo_.usingdata.Remove(usingelem);
                            repo_.usingdata.Insert(index, usingelem);
                        }
                    }

                    if (!existingfunction)
                    {
                        TypeDetails typeDetails = new TypeDetails();
                        usingelem = new UsingElem();
                        usingelem.parent = classname;
                        usingelem.containingfunction = functionname;
                        typeDetails.type = type;
                        typeDetails.usedtypename = typename;
                        usingelem.typeslist.Add(typeDetails);
                        repo_.usingdata.Add(usingelem);
                    }
                }
            }
        }

    }
    /////////////////////////////////////////////////////////
    // pops scope info from stack when leaving scope

    public class PopStack : AAction
    {
        Repository repo_;

        public PopStack(Repository repo)
        {
            repo_ = repo;
        }
        public override void doAction(CSsemi.CSemiExp semi)
        {
            Elem elem;
            try
            {
                elem = repo_.stack.pop();
                for (int i = 0; i < repo_.analyzedata.Count; ++i)
                {
                    Elem temp = repo_.analyzedata[i];
                    if (elem.type == temp.type)
                    {
                        if (elem.name == temp.name)
                        {
                            if ((repo_.analyzedata[i]).end == 0)
                            {
                                (repo_.analyzedata[i]).end = repo_.semi.lineCount;
                                break;
                            }
                        }
                    }
                    if (temp.end == 0 && temp.type == "function")
                        temp.scopecount++;
                }
            }
            catch
            {
                Console.Write("popped empty stack on semiExp: ");
                semi.display();
                return;
            }
            CSsemi.CSemiExp local = new CSsemi.CSemiExp();
            local.Add(elem.type).Add(elem.name);

            if (AAction.displaySemi)
            {
                Console.Write("\n  line# {0,-5}", repo_.semi.lineCount);
                Console.Write("leaving  ");
                string indent = new string(' ', 2 * (repo_.stack.count + 1));
                Console.Write("{0}", indent);
                this.display(local); // defined in abstract action
            }
        }

        public override void doActionInheritance(CSsemi.CSemiExp semi)
        {
            throw new NotImplementedException();
        }

        public override void doActionAggregation(CSsemi.CSemiExp semi)
        {
            throw new NotImplementedException();
        }

        public override void doActionComposition(CSsemi.CSemiExp semi)
        {
            throw new NotImplementedException();
        }

        public override void doActionUsing(CSsemi.CSemiExp semi)
        {
            throw new NotImplementedException();
        }
    }
    ///////////////////////////////////////////////////////////
    // action to print function signatures - not used in demo

    public class PrintFunction : AAction
    {
        Repository repo_;

        public PrintFunction(Repository repo)
        {
            repo_ = repo;
        }
        public override void display(CSsemi.CSemiExp semi)
        {
            Console.Write("\n    line# {0}", repo_.semi.lineCount - 1);
            Console.Write("\n    ");
            for (int i = 0; i < semi.count; ++i)
                if (semi[i] != "\n" && !semi.isComment(semi[i]))
                    Console.Write("{0} ", semi[i]);
        }
        public override void doAction(CSsemi.CSemiExp semi)
        {
            this.display(semi);
        }

        public override void doActionInheritance(CSsemi.CSemiExp semi)
        {
            this.display(semi);
        }

        public override void doActionAggregation(CSsemi.CSemiExp semi)
        {
            this.display(semi);
        }

        public override void doActionComposition(CSsemi.CSemiExp semi)
        {
            this.display(semi);
        }

        public override void doActionUsing(CSsemi.CSemiExp semi)
        {
            this.display(semi);
        }
    }
    /////////////////////////////////////////////////////////
    // concrete printing action, useful for debugging

    public class Print : AAction
    {
        Repository repo_;

        public Print(Repository repo)
        {
            repo_ = repo;
        }
        public override void doAction(CSsemi.CSemiExp semi)
        {
            Console.Write("\n  line# {0}", repo_.semi.lineCount - 1);
            this.display(semi);
        }

        public override void doActionInheritance(CSsemi.CSemiExp semi)
        {
            Console.Write("\n  line# {0}", repo_.semi.lineCount - 1);
            this.display(semi);
        }

        public override void doActionAggregation(CSsemi.CSemiExp semi)
        {
            Console.Write("\n  line# {0}", repo_.semi.lineCount - 1);
            this.display(semi);
        }

        public override void doActionComposition(CSsemi.CSemiExp semi)
        {
            Console.Write("\n  line# {0}", repo_.semi.lineCount - 1);
            this.display(semi);
        }

        public override void doActionUsing(CSsemi.CSemiExp semi)
        {
            Console.Write("\n  line# {0}", repo_.semi.lineCount - 1);
            this.display(semi);
        }
    }
    /////////////////////////////////////////////////////////
    // rule to detect namespace declarations

    public class DetectNamespace : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int index = semi.Contains("namespace");
            if (index != -1)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // create local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add(semi[index]).Add(semi[index + 1]);
                doActions(local);
                return true;
            }
            return false;
        }
    }
    /////////////////////////////////////////////////////////
    // rule to dectect class definitions

    public class DetectClass : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int indexCL = semi.Contains("class");
            int indexIF = semi.Contains("interface");
            int indexST = semi.Contains("struct");
            int indexEN = semi.Contains("enum");

            int index = Math.Max(indexCL, indexIF);
            index = Math.Max(index, indexST);
            index = Math.Max(index, indexEN);
            if (index != -1)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add(semi[index]).Add(semi[index + 1]);
                doActions(local);

                return true;
            }
            return false;
        }
    }
    /////////////////////////////////////////////////////////
    // rule to dectect class inheritance

    public class DetectInheritance : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int indexCL = semi.Contains("class");
            int indexIF = semi.Contains("interface");

            int index = Math.Max(indexCL, indexIF);
            int index2 = semi.FindFirst(":");
            if (index != -1 && index2 != -1)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add(semi[index + 1]).Add(semi[index + 3]);
                doActionsInheritance(local);

                int multiInheritCount = semi.FindAll(",");
                while (multiInheritCount > 0)
                {
                    index2 = index2 + 2;
                    local = new CSsemi.CSemiExp();
                    local.displayNewLines = false;
                    local.Add(semi[index + 1]).Add(semi[index2 + 1]);
                    doActionsInheritance(local);
                    multiInheritCount--;
                }

                return true;
            }
            return false;
        }
    }
    /////////////////////////////////////////////////////////
    // rule to dectect class aggregation

    public class DetectAggregation : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            Repository repo_ = Repository.getInstance();
            int index = semi.FindFirst("new");
            if (index != -1)
            {
                string currclassName = DetectParentClass.getClassName(repo_.semi.lineCount);
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add(semi[index + 1]).Add(semi[index - 2]).Add(currclassName);
                doActionsAggregation(local);
                return true;
            }

            return false;
        }
    }

    public class DetectComposition : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            List<string> variablecountlist = null;
            Repository repo_ = Repository.getInstance();

            if (semi.count >= 2)
            {
                variablecountlist = semi.FindVariablesList();
            }

            if (variablecountlist != null && variablecountlist.Count == 2)
            {
                string currclassName = DetectParentClass.getClassName(repo_.semi.lineCount);
                //if (currclassName != "")
                {
                    CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                    local.displayNewLines = false;
                    local.Add(variablecountlist[0]).Add(variablecountlist[1]).Add(currclassName);
                    doActionsComposition(local);
                    return true;
                }
            }

            return false;
        }
    }

    public class DetectUsing : ARule
    {
        public static bool isSpecialToken(string token)
        {
            string[] SpecialToken = { "if", "else", "for", "foreach", "while", "catch", "try", "finally", "using", "switch", "case", "do" };
            foreach (string stoken in SpecialToken)
                if (stoken == token)
                    return true;
            return false;
        }

        public override bool test(CSsemi.CSemiExp semi)
        {
            Repository repo_ = Repository.getInstance();
            int index = semi.Contains("(");
            int endindex = semi.Contains(")");
            int loopindex = semi.FindAll(",");
            int currindex = 0;
            bool flag = false;

            if (index != -1 && !isSpecialToken(semi[index - 1]))
            {
                string currclassName = DetectParentClass.getClassName(repo_.semi.lineCount);
                currindex = index;
                string functionname = semi[currindex - 1];
                for (int i = 0; i <= loopindex; i++)
                {
                    CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                    local.displayNewLines = false;
                    local.Add(currclassName).Add(functionname).Add(semi[currindex + 1]).Add(semi[currindex + 2]);
                    doActionsUsing(local);
                    flag = true;
                    currindex = index + 3;
                }
            }
            if (flag)
                return true;

            return false;
        }
    }
    /////////////////////////////////////////////////////////
    // rule to dectect function definitions

    public class DetectFunction : ARule
    {
        public static bool isSpecialToken(string token)
        {
            string[] SpecialToken = { "if", "else", "for", "foreach", "while", "catch", "try", "finally", "using", "switch", "case", "do" };
            foreach (string stoken in SpecialToken)
                if (stoken == token)
                    return true;
            return false;
        }
        public override bool test(CSsemi.CSemiExp semi)
        {
            if (semi[semi.count - 1] != "{")
                return false;

            int index = semi.FindFirst("(");
            if (index > 0 && !isSpecialToken(semi[index - 1]))
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                local.Add("function").Add(semi[index - 1]);
                doActions(local);
                return true;
            }
            return false;
        }
    }
    ////////////////////////////////////
    // rule to dectect scope with braces

    public class DetectScope : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            string[] SpecialToken = { "if", "else", "for", "foreach", "while", "catch", "try", "finally", "switch", "do" };
            int index = -1;
            foreach (string stoken in SpecialToken)
            {
                int tempindex = semi.Contains(stoken);
                if (tempindex != -1)
                    index = Math.Max(index, tempindex);

            }
            int index2 = semi.FindFirst("{");

            if (index != -1 && index2 != -1)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add("Scope").Add(semi[index]);
                doActions(local);
                return true;
            }

            return false;
        }
    }
    ////////////////////////////////////
    // rule to dectect scope without braces

    public class DetectScopeWithoutBraces : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            string[] SpecialToken = { "if", "else", "for", "foreach", "while", "switch", "do", "break" };
            int index = -1;
            int index2 = 0;
            CSsemi.CSemiExp local = new CSsemi.CSemiExp();
            bool flag = false;
            foreach (string stoken in SpecialToken)
            {
                index = semi.Contains(stoken);
                index2 = semi.FindFirst("{");
                if (index != -1 && index2 == -1)
                {
                    local = new CSsemi.CSemiExp();
                    // local semiExp with tokens for type and name
                    local.displayNewLines = false;
                    local.Add("Scope").Add(semi[index]);
                    doActions(local);
                    flag = false;
                }
            }
            if (flag)
                return true;

            return false;
        }
    }
    /////////////////////////////////////////////////////////
    // detect entering anonymous scope
    // - expects namespace, class, and function scopes
    //   already handled, so put this rule after those
    public class DetectAnonymousScope : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int index = semi.Contains("{");
            if (index != -1)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // create local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add("control").Add("anonymous");
                doActions(local);
                return true;
            }
            return false;
        }
    }
    /////////////////////////////////////////////////////////
    // detect leaving scope

    public class DetectLeavingScopeWithoutBraces : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            string[] SpecialToken = { "if", "else", "for", "foreach", "while", "switch", "case", "do", "break" };
            int index = -1;
            int index2 = 0;
            CSsemi.CSemiExp local = new CSsemi.CSemiExp();
            bool flag = false;
            foreach (string stoken in SpecialToken)
            {
                index = semi.Contains(stoken);
                index2 = semi.FindFirst("{");
                if (index != -1 && index2 == -1)
                {
                    local = new CSsemi.CSemiExp();
                    // local semiExp with tokens for type and name
                    local.displayNewLines = false;
                    local.Add("Scope").Add(semi[index]);
                    doActions(local);
                    flag = false;
                }
            }
            if (flag)
                return true;

            return false;

        }
    }
    /////////////////////////////////////////////////////////
    // detect leaving scope

    public class DetectLeavingScope : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int index = semi.Contains("}");
            if (index != -1)
            {
                doActions(semi);
                return true;
            }
            return false;
        }
    }

    public class BuildCodeAnalyzer
    {
        static Repository repo = new Repository();

        public BuildCodeAnalyzer(CSsemi.CSemiExp semi)
        {
            repo.semi = semi;
        }
        public virtual Parser build()
        {
            Parser parser = new Parser();

            // decide what to show
            AAction.displaySemi = true;
            AAction.displayStack = false;  // this is default so redundant

            // action used for namespaces, classes, and functions
            PushStack push = new PushStack(repo);

            // capture namespace info
            DetectNamespace detectNS = new DetectNamespace();
            detectNS.add(push);
            parser.add(detectNS);

            // capture class info
            DetectClass detectCl = new DetectClass();
            detectCl.add(push);
            parser.add(detectCl);

            // capture function info
            DetectFunction detectFN = new DetectFunction();
            detectFN.add(push);
            parser.add(detectFN);

            // handle these scopes -  "if", "for", "foreach", "while", "catch", "try", "switch" among others
            DetectScope detectScop = new DetectScope();
            detectScop.add(push);
            parser.add(detectScop);

            // handle these scopes without braces-  "if", "for", "foreach", "while", "catch", "try", "switch" among others
            DetectScopeWithoutBraces detectScopWB = new DetectScopeWithoutBraces();
            detectScopWB.add(push);
            parser.add(detectScopWB);

            // handle entering anonymous scopes, e.g., if, while, etc.
            DetectAnonymousScope anon = new DetectAnonymousScope();
            anon.add(push);
            parser.add(anon);

            // handle leaving scopes
            DetectLeavingScopeWithoutBraces leaveWB = new DetectLeavingScopeWithoutBraces();
            PopStack popWB = new PopStack(repo);
            leaveWB.add(popWB);
            parser.add(leaveWB);

            // handle leaving scopes
            DetectLeavingScope leave = new DetectLeavingScope();
            PopStack pop = new PopStack(repo);
            leave.add(pop);
            parser.add(leave);

            // parser configured
            return parser;
        }
    }

    public class BuildCodeAnalyzerRelationships
    {
        Repository repo = Repository.getInstance();

        public BuildCodeAnalyzerRelationships(CSsemi.CSemiExp semi)
        {
            repo.semi = semi;
        }
        public virtual Parser build()
        {
            Parser parser = new Parser();

            // decide what to show
            AAction.displaySemi = true;
            AAction.displayStack = false;  // this is default so redundant

            // action used for namespaces, classes, and functions
            PushStack push = new PushStack(repo);

            // capture inheritance info
            DetectInheritance detectIn = new DetectInheritance();
            detectIn.add(push);
            parser.add(detectIn);

            // capture aggregated info
            DetectAggregation detectAg = new DetectAggregation();
            detectAg.add(push);
            parser.add(detectAg);

            DetectComposition detectCs = new DetectComposition();
            detectCs.add(push);
            parser.add(detectCs);

            DetectUsing detectUs = new DetectUsing();
            detectUs.add(push);
            parser.add(detectUs);

            // parser configured
            return parser;
        }
    }

    public static class DetectParentClass
    {
        static public string getClassName(int lineno)
        {
            string classname = "";
            Repository repo_ = Repository.getInstance();
            Elem elem = new Elem();
            int begin = 0;

            for (int i = 0; i < repo_.analyzedata.Count; i++)
            {
                elem = repo_.analyzedata[i];
                if (elem.type == "class")
                {
                    if (elem.begin > begin && elem.begin < lineno && elem.end > lineno)
                    {
                        begin = elem.begin;
                        classname = elem.name;
                    }
                }
            }

            return classname;
        }
    }
}

