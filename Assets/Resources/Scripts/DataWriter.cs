using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

public class DataWriter
{
    string run_id;
    string filename;
    public DataWriter(string run_id)
    {
        this.run_id = run_id;
        this.filename = $"Testing/{run_id}/Testing.csv";
        
        Directory.CreateDirectory($"Testing/{run_id}");

        // Initalize header
        using (StreamWriter sw = new StreamWriter(filename))
        {
            sw.WriteLine("Collision_count,Episode_length");
        }
    }

    public void write(int collisions, int epLength)
    {
        // Write each directory name to a file.
        using (StreamWriter sw = File.AppendText(filename))
        {
                sw.WriteLine($"{collisions.ToString()},{epLength.ToString()}");
        }
    }

    public void writeTest()
    {
        // Get the directories currently on the C drive.
        DirectoryInfo[] cDirs = new DirectoryInfo(@"c:\").GetDirectories();

        // Write each directory name to a file.
        using (StreamWriter sw = new StreamWriter("CDriveDirs.txt"))
        {
            foreach (DirectoryInfo dir in cDirs)
            {
                sw.WriteLine(dir.Name);
            }
        }

        // Read and show each line from the file.
        string line = "";
        using (StreamReader sr = new StreamReader("CDriveDirs.txt"))
        {
            while ((line = sr.ReadLine()) != null)
            {
                Console.WriteLine(line);
            }
        }
    }
}
