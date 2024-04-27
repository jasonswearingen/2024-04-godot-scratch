using Godot;
using System;

public partial class Main : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		AddChild(Player);

	}

	public Player Player { get; set; }=new Player();

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	
}

public partial class Player: Node3D
{
   public override void _Ready()
   {
      base._Ready();
		GD.Print("Player Ready");
   }

   public override void _Process(double delta)
   {
      base._Process(delta);
		if (Input.IsActionPressed("ui_accept"))
      {
         GD.Print("gotcha");
      }

   }

}


public partial class DIHost : Godot.Node
{
   public override void _Ready()
   {
      base._Ready();
      GD.Print("DIHost Ready");



   }
}