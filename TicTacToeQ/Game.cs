using Rls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToeQ
{
	public class Game : IRlsEnvironment<Observation, PlayerAction>
	{
		public readonly int PlayerCount = 2;

		public readonly Board Board = new();

		public int CurrentPlayerIndex { get; private set; }

		public Game()
		{
			this.CurrentPlayerIndex = 0;
		}

		public Observation GetObservation( int playerIndex )
		{
			var observation = new Observation();
			for( int x = 0; x < Board.Size; x++ )
			{
				for( int y = 0; y < Board.Size; y++ )
				{
					var cell = this.Board[x, y];
					if( cell == -1 )
					{
						observation[x, y] = ObservationCell.None;
					}
					else if( cell == playerIndex )
					{
						observation[x, y] = ObservationCell.Self;
					}
					else
					{
						observation[x, y] = ObservationCell.Opponent;
					}
				}
			}
			return observation;
		}

		public void MakeStep( int playerIndex, PlayerAction action )
		{
			Debug.Assert( playerIndex == this.CurrentPlayerIndex );

			action.Run( playerIndex, this.Board );

			this.CurrentPlayerIndex = ( playerIndex + 1 ) % PlayerCount;
		}

		public int CheckWin()
		{
			return this.Board.CheckWin();
		}
	}
}
