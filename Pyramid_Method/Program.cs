using System.Diagnostics;
using ConsoleTables;
using System.Text;

namespace Pyramid_Method
{
    internal class Program
    {
        static ConsoleTable finalTable = new ConsoleTable("Номер експерименту", "Кількість ядер", "Кількість потоків", "Метод", "Час (мс)", "Прискорення обчислень");
        static int experimentNumber = 0;

        public static void CalculateCycles(
            Dictionary<(int, int), int> values,
            int n,
            int m,
            int threadsAmount,
            int coresAmount,
            Func<Dictionary<(int, int), int>, Stopwatch, int, int, Dictionary<(int, int), int>> seqCalculation,
            Func<Dictionary<(int, int), int>, int, int, int, int, Stopwatch, Dictionary<(int, int), int>> pyramidCalculation,
            Func<Dictionary<(int, int), int>, int, int, int, int, Stopwatch, Dictionary<(int, int), int>> modifiedPyramidCalculation,
            Func<Dictionary<(int, int), int>, Dictionary<(int, int), int>, int, int, bool> checkResuts
            )
        {
            Stopwatch sequentialStopwatch = Stopwatch.StartNew();
            Dictionary<(int, int), int> sequentialResult = seqCalculation(values, sequentialStopwatch, n, m);
            sequentialStopwatch.Stop();
            Console.WriteLine($"Послідовне виконання зайняло : {sequentialStopwatch.ElapsedMilliseconds} мс\n");

            Stopwatch pyramidStopwatch = Stopwatch.StartNew();
            Dictionary<(int, int), int> pyramidResult = pyramidCalculation(values, n, m, coresAmount, threadsAmount, pyramidStopwatch);
            pyramidStopwatch.Stop();
            Console.WriteLine($"Виконання методу пірамід зайняло: {pyramidStopwatch.ElapsedMilliseconds} мс\n");
            Console.WriteLine($"Прискорення обчислень для методу пірамід: {sequentialStopwatch.Elapsed / pyramidStopwatch.Elapsed}\n");

            Stopwatch modifiedPyramidStopwatch = Stopwatch.StartNew();
            Dictionary<(int, int), int> modifiedPyramidResult = modifiedPyramidCalculation(values, n, m, threadsAmount, coresAmount, modifiedPyramidStopwatch);
            modifiedPyramidStopwatch.Stop();
            Console.WriteLine($"Виконання модифікованого методу пірамід зайняло: {modifiedPyramidStopwatch.ElapsedMilliseconds} мс\n");
            Console.WriteLine($"Прискорення обчислень для модифікованого методу пірамід: {sequentialStopwatch.Elapsed / modifiedPyramidStopwatch.Elapsed}\n");

            if (checkResuts(pyramidResult, sequentialResult, n, m) && checkResuts(sequentialResult, modifiedPyramidResult, n, m))
            {
                Console.WriteLine("Результати методів однакові.");
            }
            else
            {
                Console.WriteLine("Помилка! Результати методів НЕ однакові!");
            }

            double sequentialTime = sequentialStopwatch.Elapsed.TotalMilliseconds;
            double pyramidTime = pyramidStopwatch.Elapsed.TotalMilliseconds;
            double pyramidSpeedup = sequentialTime / pyramidTime;
            double modifiedPyramidTime = modifiedPyramidStopwatch.Elapsed.TotalMilliseconds;
            double modifiedPyramidSpeedup = sequentialTime / modifiedPyramidTime;

            ConsoleTable table = new ConsoleTable("Метод", "Час (мс)", "Прискорення обчислень");
            table.AddRow("Послідовне виконання", sequentialTime.ToString("F3"), "-");
            experimentNumber++;
            finalTable.AddRow(experimentNumber, coresAmount, threadsAmount, "Послідовне виконання", sequentialTime.ToString("F3"), "-");
            table.AddRow("Пірамід", pyramidTime.ToString("F3"), pyramidSpeedup.ToString("F3"));
            experimentNumber++;
            finalTable.AddRow(experimentNumber, coresAmount, threadsAmount, "Пірамід", pyramidTime.ToString("F3"), pyramidSpeedup.ToString("F3"));
            table.AddRow("Модифікація методу пірамід", modifiedPyramidTime.ToString("F3"), modifiedPyramidSpeedup.ToString("F3"));
            experimentNumber++;
            finalTable.AddRow(experimentNumber, coresAmount, threadsAmount, "Модифікація методу пірамід", modifiedPyramidTime.ToString("F3"), modifiedPyramidSpeedup.ToString("F3"));
            Console.WriteLine(table);
        }


        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            Dictionary<(int, int), int> values = new Dictionary<(int, int), int>();

            int coresAmount = 4;
            int n = 100000;
            int m = 10;
            int threadsAmount = coresAmount * m;

            Console.WriteLine("Виконання циклів (19), (20), (21):");
            CalculateCycles(values, n, m, threadsAmount, coresAmount, CyclesSet1.SequentialCalculation, CyclesSet1.PyramidMethod, CyclesSet1.ModifiedPyramidMethod, CyclesSet1.ResultsEqual);
            Console.WriteLine("--------------------------------------------------------------------------------------------------------------");            

            Console.WriteLine("Виконання циклів (5), (6), (16):");
            CalculateCycles(values, n, m, threadsAmount, coresAmount, CyclesSet2.SequentialCalculation, CyclesSet2.PyramidMethod, CyclesSet2.ModifiedPyramidMethod, CyclesSet2.ResultsEqual);
            Console.WriteLine("--------------------------------------------------------------------------------------------------------------");

            n = 100;
            m = 1000;
            threadsAmount = coresAmount;

            Console.WriteLine("Виконання циклів (22), (23), (24):");
            CalculateCycles(values, n, m, threadsAmount, coresAmount, CyclesSet3.SequentialCalculation, CyclesSet3.PyramidMethod, CyclesSet3.ModifiedPyramidMethod, CyclesSet3.ResultsEqual);
            Console.WriteLine("--------------------------------------------------------------------------------------------------------------");

            Console.WriteLine(finalTable);
        }
    }
}