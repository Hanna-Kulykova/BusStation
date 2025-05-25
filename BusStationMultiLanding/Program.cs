namespace BusStationMultiLanding;

class Program
{
    private static readonly object Lock = new();
    private static int _runsDone = 0;
    private static Dictionary<int, List<Passenger>> _waitingPassengers = new Dictionary<int, List<Passenger>>();
    private static List<Bus> _buses = new()
    {
        new Bus(175, 20),
        new Bus(42, 15),
        new Bus(7, 25)
    };

    private static Dictionary<int, int> _busesArrived = new Dictionary<int, int>();

    public static event Action<int>? BusArrived;

    static void Main()
    {
        foreach (var bus in _buses)
        {
            _waitingPassengers[bus.Number] = new List<Passenger>();
            _busesArrived[bus.Number] = 0;
        }

        BusArrived += (busNumber) => Console.WriteLine($"Автобус №{busNumber} під'їхав!");

        Thread passengerGenerator = new Thread(PassengerGenerator);
        List<Thread> busThreads = new List<Thread>();

        foreach (var bus in _buses)
        {
            Thread t = new Thread(() => BusArrival(bus));
            busThreads.Add(t);
            t.Start();
        }

        passengerGenerator.Start();

        passengerGenerator.Join();
        foreach (var t in busThreads)
            t.Join();

        Console.WriteLine("День на зупинці завершено.");
    }

    static void PassengerGenerator()
    {
        Random rnd = new Random();
        int totalRuns = _buses.Count;


        while (true)
        {
            Thread.Sleep(500);

            lock (Lock)
            {
                if (_runsDone >= totalRuns)
                {
                    Console.WriteLine("Всі автобуси від'їхали, пасажири більше не приходять.");
                    break;
                }
                
                int busIndex = rnd.Next(_buses.Count);
                int busNumber = _buses[busIndex].Number;
                int countNew = rnd.Next(1, 10);
                var newPassengers = new List<Passenger>();
                for (int i = 0; i < countNew; i++)
                {
                    newPassengers.Add(new Passenger(busNumber));
                }
                _waitingPassengers[busNumber].AddRange(newPassengers);

                Console.WriteLine($"{countNew} пасажирів прийшло на зупинку до автобуса №{busNumber}. Всього очікують: {_waitingPassengers[busNumber].Count}");
            }
        }
    }

    static void BusArrival(Bus bus)
    {
        Random rnd = new Random();
        Thread.Sleep(3000);

            lock (Lock)
            {
                BusArrived?.Invoke(bus.Number);
                if (bus.PassengersInBus > 0)
                {
                    int passengersLeaving = rnd.Next(0, bus.PassengersInBus + 1);
                    bus.PassengersInBus -= passengersLeaving;
                    Console.WriteLine($"{passengersLeaving} пасажирів вийшло з автобуса №{bus.Number}. " +
                                      $"В автобусі залишилось: {bus.PassengersInBus}");
                }
                
                int waitingCount = _waitingPassengers[bus.Number].Count;
                int freeSeats = bus.Capacity - bus.PassengersInBus;
                int taking = Math.Min(freeSeats, waitingCount);

                var boardingPassengers = _waitingPassengers[bus.Number].GetRange(0, taking);
                Console.WriteLine("Посаджені пасажири:");
                foreach (var p in boardingPassengers)
                    Console.WriteLine($"  {p}");
                _waitingPassengers[bus.Number].RemoveRange(0, taking);

                bus.PassengersInBus += taking;

                Console.WriteLine($"Автобус №{bus.Number} забрав {taking} пасажирів. Залишилось на зупинці: {_waitingPassengers[bus.Number].Count}");
                Console.WriteLine($"У салоні автобуса №{bus.Number} тепер: {bus.PassengersInBus}");
                _busesArrived[bus.Number]++;
                _runsDone++;
            }
    }
}