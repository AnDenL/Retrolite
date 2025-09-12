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

    [HideInInspector] public HealthBase Health;

    protected virtual void Start()
    {
        Health = GetComponent<HealthBase>();
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
                return other.alignment == Enemy || other.alignment == EvilEnemy;
            case EvilAlly:
                return other.alignment != Enemy || other.alignment != EvilAlly;
            case Neutral:
                return other.alignment == EvilEnemy || other.alignment == EvilAlly;
            case Evil:
                return true;
            case Enemy:
                return other.alignment == Ally || other.alignment == EvilAlly;
            case EvilEnemy:
                return other.alignment != Enemy || other.alignment != EvilEnemy;
            case FullyFriendly:
                return false;
        }
    }

    protected virtual Creature FindTarget()
    {
        Creature creature = null;

        if (alignment == Enemy) return Player.instance.Creature;

        return creature;
    }
}
public enum Alignment { Ally,  EvilAlly, Neutral, Evil, Enemy, EvilEnemy, FullyFriendly }