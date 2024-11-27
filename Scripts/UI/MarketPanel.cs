using Godot;
using System;
using System.Collections.Generic;

public partial class MarketPanel : MarginContainer
{
	private CommodityMarket _currentMarket;
	private int _currentCommodityIndx = 0;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	
	}
	
	public void UpdateMarkets(CommodityMarket m) {
		_currentMarket = m;
		
		var graph = GetNode<GraphDisplay>("MainLayout/MainVBox/MainArea/Graph/GraphDisplay");
		graph.SetData(m.GetCommodity(_currentCommodityIndx).History, 50);
		
		var price = GetNode<Label>("MainLayout/MainVBox/MainArea/VBoxContainer/PriceLabel");
		price.Text = "Price per ton: $" + m.GetCommodity(_currentCommodityIndx).CurrentPrice.ToString("F2");
	}
}
