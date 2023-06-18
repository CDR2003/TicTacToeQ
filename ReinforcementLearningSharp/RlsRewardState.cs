using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rls
{
	public class RlsRewardState
	{
		private Dictionary<int, double> _rewards = new();

		public RlsRewardState()
		{
		}

		public double GetReward( int actionId )
		{
			return _rewards.GetValueOrDefault( actionId );
		}

		public void SetReward( int actionId, double reward )
		{
			_rewards[actionId] = reward;
		}
	}
}
