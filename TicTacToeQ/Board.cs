using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToeQ
{
	public class Board
	{
		public const int Size = 3;

		public const int EmptyCell = -1;

		public int this[int x, int y]
		{
			get
			{
				return _cells[x, y];
			}
			set
			{
				if( this.IsOccupied( x, y ) )
				{
					throw new InvalidOperationException();
				}
				
				_cells[x, y] = value;
			}
		}

		public bool IsFullyOccupied
		{
			get
			{
				for( int x = 0; x < Size; x++ )
				{
					for( int y = 0; y < Size; y++ )
					{
						if( _cells[x, y] == EmptyCell )
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		private readonly int[,] _cells = new int[Size, Size];

		public Board()
		{
			for( int x = 0; x < Size; x++ )
			{
				for( int y = 0; y < Size; y++ )
				{
					_cells[x, y] = EmptyCell;
				}
			}
		}

		public bool IsOccupied( int x, int y )
		{
			return _cells[x, y] != EmptyCell;
		}

		public int CheckWin()
		{
			int playerIndex = CheckWinHorizontal();
			if( playerIndex != EmptyCell )
			{
				return playerIndex;
			}

			playerIndex = this.CheckWinVertical();
			if( playerIndex != EmptyCell )
			{
				return playerIndex;
			}

			playerIndex = this.CheckWinDiagonal();
			if( playerIndex != EmptyCell )
			{
				return playerIndex;
			}

			if( this.IsFullyOccupied )
			{
				return -2;
			}

			return -1;
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			for( int y = 0; y < Size; y++ )
			{
				for( int x = 0; x < Size; x++ )
				{
					var text = _cells[x, y] switch
					{
						EmptyCell => "-",
						0 => "O",
						1 => "X",
						_ => throw new Exception()
					};
					sb.Append( text );
				}
				sb.AppendLine();
			}
			return sb.ToString();
		}

		private int CheckWinDiagonal()
		{
			var first = _cells[0, 0];
			for( int i = 0; i < Size; i++ )
			{
				if( first == EmptyCell )
				{
					continue;
				}
				if( _cells[i, i] != first )
				{
					break;
				}
				if( i == Size - 1 )
				{
					return first;
				}
			}

			first = _cells[Size - 1, 0];
			for( int i = 0; i < Size; i++ )
			{
				if( first == EmptyCell )
				{
					continue;
				}
				if( _cells[Size - 1 - i, i] != first )
				{
					break;
				}
				if( i == Size - 1 )
				{
					return first;
				}
			}
			
			return EmptyCell;
		}

		private int CheckWinVertical()
		{
			for( int x = 0; x < Size; x++ )
			{
				var first = _cells[x, 0];
				if( first == EmptyCell )
				{
					continue;
				}
				for( int y = 0; y < Size; y++ )
				{
					if( _cells[x, y] != first )
					{
						break;
					}
					if( y == Size - 1 )
					{
						return first;
					}
				}
			}
			return EmptyCell;
		}

		private int CheckWinHorizontal()
		{
			for( int y = 0; y < Size; y++ )
			{
				var first = _cells[0, y];
				if( first == EmptyCell )
				{
					continue;
				}
				for( int x = 0; x < Size; x++ )
				{
					if( _cells[x, y] != first )
					{
						break;
					}
					if( x == Size - 1 )
					{
						return first;
					}
				}
			}
			return EmptyCell;
		}
	}
}
