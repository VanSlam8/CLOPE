using System;
using System.Collections.Generic;
using System.Linq;

namespace CLOPE_Anurov
{    
    class Cluster
    {        
        public readonly Dictionary<string, int> uniqueObjects = new Dictionary<string, int>(); // Object : Cluster.Occurence(Object)
        public int numberOfTransactions = 0;
        public int area = 0;
        public int width = 0;

        public void AddTransaction(List<string> transaction)
        {
            numberOfTransactions++;
            area += transaction.Count;

            for (int i = 0; i < transaction.Count; i++)
            {
                if (!uniqueObjects.ContainsKey(transaction[i]))
                {
                    uniqueObjects.Add(transaction[i], 1);
                    width++;    
                }
                else
                {
                    uniqueObjects[transaction[i]]++;
                }
            }

        }

        public void RemoveTransaction(List<string> transaction)
        {
            numberOfTransactions--;
            area -= transaction.Count;

            for (int i = 0; i < transaction.Count; i++)
            {
                if (uniqueObjects.ContainsKey(transaction[i])) // Just a protection from misuse
                {
                    uniqueObjects[transaction[i]]--;
                    if (uniqueObjects[transaction[i]] < 1)
                    {
                        uniqueObjects.Remove(transaction[i]);
                        width--;
                    }                    
                }
                else
                {
                    throw new Exception("Attempted to remove a non-existent transaction from cluster");
                }
            }

        }
    }
    
   
}
