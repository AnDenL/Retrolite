using System;
using UnityEngine;

namespace Creatures
{
    [Serializable]
    public class AIController : ScriptableObject
    {
        protected Creature owner;
        public Creature Owner => owner;

        protected Creature target;
        public Creature Target => target;

        public Alignment Alignment;
        protected DirectionSkill Movement => owner.BaseMovement;

        public virtual bool IsPlayer => false;

        public virtual void Init(Creature owner) => this.owner = owner;
        public virtual void UpdateAI() { }

        public virtual Vector3 GetDirectionToTarget()
        {
            if (target == null) return Vector3.zero;
            return (target.transform.position - owner.transform.position).normalized;
        }

        public virtual Vector3 GetTargetPosition()
        {
            if (target == null) return Vector3.zero;
            return target.transform.position;
        }
    }
}