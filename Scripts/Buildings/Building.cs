using Godot;
using System.Collections.Generic;

public partial class Building : Node
{
	[Export] public string BuildingName { get; set; }
	[Export] public float Efficiency { get; set; }
	[Export] public float DailyCost { get; set; }
	protected List<Worker> Workers { get; set; }

	public Building()
	{
		Efficiency = 1.0f;
		Workers = new List<Worker>();
	}

	public void HireWorker(Worker worker)
	{
		Workers.Add(worker);
		AddChild(worker);
	}

	public void RemoveWorker(Worker worker)
	{
		Workers.Remove(worker);
		worker.QueueFree();
	}

	public void AssignWork(float hours)
	{
		foreach (var worker in Workers)
		{
			worker.Work(hours);
		}
	}

	public void PayWorkers()
	{
		foreach (var worker in Workers)
		{
			worker.GetPaid();
		}
	}
}
