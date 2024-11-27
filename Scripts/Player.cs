using Godot;
using System;
using System.Collections.Generic;

public partial class Player : Node
{
	[Signal] public delegate void OnMoneyChangedEventHandler(int newValue);
	[Signal] public delegate void OnOwnedSitesChangedEventHandler(MiningSite site);
	[Export] public string CompanyName { get; set; }
	private float _money;
	[Export] public float Money { 
		get {
			return _money;
		}
		set {
			_money = value;
			EmitSignal(SignalName.OnMoneyChanged, value);
		}
	}
	[Export] public Godot.Collections.Array<MiningSite> OwnedSites { get; set; }
	[Export] public ResourceManager Resources { get; set; }
	
	public Player(string name = "Rare Earth Mining Co.", float money = 200000.0f, Godot.Collections.Array<MiningSite> sites = null, ResourceManager rs = null) {
		CompanyName = name;
		Money = money;
		OwnedSites = sites ?? new Godot.Collections.Array<MiningSite>();
		Resources = rs ?? new ResourceManager();
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void DailyDecrementResources()
	{
		foreach (var mine in OwnedSites) {
			Money -= mine.DailyCost;
		}
	}
	
	public void BuyMine(MiningSite mine) {
		OwnedSites.Add(mine);
		Money -= mine.Cost;
		EmitSignal(SignalName.OnOwnedSitesChanged, mine);
	}
}
