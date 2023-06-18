using ReinforcementLearningSharp;

namespace TicTacToeQ
{
	internal class Program
	{
		private static bool MakeHumanMove( Game game )
		{
			var input = Console.ReadLine()!.Trim();
			var words = input.Split( ' ' );
			var x = int.Parse( words[0] );
			var y = int.Parse( words[1] );
			var action = new PlayerAction( x - 1, y - 1 );
			try
			{
				game.MakeStep( game.CurrentPlayerIndex, action );
			}
			catch( Exception )
			{
				Console.WriteLine( "Invalid move!" );
				return false;
			}
			return true;
		}

		private static void RunHumanVsHuman()
		{
			var game = new Game();
			for(; ; )
			{
				Console.WriteLine( "Player #{0} turn", game.CurrentPlayerIndex );

				Console.Write( game.Board );

				var isValidMove = MakeHumanMove( game );
				if( isValidMove == false )
				{
					continue;
				}

				Console.WriteLine();

				var winnerIndex = game.CheckWin();
				if( winnerIndex != Board.EmptyCell )
				{
					Console.WriteLine( $"Player #{winnerIndex} won!" );
					Console.Write( game.Board );
					break;
				}
			}
		}

		private static void RunCpuVsHuman( RlsQLearning<Game, Observation, PlayerAction> learning )
		{
			var game = new Game();
			for(; ; )
			{
				Console.WriteLine( "Player #{0} turn", game.CurrentPlayerIndex );

				Console.Write( game.Board );
				
				if( game.CurrentPlayerIndex == 0 )
				{
					learning.MakeMove( game );
				}
				else
				{
					var isValidMove = MakeHumanMove( game );
					if( isValidMove == false )
					{
						continue;
					}
				}

				Console.WriteLine();

				var winnerIndex = game.CheckWin();
				if( winnerIndex != -1 )
				{
					if( winnerIndex == -2 )
					{
						Console.WriteLine( "Draw!" );
					}
					else
					{
						Console.WriteLine( $"Player #{winnerIndex} won!" );
					}
					
					Console.Write( game.Board );
					break;
				}
			}
		}

		static void Main( string[] args )
		{
			var learning = new RlsQLearning<Game, Observation, PlayerAction>();
			learning.Train();
			
			for(; ; )
			{
				Console.Clear();
				RunCpuVsHuman( learning );
				Console.ReadLine();
			}
		}
	}
}