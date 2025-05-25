namespace BusStation;

class Program
{
    private static readonly object Lock = new();

    private static int _passengersAtStop = 0;
    private const int BusCapacity = 20;
    private const int BusCount = 5;
    private static int _busesArrived = 0;

    public static event Action? BusArrived;

    static void Main()
    {
        BusArrived += () => Console.WriteLine($"Автобус №175 під'їхав!");

        Thread passengerGenerator = new Thread(PassengerGenerator);
        Thread busThread = new Thread(BusArrival);

        passengerGenerator.Start();
        busThread.Start();

        passengerGenerator.Join();
        busThread.Join();

        Console.WriteLine("День на зупинці завершено.");
    }
    
    static void PassengerGenerator()
    {
        Random rnd = new Random();

        for (int i = 0; i < 20; i++)
        {
            Thread.Sleep(500);
            lock (Lock)
            {
                if (_busesArrived >= BusCount)
                {
                    Console.WriteLine("Всі автобуси від’їхали, нові пасажири не приходять.");
                    break;
                }
                int newPassengers = rnd.Next(1, 10);
                _passengersAtStop += newPassengers;
                Console.WriteLine($"{newPassengers} пасажирів прийшло на зупинку. " +
                                  $"Всього на зупинці: {_passengersAtStop}");
                Monitor.Pulse(Lock);
            }
        }
    }
    
    static void BusArrival()
    {
        while (true)
        {
            Thread.Sleep(2000);
            lock (Lock)
            {
                if (_busesArrived >= BusCount)
                    break;
                BusArrived?.Invoke();

                int takingPassengers = Math.Min(BusCapacity, _passengersAtStop);
                _passengersAtStop -= takingPassengers;
                Console.WriteLine($"Автобус забрав {takingPassengers} пасажирів. " +
                                  $"Залишилось на зупинці: {_passengersAtStop}");
                _busesArrived++;
                Thread.Sleep(1000);

                Monitor.Pulse(Lock);
            }
        }
    }
}