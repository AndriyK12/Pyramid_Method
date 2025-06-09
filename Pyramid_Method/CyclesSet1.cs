using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyramid_Method
{
    internal class CyclesSet1
    {
        public static bool ResultsEqual(Dictionary<(int, int), int> dict1, Dictionary<(int, int), int> dict2, int n, int m)
        {
            bool areEqual = true;
            List<(int, int)> finalIterations = new List<(int, int)>();
            for (int j = 1; j <= n; j++)
            {
                finalIterations.Add((m, j));
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
            for (int i = 1; i <= m; i++)
            {
                for (int j = 1; j <= n; j++)
                {
                    try
                    {
                        sw.Stop();
                        int a = deepCopyDictionary[(i - 1, j)];
                        sw.Start();
                    }
                    catch
                    {
                        deepCopyDictionary[(i - 1, j)] = random.Next(1, 100);
                        values[(i - 1, j)] = deepCopyDictionary[(i - 1, j)];
                        sw.Start();
                    }
                    try
                    {
                        sw.Stop();
                        int a = deepCopyDictionary[(i - 1, j - 3)];
                        sw.Start();
                    }
                    catch
                    {
                        deepCopyDictionary[(i - 1, j - 3)] = random.Next(1, 100);
                        values[(i - 1, j - 3)] = deepCopyDictionary[(i - 1, j - 3)];
                        sw.Start();
                    }
                    try
                    {
                        sw.Stop();
                        int a = deepCopyDictionary[(i - 2, j + 1)];
                        sw.Start();
                    }
                    catch
                    {
                        deepCopyDictionary[(i - 2, j + 1)] = random.Next(1, 100);
                        values[(i - 2, j + 1)] = deepCopyDictionary[(i - 2, j + 1)];
                        sw.Start();
                    }
                    deepCopyDictionary[(i, j)] = (deepCopyDictionary[(i - 1, j)])
                        + (deepCopyDictionary[(i - 1, j - 3)]) + (deepCopyDictionary[(i - 2, j + 1)]);
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

            int baseIterations = n / threadsAmount;
            int remainder = n % threadsAmount;

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
                        for (int i = 1; i <= m; i++)
                        {
                            int a = m * 3;
                            int b = m / 2;

                            int maxJ = Math.Max(1, 3 * i - a + k);
                            int minJ = Math.Min(n, (int)Math.Floor((double)(-i / 2 + b + k)));

                            for (int j = maxJ; j <= minJ; j++)
                            {
                                lock (lockObject)
                                {
                                    deepCopyDictionary[(i, j)] =
                                        deepCopyDictionary[(i - 1, j)] +
                                        deepCopyDictionary[(i - 1, j - 3)] +
                                        deepCopyDictionary[(i - 2, j + 1)];
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

            sw.Stop();

            Thread[] threads = new Thread[threadsAmount];
            int b = m / 2;
            int currentStartK = 1;

            for (int t = 0; t < threadsAmount; t++)
            {
                int iterationsForThread = n / threadsAmount + (t < n % threadsAmount ? 1 : 0);
                int startK = currentStartK;
                int endK = currentStartK + iterationsForThread - 1;
                currentStartK = endK + 1;
                int coreId = t % coresAmount;

                threads[t] = new Thread(() =>
                {
                    Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(1 << coreId);
                    for (int i = 1; i <= m; i++)
                    {
                        for (int k = startK; k <= endK; k++)
                        {
                            int minJ = Math.Min(n, (int)Math.Floor(-i / 2.0 + b + k));
                            for (int j = k; j <= minJ; j++)
                            {
                                lock (lockObject)
                                {
                                    deepCopyDictionary[(i, j)] =
                                        deepCopyDictionary[(i - 1, j)] +
                                        deepCopyDictionary[(i - 1, j - 3)] +
                                        deepCopyDictionary[(i - 2, j + 1)];
                                }
                            }
                        }
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
