///////////////////////////////////////////////////////////////////////
// Author:         Pramukh Shenoy                                    //
///////////////////////////////////////////////////////////////////////
/*
 * Get the list of all files, ignoring the files with the keyword Temporary and AssemblyInfo.cs since it does not contain any/readable data
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace CodeAnalyzer
{
    public class FileMngr
    {
        List<string> files;

        public List<string> Files
        {
            get { return files; }
            set { files = value; }
        }
        private bool recurse;

        public bool Recurse
        {
            get { return recurse; }
            set { recurse = value; }
        }
        private List<string> patterns;

        public List<string> Patterns
        {
            get { return patterns; }
            set { patterns = value; }
        }

        public FileMngr()
        {
            files = new List<string>();
            patterns = new List<string>();
        }

        public List<string> getFiles()
        {
            return files;
        }


        /// <summary>
        /// get the list of files for each path provided
        /// </summary>
        /// <param name="path">the path for which the file name has to be provided</param>
        public void findFiles(string path)
        {
            try
            {
                foreach (string pattern in patterns)
                {
                    string[] newFiles = Directory.GetFiles(path, pattern);
                    ArrayList filesAL = new ArrayList();
                    for (int i = 0; i < newFiles.Length; ++i)
                    {
                        if (!newFiles[i].Contains("Temporary") && !newFiles[i].Contains("AssemblyInfo.cs"))
                            filesAL.Add(Path.GetFullPath(newFiles[i]));
                    }
                    files.AddRange((String[])filesAL.ToArray(typeof(string)));
                }
                if (recurse)
                {
                    string[] dirs = Directory.GetDirectories(path);
                    foreach (string dir in dirs)
                        findFiles(dir);
                }
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("\nThe provided directory cannot be found {0}\n", path);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("\nThe provided file cannot be found {0}\n", path);
            }
            catch (FileLoadException)
            {
                Console.WriteLine("\nError Loading file {0}\n", path);
            }
            catch (PathTooLongException)
            {
                Console.WriteLine("\nThe provided path is too long to read. Try moving the file to another location {0}\n", path);
            }
            catch (Exception)
            {
                Console.WriteLine("\nError in {0}\n", path);
            }
        }
    }

#if(TEST_FILEMGR)
    class TestFileMgr
    {
        static void Main(string[] args)
        {
            Console.Write("\n  Testing File Manager Class");
            Console.Write("\n =======================\n");

            FileMngr fm = new FileMngr();
            fm.Patterns.Add("*.cs");
            fm.findFiles(".");
            List<string> files = fm.getFiles();
            foreach (string file in files)
                Console.Write("\n  {0}", file);
            Console.Write("\n\n");
            Console.ReadLine();
        }
#endif
    }
}
