namespace BusStationMultiLanding;

public class Passenger
{
    private static int _idCounter = 0;
    public int Id { get; }
    public int BusNumber { get; }

    public Passenger(int busNumber)
    {
        Id = Interlocked.Increment(ref _idCounter);
        BusNumber = busNumber;
    }

    public override string ToString()
    {
        return $"Пасажир {Id} (на автобус №{BusNumber})";
    }
}