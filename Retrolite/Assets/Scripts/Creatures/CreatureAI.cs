using System;
using UnityEngine;

namespace CreatureAI
{
    [Serializable]
    public class AIController : ScriptableObject
    {
        protected Creature owner;
        public Creature Owner => owner;

        protected Creature target;
        public Creature Target => target;

        [SerializeField] protected Alignment alignment;
        public Alignment Alignment => alignment;

        protected DirectionSkill movement => owner.BaseMovement;

        public virtual void Init(Creature owner) => this.owner = owner;
        public virtual void UpdateAI() { }

        public virtual Vector3 GetDirectionToTarget()
        {
            if (target == null) return Vector3.zero;
            return (target.transform.position - owner.transform.position).normalized;
        }
    }

    public enum Alignment { Ally, EvilAlly, Neutral, Evil, Enemy, EvilEnemy, FullyFriendly }
}