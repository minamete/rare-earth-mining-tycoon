using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class MiningSite : Building
{
	[Export] public string Description { get; set; }
	[Export] public string Title { get; set; }
	[Export] public string Location { get; set; }
	[Export] public float Cost { get; set; }
	[Export] public bool Available { get; set; }
	[Export] public string ImagePath { get; set; }
	[Export] public float BaseProductionRate { get; set; } // Base hourly production rate in tons
	private Dictionary<string, float> DropTable { get; set; }

	public MiningSite()
	{
		DropTable = new Dictionary<string, float>();
		Available = true;
	}

	public void SetDropTable(Godot.Collections.Dictionary dropTable) {
		foreach (string key in dropTable.Keys) {
			DropTable[key] = (float)dropTable[key];
		}
	}

	public override void _Ready()
	{
		base._Ready();
	}

	// Perform manual excavation
	public (string, float) Excavate()
	{
		float roll = GD.Randf() * 100;

		// Roll for each item in the drop table
		foreach (var item in DropTable)
		{
			string resourceName = item.Key;
			float chance = item.Value;

			// Simulate a drop based on the drop chance
			if (roll <= chance)
			{
				return (resourceName, chance);
			}
			
			roll -= chance;
		}
		return (DropTable.Keys.First(), DropTable.Values.First());
	}

	// Start an idle production process
	public void StartIdleProduction(string resource, float ratePerHour)
	{
		GD.Print($"Starting idle production for {resource} at {ratePerHour} per hour.");
		// Add logic for idle production
	}
}
