using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CLOPE_Anurov
{
    static class DataLoader
    {
        public static List<List<string>> Load(string defaultPath=null, string splitString=",", string removeDataValue = "?")
        {
            string path = defaultPath ?? Path.Combine(Environment.CurrentDirectory, @"data\agaricus-lepiota.data");

            string line = null;
            List<List<string>> data = new List<List<string>>();

            try
            {
                StreamReader sr = new StreamReader(path);
                line = sr.ReadLine();

                while (line != null)
                {
                    data.Add(new List<string>(line.Split(splitString, StringSplitOptions.RemoveEmptyEntries)));
                    for (int i = 0; i < data[^1].Count; i++)
                    {
                        data[^1][i] += i.ToString();
                    }
                    if (removeDataValue != null)
                    {
                        data[^1] = data[^1].Where(
                            s => !string.IsNullOrWhiteSpace(s) && !s.Contains(removeDataValue)).ToList();
                    }

                    line = sr.ReadLine();   
                }
                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            return data;
        }
    }
}
