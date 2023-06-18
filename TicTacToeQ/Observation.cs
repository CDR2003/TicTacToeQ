using Rls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToeQ
{
	public class Observation : IRlsObservation<PlayerAction>
	{
		public ObservationCell this[int x, int y]
		{
			get
			{
				return _cells[x, y];
			}
			set
			{
				_cells[x, y] = value;
			}
		}

		public int UniqueId
		{
			get
			{
				var total = 0;
				var currentMultiplier = 1;
				for( int x = 0; x < Board.Size; x++ )
				{
					for( int y = 0; y < Board.Size; y++ )
					{
						var cellId = (int)_cells[x, y] * currentMultiplier;
						currentMultiplier *= Board.Size;
						total += cellId;
					}
				}
				return total;
			}
		}

		private ObservationCell[,] _cells = new ObservationCell[Board.Size, Board.Size];

		public Observation()
		{
			for( int x = 0; x < Board.Size; x++ )
			{
				for( int y = 0; y < Board.Size; y++ )
				{
					_cells[x, y] = ObservationCell.None;
				}
			}
		}

		public List<PlayerAction> GetAvailableActions()
		{
			var actions = new List<PlayerAction>();
			for( int x = 0; x < Board.Size; x++ )
			{
				for( int y = 0; y < Board.Size; y++ )
				{
					if( _cells[x, y] == ObservationCell.None )
					{
						actions.Add( new PlayerAction( x, y ) );
					}
				}
			}
			return actions;
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			for( int y = 0; y < Board.Size; y++ )
			{
				for( int x = 0; x < Board.Size; x++ )
				{
					var text = _cells[x, y] switch
					{
						ObservationCell.None => "-",
						ObservationCell.Self => "S",
						ObservationCell.Opponent => "O",
						_ => throw new Exception()
					};
					sb.Append( text );
				}
				sb.AppendLine();
			}
			return sb.ToString();
		}
	}
}
