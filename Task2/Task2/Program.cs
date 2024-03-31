using System;
using System.Collections.Generic;

// Interface for notifying hazardous situations
public interface IHazardNotifier
{
    void NotifyHazardousSituation(string containerNumber);
}

// Base class for all containers
public class Container
{
    // Properties
    public string SerialNumber { get; }
    public double MassOfCargo { get; private set; }
    public double Height { get; }
    public double TareWeight { get; }
    public double Depth { get; }
    public double MaximumPayload { get; }

    // Constructor
    public Container(string serialNumber, double massOfCargo, double height, double tareWeight, double depth, double maximumPayload)
    {
        SerialNumber = serialNumber;
        MassOfCargo = massOfCargo;
        Height = height;
        TareWeight = tareWeight;
        Depth = depth;
        MaximumPayload = maximumPayload;
    }

    // Method to load cargo into the container
    public void LoadCargo(double cargoMass)
    {
        if (cargoMass > MaximumPayload)
        {
            throw new OverfillException("Cargo mass exceeds container's maximum payload.");
        }
        MassOfCargo = cargoMass;
    }

    // Method to empty the cargo from the container
    public void EmptyCargo()
    {
        MassOfCargo = 0;
    }
}

// Liquid container class
public class LiquidContainer : Container, IHazardNotifier
{
    // Properties specific to liquid containers
    public bool IsHazardous { get; }

    // Constructor
    public LiquidContainer(string serialNumber, double massOfCargo, double height, double tareWeight, double depth, double maximumPayload, bool isHazardous)
        : base(serialNumber, massOfCargo, height, tareWeight, depth, maximumPayload)
    {
        IsHazardous = isHazardous;
    }

    // Method to notify hazardous situation
    public void NotifyHazardousSituation(string containerNumber)
    {
        Console.WriteLine($"Hazardous situation in liquid container {containerNumber}");
    }

    // Method to load cargo into the liquid container
    public new void LoadCargo(double cargoMass)
    {
        if (IsHazardous)
        {
            if (cargoMass > MaximumPayload * 0.5)
            {
                NotifyHazardousSituation(SerialNumber);
            }
        }
        else
        {
            if (cargoMass > MaximumPayload * 0.9)
            {
                throw new OverfillException("Cargo mass exceeds container's maximum payload.");
            }
        }
        base.LoadCargo(cargoMass);
    }
}

// Gas container class
public class GasContainer : Container, IHazardNotifier
{
    // Properties specific to gas containers
    public double Pressure { get; }

    // Constructor
    public GasContainer(string serialNumber, double massOfCargo, double height, double tareWeight, double depth, double maximumPayload, double pressure)
        : base(serialNumber, massOfCargo, height, tareWeight, depth, maximumPayload)
    {
        Pressure = pressure;
    }

    // Method to notify hazardous situation
    public void NotifyHazardousSituation(string containerNumber)
    {
        Console.WriteLine($"Hazardous situation in gas container {containerNumber}");
    }

    // Method to load cargo into the gas container
    public new void LoadCargo(double cargoMass)
    {
        if (cargoMass > MaximumPayload)
        {
            throw new OverfillException("Cargo mass exceeds container's maximum payload.");
        }
        base.LoadCargo(cargoMass);
    }
}

// Refrigerated container class
public class RefrigeratedContainer : Container
{
    // Properties specific to refrigerated containers
    public string ProductType { get; }
    public double Temperature { get; }

    // Constructor
    public RefrigeratedContainer(string serialNumber, double massOfCargo, double height, double tareWeight, double depth, double maximumPayload, string productType, double temperature)
        : base(serialNumber, massOfCargo, height, tareWeight, depth, maximumPayload)
    {
        ProductType = productType;
        Temperature = temperature;
    }
}

// Container ship class
public class ContainerShip
{
    // Properties
    public List<Container> Containers { get; }
    public double MaxSpeed { get; }
    public int MaxNumberOfContainers { get; }
    public double MaxWeightCapacity { get; }

    // Constructor
    public ContainerShip(double maxSpeed, int maxNumberOfContainers, double maxWeightCapacity)
    {
        Containers = new List<Container>();
        MaxSpeed = maxSpeed;
        MaxNumberOfContainers = maxNumberOfContainers;
        MaxWeightCapacity = maxWeightCapacity;
    }

    // Method to load a container onto the ship
    public void LoadContainer(Container container)
    {
        if (Containers.Count >= MaxNumberOfContainers)
        {
            throw new InvalidOperationException("Cannot load more containers, ship is full.");
        }

        if ((Containers.Sum(c => c.MassOfCargo) + container.MassOfCargo) > MaxWeightCapacity)
        {
            throw new InvalidOperationException("Cannot load container, weight capacity exceeded.");
        }

        Containers.Add(container);
    }

    // Method to remove a container from the ship
    public void RemoveContainer(Container container)
    {
        Containers.Remove(container);
    }

    // Method to unload a container
    public Container UnloadContainer(string containerSerialNumber)
    {
        var container = Containers.FirstOrDefault(c => c.SerialNumber == containerSerialNumber);
        if (container != null)
        {
            Containers.Remove(container);
        }
        return container;
    }
}

// Custom exception for overfilling containers
public class OverfillException : Exception
{
    public OverfillException(string message) : base(message)
    {
    }
}
class Program
{
    static void Main(string[] args)
    {
        try
        {
            // Create a liquid container
            LiquidContainer liquidContainer = new LiquidContainer("KON-L-1", 0, 100, 50, 60, 500, true);
            liquidContainer.LoadCargo(200); // Should throw OverfillException

            // Create a gas container
            GasContainer gasContainer = new GasContainer("KON-G-1", 0, 120, 55, 65, 1000, 10);
            gasContainer.LoadCargo(1200); // Should throw OverfillException

            // Create a refrigerated container
            RefrigeratedContainer refrigeratedContainer = new RefrigeratedContainer("KON-C-1", 0, 110, 52, 62, 800, "Bananas", 4);
            refrigeratedContainer.LoadCargo(700); // Should load successfully

            // Create a container ship
            ContainerShip containerShip = new ContainerShip(20, 10, 10000);
            containerShip.LoadContainer(liquidContainer);
            containerShip.LoadContainer(gasContainer);
            containerShip.LoadContainer(refrigeratedContainer);

            // Unload a container
            Container unloadedContainer = containerShip.UnloadContainer("KON-L-1");
            if (unloadedContainer != null)
            {
                Console.WriteLine($"Container {unloadedContainer.SerialNumber} unloaded successfully.");
            }
            else
            {
                Console.WriteLine("Container not found on the ship.");
            }
        }
        catch (OverfillException ex)
        {
            Console.WriteLine($"Overfill Exception: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Invalid Operation Exception: {ex.Message}");
        }
    }
}