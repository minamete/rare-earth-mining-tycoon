using Godot;
using System;

public partial class MiningSiteChooserPanel : Window
{
	[Signal]
	public delegate void OnMinePurchasedEventHandler(MiningSite site);
	private float _availableMoney;
	[Export] public float AvailableMoney { 
		get {
			return _availableMoney;	
		}
		set {
			_availableMoney = value;
			OnPropertyChanged();
		}
	}
	private Godot.Collections.Array<MiningSite> _availableMines;
	[Export] public Godot.Collections.Array<MiningSite> AvailableMines { 
		get {
			return _availableMines;
		}
		set {
			_availableMines = value;
			OnPropertyChanged();
		}
	}
	private MiningSite _activeMine = null;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void OnMineListItemSelected(int index)
	{
		_activeMine = _availableMines[index];
		var nameLabel = GetNode<Label>("MainLayout/MineDescription/Name");
		var titleLabel = GetNode<Label>("MainLayout/MineDescription/Title");
		var descriptionLabel = GetNode<Label>("MainLayout/MineDescription/Description");
		var costLabel = GetNode<Label>("MainLayout/MineDescription/Cost");
		
		nameLabel.Text = _activeMine.BuildingName;
		titleLabel.Text = _activeMine.Title;
		descriptionLabel.Text = _activeMine.Description;	
		costLabel.Text = "Cost: $" + _activeMine.Cost.ToString();
	
		if (_activeMine != null) {
			var buyButton = GetNode<Button>("MainLayout/BuyButton");
			if (AvailableMoney >= _activeMine.Cost) {
				costLabel.Modulate = Colors.Green;
				buyButton.Disabled = false;
			} else {
				costLabel.Modulate = Colors.Red;
				buyButton.Disabled = true;
			}
		}
	}
	
	public void OnPropertyChanged()
	{
		var ml = GetNode<Label>("MainLayout/MoneyLabel");
		var selectMinesButton = GetNode<OptionButton>("MainLayout/MineList");
		
		ml.Text = "Money: $" + AvailableMoney.ToString();	
		selectMinesButton.Clear();
		
		foreach (var mine in AvailableMines) {
			selectMinesButton.AddItem(mine.BuildingName);
		}
		if (AvailableMines.Count > 0) {
			OnMineListItemSelected(0);
		}
	}
	
	public void OnBuyButtonPressed() {
		EmitSignal(SignalName.OnMinePurchased, _activeMine);
		QueueFree();
	}
}
