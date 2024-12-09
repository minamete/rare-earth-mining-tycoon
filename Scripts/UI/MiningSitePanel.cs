using Godot;
using System;

public partial class MiningSitePanel : Container
{
	[Signal]
	public delegate void ExtractPressedEventHandler(string ore);
	private PackedScene popUpScene = GD.Load<PackedScene>("res://Scenes/Animations/PopUpText.tscn");
	private MiningSite _mineSite = null;
	private Godot.Collections.Array<MiningSite> _mineSiteList;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void OnMineListItemSelected(int index) {
		OnMineSiteUpdate(_mineSiteList[index]);
	}
	
	public void OnMoneyUpdated(float money) {
		var button = GetNode<Button>("MainLayoutMargin/MainLayout/MineDetails/MineControls/Extract");
		if (money <= 10) {
			button.Disabled = true;
		} else {
			button.Disabled = false;
		}
	}
	
	public void UpdateMineList(Godot.Collections.Array<MiningSite> msl) {
		_mineSiteList = msl;
		GD.Print("Updating");
		var mineListOptions = GetNode<OptionButton>("MainLayoutMargin/MainLayout/MineDetails/MineControls/SelectMine");
		mineListOptions.Clear();
		
		foreach (var mine in msl) {
			mineListOptions.AddItem(mine.BuildingName);
		}
		
		if (_mineSiteList.Count > 0) {
			OnMineListItemSelected(0);
		}
	}
	
	public void OnExtractPressed() {
		var button = GetNode<Button>("MainLayoutMargin/MainLayout/MineDetails/MineControls/Extract");
		//button.Disabled = true;
		var timer = GetNode<Timer>("MainLayoutMargin/MainLayout/MineDetails/MineControls/Extract/MineTimer");
		timer.Start();
		
		(string item, float chance) = GameManager.Instance.MineOnce(_mineSite);
		ShowMinedObject(item, chance);
	}
	
	public void ShowMinedObject(string text, float rarity) {
		Color color = MiningSite.GetColourFromRarity(rarity);		
		var button = GetNode<Button>("MainLayoutMargin/MainLayout/MineDetails/MineControls/Extract");
		var popup = popUpScene.Instantiate() as PopUpText;
		button.AddChild(popup);
		popup.Popup("+" + text, new Vector2(1400, 300), color);
	}
	
	public void OnMineSiteUpdate(MiningSite minesite) {
		_mineSite = minesite;
		
		var coverUp = GetNode<ColorRect>("CoverUpBg");
		var siteName = GetNode<Label>("MainLayoutMargin/MainLayout/SiteName");
		var refImage = GetNode<TextureRect>("MainLayoutMargin/MainLayout/MineDetails/RefImage");

		if (minesite == null) {
			coverUp.Show();
			return;
		}
		
		siteName.Text = minesite.BuildingName;
		var image = Image.LoadFromFile(minesite.ImagePath);
		var texture = ImageTexture.CreateFromImage(image);
		refImage.SetTexture(texture);
		
		coverUp.Hide();
	}
	
	private void OnMineTimerTimeout() {
		var button = GetNode<Button>("MainLayoutMargin/MainLayout/MineDetails/MineControls/Extract");
		//button.Disabled = false;
	}
}
