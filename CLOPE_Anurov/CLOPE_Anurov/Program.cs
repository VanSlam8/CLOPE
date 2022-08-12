using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CLOPE_Anurov
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Loginom!");
            List<List<string>> data = DataLoader.Load();

            Console.WriteLine($"Number Of Transactions: {data.Count}");
            //PrintNestedLists(new List<List<string>>(data.ToArray()[0..5])); // Print slice of data
            //PrintNestedLists(new List<List<string>>(data.ToArray()[^5..^0])); // Print slice of data
            //PrintNestedLists(data); // Print all data
            
            Partition partition = new Partition(repulsion:2.6f);
            Stopwatch stopwatch = Stopwatch.StartNew();
            partition.Initialize(data, log:false); // -----------------   
            stopwatch.Stop();
            Console.WriteLine($"Initialization execution time: {stopwatch.ElapsedMilliseconds} ms\n");

            partition.Iterate(data, limitIterations:true, iterations:2, log: true);

            //Console.WriteLine(partition); // Print partition


            Console.WriteLine("\n\nPress any key to exit. . .");
            Console.ReadKey();
        }

        /// <summary>
        /// Writes to System.Console all elements in each of nested Lists.
        /// Bottom Type has to be typeof(string)
        /// Basically equal to regular Console.WriteLine in terms of perfomance
        /// </summary>
        /// <param name="nestedList"></param>
        static void PrintNestedLists(IEnumerable<dynamic> nestedList)
        {
            // Base case
            if (nestedList.GetType() == typeof(List<string>))
            {
                Console.WriteLine(string.Join(",", nestedList));
            }
            else
            {
                // Foreach inner Enumerable: call itself
                foreach (var list in nestedList)
                {
                    PrintNestedLists(list);
                }
            }
        }

    }
}
