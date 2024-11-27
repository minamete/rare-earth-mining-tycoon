using Godot;
using System;

public partial class Worker : Node
{
	[Export] public string WorkerName { get; set; }
	[Export] public float Productivity { get; set; }
	[Export] public float Morale { get; set; }
	[Export] public float Fatigue { get; set; }
	[Export] public float Salary { get; set; }

	public Worker(string name, float productivity, float morale, float salary)
	{
		WorkerName = name;
		Productivity = productivity;
		Morale = morale;
		Fatigue = 0;
		Salary = salary;
	}
	
	public void Work(float hours)
	{
		if (Morale <= 0) Morale = 0;  // Prevent morale from going negative
		Productivity *= Morale / 100f;  // Productivity is affected by morale
		Fatigue += hours * 0.1f;  // Workers get fatigued over time

		if (Fatigue > 100)
		{
			Morale -= 5;  // Lower morale if fatigue is too high
		}
	}
	
	public void GetPaid()
	{
		Morale += 5;  // Boost morale with salary payment
		Fatigue = Mathf.Max(Fatigue - 10, 0);  // Slightly reduce fatigue
	}
	
	// Rests fully
	public void Rest(float hours)
	{
		Fatigue = Mathf.Max(Fatigue - 10 * hours, 0);
		Morale += 10; 
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
