using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Rls
{
	public class RlsRewardTable
	{
		private readonly Dictionary<int, RlsRewardState> _states = new();

		public RlsRewardTable()
		{
		}

		public RlsRewardState? GetRewardState( int stateId )
		{
			return _states.GetValueOrDefault( stateId );
		}

		public double GetReward( int stateId, int actionId )
		{
			var rewardState = this.GetRewardState( stateId );
			if( rewardState == null )
			{
				return 0;
			}
			return rewardState.GetReward( actionId );
		}

		public void SetReward( int stateId, int actionId, double reward )
		{
			if( _states.TryGetValue( stateId, out var state ) == false )
			{
				state = new RlsRewardState();
				_states.Add( stateId, state );
			}
			state.SetReward( actionId, reward );
		}
	}
}
