using Godot;
using System;

namespace SnakeGame
{
	public partial class Berry : Sprite2D
	{
		public override void _Draw()
		{
			DrawCircle(new Vector2(20, 20), 15, new Color(1, 0, 0));
		}
	}
}
