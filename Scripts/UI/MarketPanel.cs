using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class MarketPanel : MarginContainer
{
	private CommodityMarket _currentMarket;
	private int _currentCommodityIndx = 0; 
	private Player _player;
	private List<string> _playerResources = new List<string>();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	
	}
	
	public void UpdatePlayer(Player p)
	{
		_player = p;
		var node = GetNode<VBoxContainer>("MainLayout/MainVBox");
		var msg = GetNode<ColorRect>("ColorRect");
		if (_player.Resources.HasNothing()) {
			node.Hide();
			msg.Show();
		} else {
			node.Show();
			msg.Hide();
			
			// player has resources, so this will be nonzero
			_playerResources = _player.Resources.GetNonzeroResources();
			// find this resource in CommodityMarket
			// if it is not nil, then it was there before, so make sure.
			if (!_playerResources.Any(item => item == _currentMarket.GetCommodity(_currentCommodityIndx).Name)) {
				_currentCommodityIndx = _currentMarket.GetCommodityFromName(_playerResources[0]);
			}
			
			var selectButton = GetNode<OptionButton>("MainLayout/MainVBox/MainArea/VBoxContainer/SelectComm");
			selectButton.Clear();
			foreach (var rss in _playerResources) {
				selectButton.AddItem(rss);
			}
			selectButton.Selected = _playerResources.FindIndex(item => item == _currentMarket.GetCommodity(_currentCommodityIndx).Name);
			_updateGraphData();
		}
	}
	
	public void UpdateMarkets(CommodityMarket m) {
		_currentMarket = m;
		_updateGraphData();
	}
	
	public void OnSellOnePressed() {
		sellSelected(1);
	}
	
	public void OnSellTenPressed() {
		sellSelected(10);
	}
	
	public void OnSellHundredPressed() {
		sellSelected(100);
	}
	
	public void OnBuyOnePressed() {
		buySelected(1);
	}
	
	public void OnBuyTenPressed() {
		buySelected(10);
	}
	
	public void OnBuyHundredPressed() {
		buySelected(100);
	}	
	
	private void sellSelected(int num) {
		var commodity = _currentMarket.GetCommodity(_currentCommodityIndx);
		_player.SellResource(commodity.Name, num, num * commodity.CurrentPrice);
	}
	
	private void buySelected(int num) {
		var commodity = _currentMarket.GetCommodity(_currentCommodityIndx);
		_player.BuyResource(commodity.Name, num, num * commodity.CurrentPrice);
	}
	
	private void OnSelectCommItem(int item) {
		_currentCommodityIndx = _currentMarket.GetCommodityFromName(_playerResources[item]);
		_updateGraphData();
	}
	
	private void _updateGraphData() {
		var currentCommodity = _currentMarket.GetCommodity(_currentCommodityIndx);
		var graph = GetNode<GraphDisplay>("MainLayout/MainVBox/MainArea/Graph/GraphDisplay");
		graph.SetData(currentCommodity.History, 50);
		
		var price = GetNode<Label>("MainLayout/MainVBox/MainArea/VBoxContainer/PriceLabel");
		price.Text = "Price per ton: $" + currentCommodity.CurrentPrice.ToString("F2");
		
		var title = GetNode<Label>("MainLayout/MainVBox/Title");
		title.Text = "Raw Ores Market - " + currentCommodity.Name;
		
		var ownLabel = GetNode<Label>("MainLayout/MainVBox/MainArea/VBoxContainer/OwnLabel");
		ownLabel.Text = $"Owned: {(_player.Resources.GetResourceAmount(currentCommodity.Name)).ToString()}";
	
		var sellOneButton = GetNode<Button>("MainLayout/MainVBox/MainArea/VBoxContainer/SellOne");
		var sellTenButton = GetNode<Button>("MainLayout/MainVBox/MainArea/VBoxContainer/SellTen");
		var sellHundredButton = GetNode<Button>("MainLayout/MainVBox/MainArea/VBoxContainer/SellHundred");
		float ct = _player.Resources.GetResourceAmount(currentCommodity.Name);
		if ( ct >= 1 ) {
			sellOneButton.Disabled = false;
		} else {
			sellOneButton.Disabled = true;
		}
		if (ct >= 10) {
			sellTenButton.Disabled = false;
		} else {
			sellTenButton.Disabled = true;
		}
		if (ct >= 100) {
			sellHundredButton.Disabled = false;
		} else {
			sellHundredButton.Disabled = true;
		}
		var money = _player.Money;
		var buyOneButton = GetNode<Button>("MainLayout/MainVBox/MainArea/VBoxContainer/BuyOne");
		var buyTenButton = GetNode<Button>("MainLayout/MainVBox/MainArea/VBoxContainer/BuyTen");
		var buyHundredButton = GetNode<Button>("MainLayout/MainVBox/MainArea/VBoxContainer/BuyHundred");
		if (money >= currentCommodity.CurrentPrice) {
			buyOneButton.Disabled = false;
		} else {
			buyOneButton.Disabled = true;
		}
		if (money >= currentCommodity.CurrentPrice * 10) {
			buyTenButton.Disabled = false;
		} else {
			buyTenButton.Disabled = true;
		}
		if (money >= currentCommodity.CurrentPrice * 100) {
			buyHundredButton.Disabled = false;
		} else {
			buyHundredButton.Disabled = true;
		}
	}
}
