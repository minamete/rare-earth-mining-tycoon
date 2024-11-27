using Godot;
using System.Collections.Generic;

public partial class ResourceManager : Node
{
	private Dictionary<string, float> _resources = new Dictionary<string, float>();

	public ResourceManager(Dictionary<string, float> rs = null) {
		Json JSON = new Json();
		string filePath = "res://Resources/Data/Resources.tres";
		var jsonResource = ResourceLoader.Load(filePath);
		string jsonText = jsonResource.Get("data").ToString();

		var parsed = JSON.Parse(jsonText);
		if (parsed == Error.Ok)
		{
			var data = JSON.Data.AsGodotArray();

			foreach (Godot.Collections.Dictionary item in data)
			{
				if (rs != null && rs.ContainsKey(item["Name"].ToString())) {
					_resources[item["Name"].ToString()] = rs[item["Name"].ToString()];
				} else {
					_resources[item["Name"].ToString()] = 0;
				}
			}
		}
	}
	
	public override void _Ready()
	{
	}

	public void AddResource(string resource, float amount)
	{
		if (!_resources.ContainsKey(resource))
			_resources[resource] = 0;

		_resources[resource] += amount;
	}

	public bool SpendResource(string resource, float amount)
	{
		if (!_resources.ContainsKey(resource) || _resources[resource] < amount)
			return false;

		_resources[resource] -= amount;
		return true;
	}

	public float GetResourceAmount(string resource)
	{
		return _resources.ContainsKey(resource) ? _resources[resource] : 0;
	}
	
	public void PrintResourceTable() {
		GD.Print("Resource Table");
		foreach( var item in _resources.Keys) {
			GD.Print(item.ToString());
		}
	}
}
