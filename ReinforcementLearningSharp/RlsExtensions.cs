using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReinforcementLearningSharp
{
	public static class RlsExtensions
	{
		public static T RandomElement<T>( this IList<T> list )
		{
			if( list.Count == 0 )
			{
				throw new InvalidOperationException();
			}

			var index = Random.Shared.Next( list.Count );
			return list[index];
		}
	}
}
