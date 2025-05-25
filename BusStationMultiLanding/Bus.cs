namespace BusStationMultiLanding;

public class Bus
{
    public int Number { get; }
    public int Capacity { get; }
    public int PassengersInBus { get; set; }
    public Bus(int number, int capacity)
    {
        Number = number;
        Capacity = capacity;
        PassengersInBus = Random.Shared.Next(0, capacity);
    }
}