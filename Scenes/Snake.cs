using Godot;
using System;

namespace SnakeGame
{
	public partial class Snake : Node2D
	{
		private static readonly Random RandomGenerator = new();
		private int SnakeSegmentSize;
		private Vector2I GameGridSize;
		private Vector2 WindowSize;

		// Scenes
		private Berry CurrentBerry;
		private SnakeBody SnakeBody;

		private int Score;
		private Label GameOverLabel;

		public override void _Ready()
		{
			SnakeSegmentSize = 40;
			WindowSize = GetViewportRect().Size;

			GameGridSize = new Vector2I((int)WindowSize.X / SnakeSegmentSize, (int)WindowSize.Y / SnakeSegmentSize);

			SnakeBody = GetNode<SnakeBody>("SnakeBody");
			SnakeBody.Position = new Vector2(0, 0);

			// Initialize score
			Score = 0;

			// Set up GameOverLabel
			GameOverLabel = GetNode<Label>("GameOverLabel");
			GameOverLabel.Visible = false;

			// Remove existing berries at the start of the game
			foreach (var node in GetChildren())
			{
				if (node is Berry existingBerry)
				{
					RemoveChild(existingBerry);
				}
			}

			SpawnNewBerry();
			SnakeBody.GameOver += OnGameOver;
			SnakeBody.ScoreChanged += OnScoreChanged;
		}

		public override void _Process(double delta)
		{
			if (CurrentBerry != null && SnakeBody.TryEat(CurrentBerry))
			{
				RemoveChild(CurrentBerry);
				SpawnNewBerry();
			}
		}

		private void SpawnNewBerry()
		{
			Vector2 newBerryPosition;
			do
			{
				newBerryPosition = new Vector2(RandomGenerator.Next(GameGridSize.X) * SnakeSegmentSize, RandomGenerator.Next(GameGridSize.Y) * SnakeSegmentSize);
			} while (SnakeBody.IsPositionOccupied(newBerryPosition));

			CurrentBerry = new Berry
			{
				Position = newBerryPosition
			};
			AddChild(CurrentBerry);
		}

		private void OnGameOver()
		{
			if (CurrentBerry != null)
			{
				RemoveChild(CurrentBerry);
			}

			// Show GameOverLabel with score
			GameOverLabel.Text = $"Game over, Score: {Score}";
			GameOverLabel.Visible = true;
		}

		private void OnScoreChanged(int newScore)
		{
			Score = newScore;
		}
	}
}
