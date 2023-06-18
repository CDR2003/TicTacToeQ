using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rls
{
	public interface IRlsInstantRewardGenerator<TObservation, TAction>
		where TObservation : IRlsObservation<TAction>
		where TAction : IRlsAction
	{
		double GetReward( TObservation observation, TAction action );
	}
}
