using Godot;
using System;
using System.Collections.Generic;

public partial class GraphDisplay : Control
{
	private List<Vector2> dataPoints = new List<Vector2>(); // Stock price points
	private float minPrice, maxPrice;
	private float timeSpan;
	private FontFile font;
	
	public void SetData(List<(int time, float price)> prices, float timeInterval)
	{
		dataPoints.Clear();
		timeSpan = timeInterval;

		minPrice = float.MaxValue;
		maxPrice = float.MinValue;

		for (int i = 0; i < prices.Count; i++)
		{
			dataPoints.Add(new Vector2(i * timeInterval, prices[i].Item2));
			minPrice = Mathf.Min(minPrice, prices[i].Item2);
			maxPrice = Mathf.Max(maxPrice, prices[i].Item2);
		}

		QueueRedraw();
	}
	
	public override void _Ready()
	{
	 	font = new FontFile();
		font.LoadDynamicFont("res://Resources/Assets/Fonts/Lato-Regular.ttf");
	}

	public override void _Draw()
	{
		if (dataPoints.Count < 2 || maxPrice - minPrice == 0) return;

		// Normalize data for graph dimensions
		float graphWidth = Size.X;
		float graphHeight = Size.Y;
		
		// Draw axes
		DrawLine(new Vector2(Position.X, Position.Y + graphHeight), new Vector2(Position.X + graphWidth, Position.Y + graphHeight), Colors.White, 2);
		DrawLine(Position, new Vector2(Position.X, Position.Y + graphHeight), Colors.White, 2);

		float range = maxPrice - minPrice;
		float tickStep = range / 10;
		float scaleY = (graphHeight - 40) / (range);
		for (int i = 0; i <= 10; i++)
		{
			float val = minPrice + i * tickStep;
			float y = Position.Y + graphHeight - (val - minPrice) * scaleY;
			Vector2 tickPos = new Vector2(Position.X, y);

			DrawLine(tickPos, tickPos + new Vector2(10, 0), Colors.White, 1);
			DrawString(font, tickPos + new Vector2(15, -5), val.ToString("F2")); 
		}
		
		Vector2 lastPoint = Normalize(dataPoints[0], graphWidth, graphHeight);
		for (int i = 1; i < dataPoints.Count; i++)
		{
			Vector2 currentPoint = Normalize(dataPoints[i], graphWidth, graphHeight);
			DrawLine(lastPoint, currentPoint, Colors.LightBlue, 2);
			lastPoint = currentPoint;
		}
	}

	private Vector2 Normalize(Vector2 point, float width, float height)
	{
		// Map x and y values to fit graph dimensions
		float x = (point.X / (timeSpan * (dataPoints.Count - 1))) * (width - 20);
		float y = height - ((point.Y - minPrice) / (maxPrice - minPrice) * (height - 40));
		return new Vector2(x, y);
	}
}
