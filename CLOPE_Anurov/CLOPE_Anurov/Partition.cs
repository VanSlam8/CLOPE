using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLOPE_Anurov
{
    class Partition
    {  
        public List<Cluster> clusters = new List<Cluster>();

        public readonly float repulsion = 2.6f;

        // Since class Cluster is mutable it only points to the cluster that n-th transaction belongs to, no duplicate objects here.
        private readonly Dictionary<int, Cluster> transactionToClusterMap = new Dictionary<int, Cluster>();
        private int datasetHashCode = 0;     

        public Partition(float repulsion)
        {
            this.repulsion = repulsion;
        }

        public Partition()
        {
        }

        /// <summary>
        /// Calculates gain of Profit as a result of Adding transaction to the cluster
        /// </summary>
        /// <param name="cluster"></param>
        /// <param name="transaction"></param>
        /// <param name="repulsion">repulsion value: repulsion > 1></param>
        /// <returns></returns>
        public  float DeltaAdd(Cluster cluster, List<string> transaction, float repulsion)
        {
            int newArea = cluster.area + transaction.Count;
            int newWidth = cluster.width;
            for (int i = 0; i < transaction.Count; i++)
            {
                if (!cluster.uniqueObjects.ContainsKey(transaction[i]))
                {
                    newWidth++;
                }
            }

            float newProfit = newArea * (cluster.numberOfTransactions + 1) / MathF.Pow(newWidth, repulsion);
            float previousProfit = cluster.area * cluster.numberOfTransactions / MathF.Pow(cluster.width, repulsion);
            return newProfit - previousProfit;
        }

        /// <summary>
        /// Initialization Phase
        /// </summary>
        /// <param name="dataset">List of transactions</param>
        /// <param name="iterate">Go to Iteration Phase right after Initialization is done?</param>
        /// <param name="iterations">Number of iterations, for unlimited: -1</param>
        /// <param name="log">Additinal console output</param>
        public void Initialize(List<List<string>> dataset, bool iterate = false, int iterations = 1, bool log = true)
        {
            
            if (log) Console.WriteLine($"Initialization has begun. repulsion: {repulsion}");

            datasetHashCode = dataset.GetHashCode();
            for (int t = 0; t < dataset.Count; t++) // t stands for transaction
            {           
                float maxDelta = dataset[t].Count / MathF.Pow(dataset[t].Distinct().Count(), repulsion);
                int maxDeltaIndex = -1;

                for (int i = 0; i < clusters.Count; i++)
                {
                    float delta = DeltaAdd(clusters[i], dataset[t], repulsion);
                    if (delta >= maxDelta)
                    {
                        maxDelta = delta;
                        maxDeltaIndex = i;
                    }
                }

                if (maxDeltaIndex == -1)
                {                    
                    clusters.Add(new Cluster());
                    clusters[^1].AddTransaction(dataset[t]);
                    transactionToClusterMap.Add(t, clusters[^1]);
                }
                else
                {
                    clusters[maxDeltaIndex].AddTransaction(dataset[t]);
                    transactionToClusterMap.Add(t, clusters[maxDeltaIndex]);
                }
            }

            if (log)
            {
                Console.WriteLine($"Initialization has been completed.");
                Console.WriteLine($"Number of clusters: {clusters.Count}");
            }

            if (iterate)
            {
                Iterate(dataset, log:log);
            }
        }

        /// <summary>
        /// Iteration Phase
        /// </summary>
        /// <param name="dataset">List of transactions</param>
		/// <param name="limitIterations">Limit the number of iterations?</param>
        /// <param name="iterations">Number of iterations</param>
        /// <param name="log">Additinal console output</param>
        public void Iterate(List<List<string>> dataset, bool limitIterations = true, int iterations = 1, bool log = true)
        {
            if (log) Console.WriteLine($"Iteration has begun. repulsion: {repulsion}");

            if (clusters.Count == 0)
            {
                Console.WriteLine("WARNING! Iteration was called before Initialization. Starting Initialization Phase on the given dataset. . .");
                Initialize(dataset, false, log:log);
            }
            else if (dataset.Count != transactionToClusterMap.Count || datasetHashCode != dataset.GetHashCode())
            {
                Console.WriteLine("ERROR! Attempted to Iterate on a different dataset compared to Initialization");
                return;
            }            

            bool moved = false;
            do
            {
                moved = false;
                if (limitIterations)
                {
                    iterations--;
                }
                

                for (int t = 0; t < dataset.Count; t++)
                {
                    Cluster prevCluster = transactionToClusterMap[t];
                    int maxDeltaIndex = -1;
                    float maxDelta = dataset[t].Count / MathF.Pow(dataset[t].Distinct().Count(), repulsion);

                    transactionToClusterMap[t].RemoveTransaction(dataset[t]);

                    for (int clusterIndex = 0; clusterIndex < clusters.Count; clusterIndex++)
                    {
                        float delta = DeltaAdd(clusters[clusterIndex], dataset[t], repulsion);                        
                        
                        if (delta >= maxDelta)
                        {
                            maxDelta = delta;
                            maxDeltaIndex = clusterIndex;
                        }
                    }

                    if (clusters[maxDeltaIndex] != prevCluster)
                    {
                        moved = true;
                    }

                    if (maxDeltaIndex == -1)
                    {
                        clusters.Add(new Cluster());
                        clusters[^1].AddTransaction(dataset[t]);
                        transactionToClusterMap[t] = clusters[^1];
                    }
                    else
                    {
                        clusters[maxDeltaIndex].AddTransaction(dataset[t]);
                        transactionToClusterMap[t] = clusters[maxDeltaIndex];
                    }
                    
                }

                if (log) Console.WriteLine("    Cycle complete.");
            } while (moved && (!limitIterations || iterations > 0));

            clusters = clusters.Where(c => c.numberOfTransactions > 0).ToList(); // Remove all empty clusters

            if (log) Console.WriteLine($"Iteration has been completed.\n");
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            StringBuilder clusterBlock = new StringBuilder();
            result.AppendLine($"Number of clusters: {clusters.Count}");
            int max = 0;
            int min = int.MaxValue;
            int sum = 0;
            for (int i = 0; i < clusters.Count; i++)
            {
                bool containsEdible = clusters[i].uniqueObjects.ContainsKey("e0");
                bool containsPoisonous = clusters[i].uniqueObjects.ContainsKey("p0");
                int edibleCount = containsEdible ? clusters[i].uniqueObjects["e0"] : 0;
                int poisonousCount = containsPoisonous ? clusters[i].uniqueObjects["p0"] : 0;

                clusterBlock.AppendLine($"Cluster #{i + 1}.\n    Edible: {edibleCount}, Poisonous: {poisonousCount}");

                if (clusters[i].numberOfTransactions > max) max = clusters[i].numberOfTransactions;
                if (clusters[i].numberOfTransactions < min) min = clusters[i].numberOfTransactions;
                sum += clusters[i].numberOfTransactions;
            }
            result.AppendLine($"Minimun number of transactions: {min}");
            result.AppendLine($"Maximum number of transactions: {max}");
            result.AppendLine($"Total number of transactions: {sum}");
            result.Append(clusterBlock);
            return result.ToString();
        }

    }
}
