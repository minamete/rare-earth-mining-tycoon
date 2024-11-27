using Godot;
using System;
using System.Collections.Generic;

public partial class InventoryPanel : Window
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void UpdateInventory(Dictionary<string, float> inventory) {
		var id = GetNode<GridContainer>("MarginContainer/VBoxContainer/ScrollContainer/InventoryDisplay");
		var idChildren = id.GetChildren();
		foreach (var child in idChildren) {
			child.QueueFree();
		}
		
		foreach (var key in inventory.Keys) {
			if (inventory[key] == 0) continue;
			var keyLabel = new Label();
			keyLabel.Text = key;
			var valLabel = new Label();
			valLabel.Text = inventory[key].ToString();
			id.AddChild(keyLabel);
			id.AddChild(valLabel);
		}
	}
}
