using Godot;
using System;
using System.Collections.Generic;

public partial class MainUiPanel : CanvasLayer
{
	private Godot.Collections.Array<MiningSite> _availableMines;
	private PackedScene _miningSiteChooserPanel;
	private PackedScene _inventoryPanel;
	private Player _player;
	
	public void UpdateDay(int day) {
		var dayLabel = GetNode<Label>("Separator/TopPanel/DayLabel");
		dayLabel.Text = "Day " + day.ToString();
	}
	
	public void UpdatePlayerInfo(Player player) {
		_player = player;
		
		var companyName = GetNode<Label>("Separator/TopPanel/CompanyName");
		var moneyLabel = GetNode<Label>("Separator/TopPanel/MoneyLabel");
		var miningPanel = GetNode<MiningSitePanel>("Separator/MainPanel/ImportantPanels/MiningSitePanel");
		_player.OnOwnedSitesChanged += miningPanel.OnMineSiteUpdate;
		miningPanel.UpdateMineList(player.OwnedSites);
		
		companyName.Text = _player.CompanyName;
		moneyLabel.Text = "Money: $" + _player.Money.ToString();
		
		if (HasNode("inv")) {
			var inv = GetNode<InventoryPanel>("inv");
			inv.UpdateInventory(_player.Resources.ShowResources());
		}
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
		_inventoryPanel = GD.Load<PackedScene>("res://Scenes/UI/InventoryPanel.tscn");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void OnInventoryPressed() {
		if (HasNode("inv")) {
			return;
		}
		InventoryPanel inv = _inventoryPanel.Instantiate() as InventoryPanel;
		inv.SetName("inv");
		inv.CloseRequested += () => inv.QueueFree();
		inv.UpdateInventory(_player.Resources.ShowResources());
		AddChild(inv);
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
	
	private void OnMiningSitePressed() {
		var mine = GetNode<MiningSitePanel>("Separator/MainPanel/ImportantPanels/MiningSitePanel");
		mine.Show();
		
		var marketPanel = GetNode<MarketPanel>("Separator/MainPanel/ImportantPanels/MarketPanel");
		marketPanel.Hide();
	}
	
	private void OnMarketPressed() {
		var mine = GetNode<MiningSitePanel>("Separator/MainPanel/ImportantPanels/MiningSitePanel");
		mine.Hide();
		
		var marketPanel = GetNode<MarketPanel>("Separator/MainPanel/ImportantPanels/MarketPanel");
		marketPanel.Show();
	}
}
