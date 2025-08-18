using Godot;
using System;
using System.Linq;
using System.Collections.Generic;


public partial class RoleData : Resource
{
	public String Tag {get; private set;}


	public void SetRoleTag(string tag) {
		if (Tag == null) Tag = tag;
		else GD.PrintErr($"{Tag} classes Name already set");
	}

}
