using Game.Data;
using Game.Events;
using Game.Gameplay;
using Game.Grid;
using GamePlugins.Events;
using GamePlugins.Utils;
using NUnit.Framework;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class ActorController : MonoBehaviour, ITurnActor
	{
		[BoxGroup("Actions")]
		[SerializeField]
		private MoveAction moveAction;
		[BoxGroup("Actions")]
		[SerializeField]
		private AttackAction attackAction;

		[BoxGroup("Actions")]
		[ShowInInspector, ReadOnly]
		private ITurnAction activeAction;
		[BoxGroup("Actions")]
		[ShowInInspector, ReadOnly]
		private List<ITurnAction> actorActions;

		private PlayerInputController inputController;
		[ShowInInspector, ReadOnly]
		private ActorInfo info;
		[ShowInInspector, ReadOnly]
		private TeamInfo teamInfo;
		[ShowInInspector, ReadOnly]
		private Vector2Int currentPosition;
		[ShowInInspector, ReadOnly]
		private bool isTurnActive = false;

		public string ID => gameObject.name;

		public ActorInfo Info { get => info; }
		public Vector2Int CurrentPosition { get => currentPosition; }

		public event Action OnTurnCompleted;

		public void Initialize(ActorInfo info, PlayerInputController inputController, TeamInfo teamInfo)
		{
			this.info = info;
			this.teamInfo = teamInfo;
			this.inputController = inputController;
			isTurnActive = false;
			InitializeActions();
		}

		private void InitializeActions()
		{
			actorActions = new List<ITurnAction>();
			actorActions.Add(moveAction);
			actorActions.Add(attackAction);

			foreach (var action in actorActions)
			{
				action.Initialize(this);
			}
		}

		public void Move(Vector2Int positionIndex)
		{
			currentPosition = positionIndex;
			Vector3 worldPosition = HexGridManager.Instance.GridIndexToWordPosition(positionIndex);
			transform.position = worldPosition;
		}

		public bool HasAnyActions()
		{
			if (actorActions.IsNullOrEmpty()) return false;

			foreach (var action in actorActions)
			{
				if (action.IsAvailable())
				{
					return true;
				}
			}

			return false;
		}

		public void TurnStart()
		{
			isTurnActive = true;
			//TEMP
			SetActiveAction(moveAction);
		}

		public void TurnEnd()
		{
			DisabelCurrentAction();
			isTurnActive = false;
		}

		private void OnActionCompleted()
		{
			OnTurnCompleted?.Invoke();

		}

		private void DisabelCurrentAction()
		{
			if (activeAction != null)
			{
				activeAction.OnActionCompleted -= OnActionCompleted;
				activeAction.DisableAction();
			}
			activeAction = null;
		}

		public Sprite GetActorIcon()
		{
			return info.CharacterSprite;
		}

		public float GetHealthNormalized()
		{
			return 1;//TEMP
		}

		public Color GetTeamColor()
		{
			return teamInfo.TeamColor;
		}

		public IReadOnlyList<ITurnAction> GetActions()
		{
			return actorActions;
		}

		public ITurnAction GetActiveAction()
		{
			return activeAction;
		}

		public void SetActiveAction(ITurnAction action)
		{
			if (!action.IsAvailable()) return;

			DisabelCurrentAction();

			activeAction = action;
			activeAction.ActivateAction();
			activeAction.OnActionCompleted += OnActionCompleted;
			EventBus.Publish<ActiveActorRefreshEvent>(new ActiveActorRefreshEvent(this));
		}

		#region Event Registration/Unregistration
		public void RegisterHexCellClickEvent(Action<Vector2Int> onCellClicked)
		{
			inputController.CellClicked += onCellClicked;
		}

		public void UnRegisterHexCellClickEvent(Action<Vector2Int> onCellClicked)
		{
			inputController.CellClicked -= onCellClicked;
		}
		#endregion
	}
}
