using Godot;
using System;
using System.Collections.Generic;

public partial class GameManager : Node
{
	public static GameManager Instance { get; private set; }
	public CommodityMarket Market { get; private set; }
	private PackedScene _marketScene;
	private Player _player = new Player();
	private int days = 1;
	private Godot.Collections.Array<MiningSite> _miningSiteList = new Godot.Collections.Array<MiningSite>();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_marketScene = GD.Load<PackedScene>("res://Scenes/CommodityMarket.tscn");
		Market = _marketScene.Instantiate() as CommodityMarket;
		AddChild(Market);
		
		_player.OnMoneyChanged += OnPlayerChanged;
		_player.OnInventoryChanged += OnInventoryChanged;
		Instance = this;
		Json JSON = new Json();
		
		string filePath = "res://Resources/Data/MiningSites.tres";
		var jsonResource = ResourceLoader.Load(filePath);
		string jsonText = jsonResource.Get("data").ToString();

		var parsed = JSON.Parse(jsonText);
		if (parsed == Error.Ok)
		{
			var data = JSON.Data.AsGodotDictionary();
			var miningSites = data["MiningSites"].As<Godot.Collections.Array>();

			foreach (Godot.Collections.Dictionary siteData in miningSites)
			{

				CreateMiningSite(siteData);
			}
		}	
		
		var mainUIPanel = GetNode<MainUiPanel>("MainUiPanel");
		mainUIPanel.UpdateInternalMineSiteList(_miningSiteList);
		OnPlayerChanged(0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void AdvanceTime(float delta)
	{
		
	}
	
	// private instancers
	
	private void OnPlayerChanged(int val) {
		var mainUIPanel = GetNode<MainUiPanel>("MainUiPanel");
		mainUIPanel.UpdatePlayerInfo(_player);
		
		var marketPanel = GetNode<MarketPanel>("MainUiPanel/Separator/MainPanel/ImportantPanels/MarketPanel");
		marketPanel.UpdatePlayer(_player);
		
		var miningPanel = GetNode<MiningSitePanel>("MainUiPanel/Separator/MainPanel/ImportantPanels/MiningSitePanel");
		miningPanel.OnMoneyUpdated(_player.Money);
	}
	
	private void CreateMiningSite(Godot.Collections.Dictionary siteData)
	{
		var miningSiteScene = GD.Load<PackedScene>("res://Scenes/MiningSite.tscn");
		var miningSite = miningSiteScene.Instantiate<MiningSite>();

		miningSite.BuildingName = siteData["Name"].ToString();
		miningSite.SetDropTable(siteData["DropTable"].AsGodotDictionary());
		miningSite.BaseProductionRate = (float)siteData["BaseProductionRate"];
		miningSite.Title = siteData["Title"].ToString();
		miningSite.Description = siteData["Description"].ToString();
		miningSite.Location = siteData["Location"].ToString();
		miningSite.Cost = (float)siteData["Cost"];
		miningSite.ImagePath = siteData["ImagePath"].ToString();

		_miningSiteList.Add(miningSite);
		AddChild(miningSite);
	}
	
	public void OnMinePurchased(MiningSite site) {
		GD.Print($"Now managing mining site: {site.BuildingName}");
		_player.BuyMine(site);
		site.Available = false;
	}
	
	public (string, float) MineOnce(MiningSite site) {
		(string item, float chance) = site.Excavate();
		_player.AddResource(item, 1);
		_player.Money -= 10;
		return (item, chance);
	}
	
	private void OnDayTimerTimeout() {
		var timer = GetNode<Timer>("DayTimer");
		timer.Start(24);
		_player.DailyDecrementResources();
		
		days += 1;
		var mainUIPanel = GetNode<MainUiPanel>("MainUiPanel");
		mainUIPanel.UpdateDay(days);
		GD.Print("Day passed");
	}
	
	private void OnMarketTimerTimeout() {
		var timer = GetNode<Timer>("MarketTimer");
		timer.Start(1);
		Market.UpdatePrices();
		
		var marketPanel = GetNode<MarketPanel>("MainUiPanel/Separator/MainPanel/ImportantPanels/MarketPanel");
		marketPanel.UpdateMarkets(Market);
	}
	
	private void OnInventoryChanged(ResourceManager rss) {
		var marketPanel = GetNode<MarketPanel>("MainUiPanel/Separator/MainPanel/ImportantPanels/MarketPanel");
		marketPanel.UpdatePlayer(_player);
	}
}
