using Godot;
using System;
using System.Collections.Generic;

public partial class MainUiPanel : CanvasLayer
{
	private Godot.Collections.Array<MiningSite> _availableMines;
	private PackedScene _miningSiteChooserPanel;
	private Player _player;
	
	public void UpdateDay(int day) {
		var dayLabel = GetNode<Label>("Separator/TopPanel/DayLabel");
		dayLabel.Text = "Day " + day.ToString();
	}
	
	public void UpdatePlayerInfo(Player player) {
		_player = player;
		
		var companyName = GetNode<Label>("Separator/TopPanel/CompanyName");
		var moneyLabel = GetNode<Label>("Separator/TopPanel/MoneyLabel");
		var miningPanel = GetNode<MiningSitePanel>("Separator/MainPanel/MiningSitePanel");
		_player.OnOwnedSitesChanged += miningPanel.OnMineSiteUpdate;
		miningPanel.UpdateMineList(player.OwnedSites);
		
		companyName.Text = _player.CompanyName;
		moneyLabel.Text = "Money: $" + _player.Money.ToString();
	}

	
	public void UpdateInternalMineSiteList(Godot.Collections.Array<MiningSite> ms) {
		_availableMines = ms;
		if (HasNode("mscp")) {
			var mscp = GetNode<MiningSiteChooserPanel>("mscp");
			mscp.AvailableMines = ms;
		}
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_miningSiteChooserPanel = GD.Load<PackedScene>("res://Scenes/UI/MiningSiteChooserPanel.tscn");

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void OnNewMiningSiteButtonPressed() {
		if (HasNode("mscp")) {
			return;
		}
		MiningSiteChooserPanel mscp = _miningSiteChooserPanel.Instantiate() as MiningSiteChooserPanel;
		mscp.SetName("mscp");
		mscp.CloseRequested += () => mscp.QueueFree();
		var unpurchasedMines = new Godot.Collections.Array<MiningSite>();
		foreach(var mine in _availableMines) {
			if (mine.Available) {
				unpurchasedMines.Add(mine);
			}
		}
		mscp.AvailableMines = unpurchasedMines;
		mscp.AvailableMoney = _player.Money;
		mscp.Connect("OnMinePurchased", new Callable(GameManager.Instance, "OnMinePurchased"));
		AddChild(mscp);
	}
}