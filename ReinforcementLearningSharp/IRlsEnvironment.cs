namespace Rls
{
	public interface IRlsEnvironment<TObservation, TAction>
		where TObservation : IRlsObservation<TAction>
		where TAction : IRlsAction
	{
		int CurrentPlayerIndex { get; }

		TObservation GetObservation( int playerIndex );

		void MakeStep( int playerIndex, TAction action );

		int CheckWin();
	}
}