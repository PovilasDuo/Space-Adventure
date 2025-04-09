using UnityEngine;

[CreateAssetMenu(fileName = "EnemyFireEvent", menuName = "Boid/Interactions/EnemyFireEvent", order = 1)]
public class EnemyFireEvent : BaseAction
{
    /// <summary>
    /// This action is triggered when the enemy fires a projectile.
    /// </summary>
    /// <param name="gameObject">The GameObject for which to get the component required</param>
    public override void Invoke(GameObject gameObject)
    {
        IEnemyShooter shooter = gameObject.GetComponent<IEnemyShooter>();
        if (shooter != null)
        {
            shooter.FireProjectile();
        }
    }
}