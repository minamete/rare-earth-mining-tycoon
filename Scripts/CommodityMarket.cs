using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Commodity 
{
	public string Name { get; set; }
	public string CommonName { get; set; }
	public List<string> Industries { get; set; }
	public float BasePrice { get; set; }
	public int MinTradingVolume { get; set; }
	public int MaxTradingVolume { get; set; }
	public int MinDemand { get; set; }
	public int MaxDemand { get; set; }
	public float MinIntradayVol { get; set; }
	public float MaxIntradayVol { get; set; }
	public float MinInterdayVol { get; set; }
	public float MaxInterdayVol { get; set; }
	public float CurrentPrice { get; set; }
	public List<(int time, float price)> History { get; set; }
	
	public void InitializePrice() => CurrentPrice = BasePrice;
	public void InitializeHistory() {
		History = new List<(int time, float price)> ();
		History.Add((0, BasePrice));
	}
}

public partial class CommodityMarket : Node
{
	private List<Commodity> commodities = new List<Commodity>();
	int time = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Loading commodities");
		_LoadCommodities("Resources/Data/Ores.tres");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void _LoadCommodities(string path)
	{
		GD.Print("Starting to load commodities");
		Json JSON = new Json();
		string fpath = "res://Resources/Data/Ores.tres";
		var json = ResourceLoader.Load(fpath);
		
		string jsonText = json.Get("data").ToString();
		var parsed = JSON.Parse(jsonText);
		if (parsed == Error.Ok)
		{
			var jsonArray = JSON.Data.AsGodotArray();
			foreach (var entry in jsonArray)
			{
				Commodity commodity = ParseCommodity(entry.AsGodotDictionary());
				commodities.Add(commodity);
				GD.Print($"Loaded commodity: {commodity.Name} at base price: {commodity.BasePrice}");
			}
		} else {
			GD.Print(JSON.GetErrorMessage());
		}
	}
	
	private Commodity ParseCommodity(Godot.Collections.Dictionary data)
	{
		GD.Print("Parsing commodities");
		List<string> industriesList = new List<string>();

		foreach (var item in data["Industries"].AsGodotArray())
		{
			industriesList.Add(item.ToString());
		}
		Commodity commodity = new Commodity ();
		commodity.Name = data["Name"].ToString();
		commodity.CommonName = data["CommonName"].ToString();
		commodity.Industries = industriesList;
		commodity.BasePrice = (float)(data["BasePrice"]);
		commodity.MinTradingVolume = (int)((data["TradingVolume"].AsGodotArray())[0]);
		commodity.MaxTradingVolume = (int)((data["TradingVolume"].AsGodotArray())[1]);
		commodity.MinDemand = (int)((data["Demand"].AsGodotArray())[0]);
		commodity.MaxDemand = (int)((data["Demand"].AsGodotArray())[1]);
		commodity.MinIntradayVol = (int)((data["IntradayVol"].AsGodotArray())[0]);
		commodity.MaxIntradayVol = (int)((data["IntradayVol"].AsGodotArray())[1]);
		commodity.MinInterdayVol = (int)((data["InterdayVol"].AsGodotArray())[0]);
		commodity.MaxInterdayVol = (int)((data["InterdayVol"].AsGodotArray())[1]);
		GD.Print("Parsed commodities");

		commodity.InitializePrice();
		commodity.InitializeHistory();
		return commodity;
	}
	
	public void SimulatePrice(Commodity commodity, float timeStep = 1.0f)
	{
		float intradayVolatility = (float)GD.RandRange(commodity.MinIntradayVol, commodity.MaxIntradayVol) / 100f;

		float priceChange = commodity.CurrentPrice * intradayVolatility * ((float)GD.RandRange(-0.5, 0.5)); 
		commodity.CurrentPrice = Mathf.Max(1, commodity.CurrentPrice + priceChange);
		commodity.History.Add((time, commodity.CurrentPrice));
		if (commodity.History.Count() > 180) commodity.History.RemoveAt(0);
		
	}

	public void UpdatePrices()
	{
		foreach (var commodity in commodities)
		{
			SimulatePrice(commodity);
		}
		time ++;
	}
	
	public Commodity GetCommodity(int indx) {
		return commodities[indx];
	}
	}
