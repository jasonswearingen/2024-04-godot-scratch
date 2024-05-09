using Godot;
using System;
using GodotEx;

[Tool]
public partial class CharacterController3d : CharacterBody3D
{
   [Export]
   public DebugVisualizer3d debugVis { get; set; }
   public CharacterController3d()
   {
      this._TryHotReload();
   }
   public override void _Ready()
   {
      base._Ready();

      debugVis = new DebugVisualizer3d();
      this._AddChild(debugVis);
      
      //{
      //   var box = new CsgBox3D();
      //   box.Size = new(1, 1, 1);
      //   box.Position = new(3f, 0.5f, 0);
      //   box.RotateY(Mathf.DegToRad(45));
      //   box._EzSetAlbedoTexture("res://assets/textures/grids/Orange/texture_09.png");
      //   box.UseCollision = true;
      //   AddChild(box);
      //}
      

   }
   
   public override void _PhysicsProcess(double delta)
	{

	}
}


public partial class DebugVisualizer3d : Node3D
{


   public Vector3 InfoOffset = Vector3.Up * 2;

   [Export]
   public Node3D target;

   public override void _Ready()
   {
      base._Ready();
      if (target is null)
      {
         target = this.GetParent() as Node3D;
      }

      if (target is not null)
      {
         var bbText = new BillboardedText()
         {
            Text = target.Name,
         };
         bbText.Position = InfoOffset;
         this._AddChild(bbText);
      }

      // Create the BillboardedText

   }
   

   public override void _Process(double delta)
   {
      base._Process(delta);


      //// Create the Label
      //var label = new Label
      //{
      //   Text = "Hello 3D World",
      //   HorizontalAlignment = HorizontalAlignment.Center,
      //   VerticalAlignment = VerticalAlignment.Center,
      //   TextOverrunBehavior = TextServer.OverrunBehavior.NoTrimming,

      //};
      //AddChild(label);
      using (DD3d.NewScopedConfig().SetThickness(0.005f))
      {
         //DD3d.DrawGrid(this.target.GlobalPosition, new(10,1,10), new(0,0,0), new(1,1));
         DD3d.DrawGrid(this.target.GlobalPosition, target.Basis.X*10, target.Basis.Z*10, new(10, 10),Colors.Gray);
         DD3d.DrawArrow(target.GlobalPosition, target.GlobalPosition + target.GlobalForward(), Colors.Orange);
         //DD3d.DrawArrow(target.GlobalPosition, target.GlobalPosition + target.Forward(), Colors.YellowGreen);
      }

   }
}

public partial class BillboardedText : Label3D
{


   public override void _Ready()
   {
      // Set billboard mode to Y-Billboard (always face camera)
      //BillboardMode = BillboardModeEnum.YBillboard;
      this.Billboard = BaseMaterial3D.BillboardModeEnum.Enabled;
   }
}
