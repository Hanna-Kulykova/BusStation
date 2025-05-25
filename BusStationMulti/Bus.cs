namespace BusStationMulti;

public class Bus
{
    public int Number { get; }
    public int Capacity { get; }

    public Bus(int number, int capacity)
    {
        Number = number;
        Capacity = capacity;
    }
}