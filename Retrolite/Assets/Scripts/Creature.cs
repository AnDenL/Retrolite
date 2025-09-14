using UnityEngine;
using static Alignment;

[RequireComponent(typeof(HealthBase))]
public class Creature : MonoBehaviour
{
    [Header("Creature")]
    [SerializeField] protected Alignment alignment;
    public Alignment Alignment => alignment;

    [SerializeField] protected Creature target;
    public Creature Target => target;

    [SerializeField] protected float visionRange;

    [HideInInspector] public HealthBase Health;

    protected virtual void Start()
    {
        Health = GetComponent<HealthBase>();
    }

    public bool IsEnemyTo(Alignment other)
    {
        switch (alignment)
        {
            default:
                return false;
            case Ally:
                return other == Enemy || other == EvilEnemy;
            case EvilAlly:
                return !(other == Ally || other == EvilAlly);
            case Neutral:
                return other == EvilEnemy || other == EvilAlly;
            case Evil:
                return true;
            case Enemy:
                return other == Ally || other == EvilAlly;
            case EvilEnemy:
                return !(other == Enemy || other == EvilEnemy);
            case FullyFriendly:
                return false;
        }
    }

    public virtual Creature FindTarget()
    {
        LayerMask obstacleMask = LayerMask.GetMask("Walls");

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, visionRange);

        Creature bestTarget = null;
        float bestDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Creature creature))
            {
                if (creature == this) continue;

                if (!creature.IsEnemyTo(alignment)) continue;

                Vector2 dir = (creature.transform.position - transform.position).normalized;
                float dist = Vector2.Distance(transform.position, creature.transform.position);

                RaycastHit2D block = Physics2D.Raycast(transform.position, dir, dist, obstacleMask);
                if (block.collider != null) continue; 

                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestTarget = creature;
                }
            }
        }

        return bestTarget;
    }
}
public enum Alignment { Ally,  EvilAlly, Neutral, Evil, Enemy, EvilEnemy, FullyFriendly }