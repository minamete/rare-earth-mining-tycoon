using Godot;
using System;

public partial class PopUpText : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void OnAnimationFinished(string animationName)
	{
		QueueFree(); // Removes the node from the scene
	}

	public void Popup(string text, Vector2 position, Color color)
	{
		var label = GetNode<Label>("PopUpText");
		label.Text = text; 
		label.Position = position;
		label.Modulate = color;
		var animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		var animation = animationPlayer.GetAnimation("label_popup_animation");

		NodePath nodePath = ".";
		animation.TrackSetKeyValue(0, 0, position); 
		animation.TrackSetKeyValue(0, 1, new Vector2(position.X, position.Y - 100));
		animation.TrackSetKeyValue(1, 0, color); 

		animationPlayer.Play("label_popup_animation");		
	}
}
