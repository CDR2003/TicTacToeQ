using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rls
{
	public interface IRlsObservation<TAction> where TAction : IRlsAction
	{
		int UniqueId { get; }

		List<TAction> GetAvailableActions();
	}
}
