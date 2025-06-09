using ConsoleTables;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyramid_Method
{
    internal class CyclesSet3
    {
        public static bool ResultsEqual(Dictionary<(int, int), int> dict1, Dictionary<(int, int), int> dict2, int n, int m)
        {
            bool areEqual = true;
            List<(int, int)> finalIterations = new List<(int, int)>();
            for (int i = 1; i <= m; i++)
            {
                finalIterations.Add((i, n));
            }
            foreach (var key in finalIterations)
            {
                if (!dict1.TryGetValue(key, out int value1) || !dict2.TryGetValue(key, out int value2) || value1 != value2)
                {
                    areEqual = false;
                    break;
                }
            }
            return areEqual;
        }

        public static Dictionary<(int, int), int> SequentialCalculation(
            Dictionary<(int, int), int> values,
            Stopwatch sw,
            int n,
            int m)
        {
            Random random = new Random();
            Dictionary<(int, int), int> deepCopyDictionary = new Dictionary<(int, int), int>(values);
            for (int j = 1; j <= n; j++)
            {
                for (int i = 1; i <= m; i++)
                {
                    try
                    {
                        sw.Stop();
                        int a = deepCopyDictionary[(i, j - 1)];
                        sw.Start();
                    }
                    catch
                    {
                        deepCopyDictionary[(i, j - 1)] = random.Next(1, 100);
                        values[(i, j - 1)] = deepCopyDictionary[(i, j - 1)];
                        sw.Start();
                    }
                    try
                    {
                        sw.Stop();
                        int a = deepCopyDictionary[(i + 1, j - 1)];
                        sw.Start();
                    }
                    catch
                    {
                        deepCopyDictionary[(i + 1, j - 1)] = random.Next(1, 100);
                        values[(i + 1, j - 1)] = deepCopyDictionary[(i + 1, j - 1)];
                        sw.Start();
                    }
                    try
                    {
                        sw.Stop();
                        int a = deepCopyDictionary[(i + 2, j - 1)];
                        sw.Start();
                    }
                    catch
                    {
                        deepCopyDictionary[(i + 2, j - 1)] = random.Next(1, 100);
                        values[(i + 2, j - 1)] = deepCopyDictionary[(i + 2, j - 1)];
                        sw.Start();
                    }
                    deepCopyDictionary[(i, j)] = (deepCopyDictionary[(i, j - 1)])
                        + (deepCopyDictionary[(i + 1, j - 1)]) + (deepCopyDictionary[(i + 2, j - 1)]);
                }
            }
            return deepCopyDictionary;
        }

        public static Dictionary<(int, int), int> PyramidMethod(
            Dictionary<(int, int), int> values,
            int n,
            int m,
            int coresAmount,
            int threadsAmount,
            Stopwatch sw)
        {
            Dictionary<(int, int), int> deepCopyDictionary = new Dictionary<(int, int), int>(values);
            object lockObject = new object();

            Thread[] threads = new Thread[threadsAmount];

            int baseIterations = m / threadsAmount;
            int remainder = m % threadsAmount;

            sw.Stop();

            int currentStartK = 1;

            for (int t = 0; t < threadsAmount; t++)
            {
                int iterationsForThisThread = baseIterations + (t < remainder ? 1 : 0);
                int startK = currentStartK;
                int endK = currentStartK + iterationsForThisThread - 1;
                currentStartK = endK + 1;

                int coreId = t % coresAmount;

                threads[t] = new Thread(() =>
                {
                    Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(1 << coreId);

                    for (int k = startK; k <= endK; k++)
                    {
                        for (int j = 1; j <= n; j++)
                        {
                            for (int i = k; i <= Math.Min(m, (int)Math.Floor((double)(-2 * j + k + n * 2))); i++)
                            {
                                sw.Stop();
                                if (deepCopyDictionary.ContainsKey((i, j)))
                                {
                                    continue;
                                }
                                else
                                {
                                    sw.Start();
                                    lock (lockObject)
                                    {
                                        deepCopyDictionary[(i, j)] =
                                            (deepCopyDictionary[(i, j - 1)]) +
                                            (deepCopyDictionary[(i + 1, j - 1)]) +
                                            (deepCopyDictionary[(i + 2, j - 1)]);
                                    }
                                }
                            }
                        }
                    }
                });
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }

            sw.Start();

            foreach (var thread in threads)
            {
                thread.Join();
            }

            return deepCopyDictionary;
        }

        public static Dictionary<(int, int), int> ModifiedPyramidMethod(
            Dictionary<(int, int), int> values,
            int n,
            int m,
            int threadsAmount,
            int coresAmount,
            Stopwatch sw)
        {
            Dictionary<(int, int), int> deepCopyDictionary = new Dictionary<(int, int), int>(values);
            object lockObject = new object();
            Barrier barrier = new Barrier(threadsAmount);

            sw.Stop();

            Thread[] threads = new Thread[threadsAmount];
            int b = m / 2;
            int currentStartK = 1;

            for (int t = 0; t < threadsAmount; t++)
            {
                int iterationsForThread = m / threadsAmount + (t < m % threadsAmount ? 1 : 0);
                int startK = currentStartK;
                int endK = currentStartK + iterationsForThread - 1;
                currentStartK = endK + 1;
                int coreId = t % coresAmount;

                threads[t] = new Thread(() =>
                {
                    Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(1 << coreId);
                    for (int j = 1; j <= n; j++)
                    {
                        for (int k = startK; k <= endK; k++)
                        {
                            for (int i = k; i <= Math.Min(Math.Min(m, (int)Math.Floor((double)(-2 * j + k + n * 2))), k + 1); i++)
                            {
                                sw.Stop();
                                if (deepCopyDictionary.ContainsKey((i, j)))
                                {
                                    continue;
                                }
                                else
                                {
                                    sw.Start();
                                    lock (lockObject)
                                    {
                                        deepCopyDictionary[(i, j)] =
                                            (deepCopyDictionary[(i, j - 1)]) +
                                            (deepCopyDictionary[(i + 1, j - 1)]) +
                                            (deepCopyDictionary[(i + 2, j - 1)]);
                                    }
                                }
                            }
                        }
                        sw.Stop();
                        barrier.SignalAndWait();
                        sw.Start();
                    }
                });
            }

            foreach (var thread in threads) thread.Start();
            sw.Start();

            foreach (var thread in threads) thread.Join();

            return deepCopyDictionary;
        }
    }
}
