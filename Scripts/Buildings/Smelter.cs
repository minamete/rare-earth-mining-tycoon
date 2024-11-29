using Godot;
using System;
using System.Collections.Generic;

public class Ore 
{
	string Name;
	
}

public partial class Smelter : Building
{
	// efficiency always starts at 100 ores => 1 
	public Dictionary<Ore, float> Efficiencies; // in %
	public Dictionary<Ore, float> Temperatures;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
