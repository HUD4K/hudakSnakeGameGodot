using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SnakeGame
{
	public partial class SnakeBody : Sprite2D
	{
		[Signal]
		public delegate void GameOverEventHandler();

		[Signal]
		public delegate void ScoreChangedEventHandler(int newScore);

		private double ElapsedTime = 0;
		private enum Direction
		{
			Left,
			Right,
			Up,
			Down
		};

		private Direction CurrentDirection;
		private List<Rect2> BodySegments;
		private bool IsEating = false;
		private bool HasCrashed = false;
		private Vector2 WindowSize;

		private int Score;

		public override void _Ready()
		{
			CurrentDirection = Direction.Right;
			BodySegments = new List<Rect2>
			{
				new Rect2(0, 0, 40, 40),
				new Rect2(40, 0, 40, 40)
			};
			ZIndex = 1;
			WindowSize = GetViewportRect().Size;
			Score = 0;
		}

		public override void _Draw()
		{
			var color = new Color(255, 255, 0);
			foreach (var segment in BodySegments)
			{
				DrawRect(new Rect2(segment.Position.X + 2, segment.Position.Y + 2, 36, 36), color);
			}
		}

		public bool TryEat(Berry berry)
		{
			if (BodySegments[0].Position == berry.Position)
			{
				IsEating = true;
				Score += 10; // Increase score
				EmitSignal(nameof(ScoreChanged), Score);
				return true;
			}
			return false;
		}

		public bool CheckForCollision()
		{
			return BodySegments.Skip(1).Any(segment => segment.Position == BodySegments[0].Position);
		}

		public override void _Process(double delta)
		{
			if (HasCrashed)
				return;

			ElapsedTime += delta;
			if (ElapsedTime > 0.5)
			{
				var movementVector = CurrentDirection switch
				{
					Direction.Right => new Vector2(40, 0),
					Direction.Left => new Vector2(-40, 0),
					Direction.Up => new Vector2(0, -40),
					_ => new Vector2(0, 40),
				};

				if (BodySegments.Count > 0)
				{
					var newHeadSegment = new Rect2(BodySegments[0].Position, BodySegments[0].Size);
					newHeadSegment.Position += movementVector;

					if (newHeadSegment.Position.X < 0 || newHeadSegment.Position.X >= WindowSize.X || newHeadSegment.Position.Y < 0 || newHeadSegment.Position.Y >= WindowSize.Y)
					{
						HasCrashed = true;
						EmitSignal(nameof(GameOver));
						QueueRedraw();
						return;
					}

					BodySegments.Insert(0, newHeadSegment);
					if (!IsEating)
					{
						BodySegments.RemoveAt(BodySegments.Count - 1);
					}

					if (CheckForCollision())
					{
						HasCrashed = true;
						EmitSignal(nameof(GameOver));
						QueueRedraw();
						return;
					}

					QueueRedraw();
					IsEating = false;
					ElapsedTime = 0;
				}
			}
		}

		public bool IsPositionOccupied(Vector2 position)
		{
			return BodySegments.Any(segment => segment.Position == position);
		}

		public override void _Input(InputEvent inputEvent)
		{
			if (inputEvent.IsAction("ui_left") && CurrentDirection != Direction.Right)
			{
				CurrentDirection = Direction.Left;
				return;
			}
			if (inputEvent.IsAction("ui_right") && CurrentDirection != Direction.Left)
			{
				CurrentDirection = Direction.Right;
				return;
			}
			if (inputEvent.IsAction("ui_up") && CurrentDirection != Direction.Down)
			{
				CurrentDirection = Direction.Up;
				return;
			}
			if (inputEvent.IsAction("ui_down") && CurrentDirection != Direction.Up)
			{
				CurrentDirection = Direction.Down;
				return;
			}
		}
	}
}
