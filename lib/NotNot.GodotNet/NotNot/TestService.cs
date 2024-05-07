using Godot;

namespace NotNot;

public class TestService : ISingletonService
{
    public void Test()
    {
        _GD.Print("TestService.Test() 21 nice?", Colors.White);
    }
}