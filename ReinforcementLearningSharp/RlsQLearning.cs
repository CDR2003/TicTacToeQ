using Rls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReinforcementLearningSharp
{
	public class RlsQLearning<TEnvironment, TObservation, TAction>
		where TEnvironment : IRlsEnvironment<TObservation, TAction>, new()
		where TObservation : IRlsObservation<TAction>
		where TAction : class, IRlsAction
	{
		private class PendingRewardUpdate
		{
			public readonly TObservation Observation;

			public readonly TAction Action;

			public PendingRewardUpdate( TObservation observation, TAction action )
			{
				this.Observation = observation;
				this.Action = action;
			}
		}

		public const double InitialGreedyEpsilon = 0.9;

		public int IterationCount { get; set; } = 500000;

		public double LearningRate { get; set; } = 0.7;

		public double DiscountFactor { get; set; } = 0.9;

		public IRlsInstantRewardGenerator<TObservation, TAction>? InstantRewardGenerator { get; set; }

		private RlsRewardTable _rewardTable = new();

		private double _currentGreedyEpsilon = InitialGreedyEpsilon;

		private Dictionary<int, PendingRewardUpdate> _pendingRewardUpdates = new();

		private bool _training = false;

		public RlsQLearning()
		{
		}

		public void Train()
		{
			_training = true;

			var progress = 0;
			var progressDelta = IterationCount / 100;

			var greedyEpsilonDelta = InitialGreedyEpsilon / this.IterationCount;
			for( int i = 0; i < this.IterationCount; i++ )
			{
				this.TrainOnce();
				_currentGreedyEpsilon -= greedyEpsilonDelta;

				if( i / progressDelta != progress )
				{
					Console.WriteLine( $"Training in progress: {progress}%" );
					progress = i / progressDelta;
				}
			}

			_training = false;
		}

		public void MakeMove( TEnvironment environment )
		{
			var observation = environment.GetObservation( environment.CurrentPlayerIndex );
			var action = this.GenerateAction( observation );
			environment.MakeStep( environment.CurrentPlayerIndex, action );
		}

		private void TrainOnce()
		{
			var environment = new TEnvironment();
			for(; ; )
			{
				var gameOver = this.MakeOneStep( environment );
				if( gameOver )
				{
					break;
				}
			}
		}

		private bool MakeOneStep( TEnvironment environment )
		{
			var gameOver = this.ResolvePendingRewardUpdate( environment );
			if( gameOver )
			{
				return true;
			}

			var playerIndex = environment.CurrentPlayerIndex;
			var observation = environment.GetObservation( playerIndex );
			var action = this.GenerateAction( observation );
			environment.MakeStep( playerIndex, action );

			_pendingRewardUpdates.Add( playerIndex, new PendingRewardUpdate( observation, action ) );

			return false;
		}

		private bool ResolvePendingRewardUpdate( TEnvironment environment )
		{
			if( _pendingRewardUpdates.TryGetValue( environment.CurrentPlayerIndex, out var rewardUpdate ) == false )
			{
				return false;
			}

			var playerIndex = environment.CurrentPlayerIndex;
			var observation = rewardUpdate.Observation;
			var action = rewardUpdate.Action;
			var reward = this.GetReward( observation, action );
			var instantReward = 0.0;
			if( this.InstantRewardGenerator != null )
			{
				instantReward = this.InstantRewardGenerator.GetReward( observation, action );
			}

			var winnerIndex = environment.CheckWin();
			if( winnerIndex != -1 )
			{
				if( winnerIndex == -2 )
				{
					// Draw game
					this.RewardDrawPlayers( environment );
				}
				else
				{
					this.RewardWinner( environment, winnerIndex );
					this.PunishLoser( environment, winnerIndex );
				}
				_pendingRewardUpdates.Clear();
				return true;
			}

			var nextObservation = environment.GetObservation( playerIndex );
			var nextAvailableActions = nextObservation.GetAvailableActions();
			var nextBestAction = this.GetBestAction( nextObservation, nextAvailableActions );
			var nextReward = this.GetReward( nextObservation, nextBestAction );
			this.TweakReward( observation, action, instantReward, nextReward );

			_pendingRewardUpdates.Remove( environment.CurrentPlayerIndex );

			return false;
		}

		private void RewardWinner( TEnvironment environment, int winnerIndex )
		{
			var winnerRewardUpdate = _pendingRewardUpdates.GetValueOrDefault( winnerIndex )!;
			this.TweakReward( winnerRewardUpdate.Observation, winnerRewardUpdate.Action, 10000, 0 );
		}

		private void PunishLoser( TEnvironment environment, int winnerIndex )
		{
			foreach( var pair in _pendingRewardUpdates )
			{
				if( pair.Key == winnerIndex )
				{
					continue;
				}

				var observation = pair.Value.Observation;
				var action = pair.Value.Action;
				this.TweakReward( observation, action, -10000, 0 );
			}
		}

		private void RewardDrawPlayers( TEnvironment environment )
		{
			foreach( var pair in _pendingRewardUpdates )
			{
				var observation = pair.Value.Observation;
				var action = pair.Value.Action;
				this.TweakReward( observation, action, 0, 0 );
			}
		}

		private void TweakReward( TObservation observation, TAction action, double instantReward, double nextReward )
		{
			var reward = this.GetReward( observation, action );
			var rewardDelta = instantReward + this.DiscountFactor * nextReward - reward;
			var updatedReward = reward + this.LearningRate * rewardDelta;
			this.SetReward( observation, action, updatedReward );
		}

		private TAction GenerateAction( TObservation observation )
		{
			var availableActions = observation.GetAvailableActions();
			if( _training && this.ShouldExplore() )
			{
				return availableActions.RandomElement();
			}
			else
			{
				return this.GetBestAction( observation, availableActions );
			}
		}

		private TAction GetBestAction( TObservation observation, List<TAction> actions )
		{
			var maxReward = 0.0;
			TAction? maxRewardAction = null;
			foreach( var action in actions )
			{
				var reward = this.GetReward( observation, action );
				if( reward >= maxReward )
				{
					maxReward = reward;
					maxRewardAction = action;
				}
			}

			if( maxRewardAction == null )
			{
				return actions.RandomElement();
			}

			return maxRewardAction;
		}

		private double GetReward( TObservation observation, TAction action )
		{
			return _rewardTable.GetReward( observation.UniqueId, action.UniqueId );
		}

		private void SetReward( TObservation observation, TAction action, double reward )
		{
			_rewardTable.SetReward( observation.UniqueId, action.UniqueId, reward );
		}

		private bool ShouldExplore()
		{
			return Random.Shared.NextDouble() < _currentGreedyEpsilon;
		}
	}
}
