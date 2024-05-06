using Godot;

public partial class TestServiceNode : Node3D
{
   public override void _Process(double delta)
   {
      base._Process(delta);
      GD.Print("TestServiceNode._Process() " + DateTime.UtcNow.ToLocalTime().ToString());
   }
}