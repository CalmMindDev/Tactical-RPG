/*using Godot;
using System;

public partial class CSMapNPC : Node
{
	private Node _parentMapNPC {get; set;}
	private int level {get; set;}
	private ClassData _classData {get; set;}


	public override void _ready() 
	{
		parentMapNPC = GetParent();
		if (parentMapNPC == null) GD.PrintErr("Not attached to Node");
		if (parentMapNPC.GetClass() != "GDMapNPC") GD.PrintErr("Not attached to GDMapNPC");
		
		var classData = GDScriptData.LoadClass(parentMapNPC.Get("MapClassData").get_name());
		level = _parentMapNPC.Get("Level") as int;


	}
}*/
