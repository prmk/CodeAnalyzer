using System;
using System.Collections;
/*
 * Get the list of all files, ignoring the files with the keyword Temporary and AssemblyInfo.cs since it does not contain any/readable data
 * Author:      Pramukh Shenoy
 */
using System.Collections.Generic;
using System.IO;

namespace FileManager
{
    public class FileMngr
    {
        public List<string> files { get; set; }
        public List<string> patterns;
        public List<string> paths;
        public bool recurse;

        public FileMngr()
        {
            patterns = new List<string>();
            files = new List<string>();
            paths = new List<string>();
            recurse = true;
        }

        public void findFiles(string path)
        {
            if (patterns.Count == 0)
                addPattern("*.*");
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

        public void addPattern(string pattern)
        {
            patterns.Add(pattern);
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
            fm.addPattern("*.cs");
            fm.findFiles(".");
            List<string> files = fm.files;
            foreach (string file in files)
                Console.Write("\n  {0}", file);
            Console.Write("\n\n");
            Console.ReadLine();
        }
#endif
    }
}
