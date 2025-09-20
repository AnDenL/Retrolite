using UnityEngine;
using static Alignment;

[RequireComponent(typeof(HealthBase))]
[RequireComponent(typeof(Corruptible))]
public class Creature : MonoBehaviour
{
    [Header("Creature")]
    [SerializeField] protected Alignment alignment;
    public Alignment Alignment => alignment;

    [SerializeField] protected Creature target;
    public Creature Target => target;

    [SerializeField] protected float visionRange;

    [HideInInspector] public HealthBase HealthComponent;
    [HideInInspector] public Corruptible Corruption;

    protected virtual void Awake()
    {
        HealthComponent = GetComponent<HealthBase>();
        Corruption = GetComponent<Corruptible>();
    }

    public bool IsEnemyTo(Creature other)
    {
        if (other == null) return false;
        if (other == this) return false;

        switch (alignment)
        {
            default:
                return false;
            case Ally:
                return other.Alignment == Enemy || other.Alignment == EvilEnemy;
            case EvilAlly:
                return !(other.Alignment == Ally || other.Alignment == EvilAlly);
            case Neutral:
                return other.Alignment == EvilEnemy || other.Alignment == EvilAlly || other.Alignment == Evil;
            case Evil:
                return true;
            case Enemy:
                return other.Alignment == Ally || other.Alignment == EvilAlly;
            case EvilEnemy:
                return !(other.Alignment == Enemy || other.Alignment == EvilEnemy);
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

                if (!creature.IsEnemyTo(this)) continue;

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
public enum Alignment { Ally, EvilAlly, Neutral, Evil, Enemy, EvilEnemy, FullyFriendly }
