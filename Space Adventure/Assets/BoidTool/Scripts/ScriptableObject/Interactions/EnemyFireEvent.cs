using UnityEngine;

[CreateAssetMenu(fileName = "EnemyFireEvent", menuName = "Boid/Interactions/EnemyFireEvent", order = 1)]
public class EnemyFireEvent : BaseAction
{
    public override void Invoke(GameObject gameObject)
    {
        IEnemyShooter shooter = gameObject.GetComponent<IEnemyShooter>();
        if (shooter != null)
        {
            shooter.FireProjectile();
        }
    }
}