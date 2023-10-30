using Godot;
using System;

public partial class sparks : GpuParticles2D
{
	[Export] float timeToExpire = 0.75f;
	float timePassed = 0;

    public override void _Ready()
    {
        base._Ready();
   
		Emitting = true;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
   
		timePassed += (float) delta;

		if (timePassed > timeToExpire)
			QueueFree();

		if (timePassed > timeToExpire / 2 && Emitting)
			Emitting = false;
    }
}
