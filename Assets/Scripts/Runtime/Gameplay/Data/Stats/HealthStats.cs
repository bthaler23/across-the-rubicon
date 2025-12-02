using UnityEngine;

namespace Game.Stats
{
	public class HealthStats : IStatValue
	{
		private StatValue currentHealth;
		private StatValue maxHealth;

		public HealthStats(int maxHealth)
		{
			this.maxHealth = new StatValue(maxHealth);
			currentHealth = new StatValue(maxHealth);
		}

		public int CurrentHealth =>  Mathf.RoundToInt(currentHealth.GetValue());
		public int MaxHealth =>  Mathf.RoundToInt(maxHealth.GetValue());

		public bool IsAlive => CurrentHealth > 0;

		public void ApplyDamage(int damage)
		{
			var currentHealthValue = this.currentHealth.GetValue();
			currentHealth.ApplyChange(damage < currentHealthValue ? -damage : -currentHealthValue);
		}

		public float GetValue()
		{
			return currentHealth.GetValue();
		}
	}
}