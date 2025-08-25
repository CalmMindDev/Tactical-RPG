using Godot;
using System;
using System.Collections.Generic;


public partial class MapObjectCache {
    public List<Vector2I> General = new List<Vector2I>();
    public List<Vector2I> PlayerTeam = new List<Vector2I>();
    public List<Vector2I> EnemyTeam1 = new List<Vector2I>();
    public List<Vector2I> FriendlyTeam1 = new List<Vector2I>();


    public event EventHandler MapObjectsAsked;

    public void GetMapObjects() {
        General.Clear();
        PlayerTeam.Clear();
        EnemyTeam1.Clear();
        FriendlyTeam1.Clear();
        MapObjectsAsked.Invoke(this, EventArgs.Empty);
    }
}
interface IMapObject
{   
    Vector2I GridPosition {get;}
    Teams Team {get;}
    void SubscribeToCache();
    
    void OnMapObjectsAsked(object grid, EventArgs e) {
        var typedGrid = grid as Grid;
        typedGrid.MapObjectCache.General.Add(GridPosition);
    }
}
