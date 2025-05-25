namespace GenerateNumbers;

class Program
{
    private static readonly object Lock = new();
    private static List<(int, int)> _pairs = new();
    private static bool _isGenerated = false;
    private static bool _isSumCalculated = false;
    public static event Action? GenerationCompleted;
    
    static void Main(string[] args)
    {
        GenerationCompleted += () => Console.WriteLine("Генерація пар завершена.");

        Thread generatorThread = new Thread(GeneratePairs);
        Thread sumThread = new Thread(CalculateSums);
        Thread productThread = new Thread(CalculateProducts);

        sumThread.Start();
        productThread.Start();
        generatorThread.Start();

        generatorThread.Join();
        sumThread.Join();
        productThread.Join();

        Console.WriteLine("Усі операції завершені.");
        
    }
    
    static void GeneratePairs()
    {
        lock (Lock)
        {
            Random rnd = new Random();
            for (int i = 0; i < 10; i++)
            {
                _pairs.Add((rnd.Next(1, 10), rnd.Next(1, 10)));
            }

            //Перевірка
            /*foreach (var pair in _pairs)
            {
                Console.WriteLine(pair);
            }*/
            
            File.WriteAllLines("pairs.txt", _pairs.ConvertAll(p => $"{p.Item1} {p.Item2}"));

            _isGenerated = true;

            GenerationCompleted?.Invoke();

            Monitor.PulseAll(Lock);
        }
    }

    static void CalculateSums()
    {
        lock (Lock)
        {
            while (!_isGenerated)
                Monitor.Wait(Lock);

            List<string> sums = new List<string>();
            foreach (var p in _pairs)
            {
                sums.Add((p.Item1 + p.Item2).ToString());
            }

            //Перевірка
            /*foreach (var sum in sums)
            {
                Console.WriteLine($"sum: {sum}");
            }*/
            
            File.WriteAllLines("sums.txt", sums);
            _isSumCalculated = true;
            Monitor.PulseAll(Lock);
        }
    }

    static void CalculateProducts()
    {
        lock (Lock)
        {
            while (!_isGenerated)
                Monitor.Wait(Lock);
            while (!_isSumCalculated)
                Monitor.Wait(Lock);

            List<string> products = new List<string>();
            foreach (var p in _pairs)
            {
                products.Add((p.Item1 * p.Item2).ToString());
            }
            
            //Перевірка
            /*foreach (var p in products)
            {
                Console.WriteLine($"product: {p}");
            }*/

            File.WriteAllLines("products.txt", products);
        }
    }
}