using Rls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace TicTacToeQ
{
	public class PlayerAction : IRlsAction
	{
		public readonly int X;

		public readonly int Y;

		public int UniqueId => X * Board.Size + Y;

		public PlayerAction( int x, int y )
		{
			X = x;
			Y = y;
		}
		
		public void Run( int playerIndex, Board board )
		{
			board[X, Y] = playerIndex;
		}

		public override string ToString()
		{
			return $"{X}, {Y}";
		}
	}
}
