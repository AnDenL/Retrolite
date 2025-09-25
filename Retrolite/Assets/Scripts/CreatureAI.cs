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

        public virtual void Init(Creature owner) => this.owner = owner;
        public virtual void UpdateAI() { }
    }

    [Serializable]
    public class Skill : ScriptableObject
    {
        protected Creature owner;
        public Creature Owner => owner;

        public int Priority;
        public float MinRange = 0f;
        public float MaxRange = 5f;

        public float cooldownTime;
        protected float lastUsedTime;

        public virtual SkillType Type => SkillType.Empty;
        public virtual void Init(Creature owner) => this.owner = owner;

        public virtual bool CanUse(Creature target) => false;
        public virtual bool CanUse(Vector3 position) => false;
        public virtual bool CanUse() => false;

        public virtual bool Use(Creature target) => false;
        public virtual bool Use(Vector3 position) => false;
        public virtual bool Use() => false;

        public virtual void Activate(Creature target) { }
        public virtual void Activate(Vector3 position) { }
        public virtual void Activate() { }
    }

    public abstract class TargetedSkill : Skill
    {
        public override bool CanUse(Creature target) => target != null && Time.time >= lastUsedTime + cooldownTime &&
            Vector2.Distance(owner.transform.position, target.transform.position) >= MinRange &&
            Vector2.Distance(owner.transform.position, target.transform.position) <= MaxRange;

        public override bool Use(Creature target)
        {
            if (!CanUse(target)) return false;
            lastUsedTime = Time.time;
            Activate(target);
            return true;
        }
    }

    public abstract class PositionSkill : Skill
    {
        public override bool CanUse(Creature target) => CanUse(target.transform.position);

        public override bool Use(Creature target) => Use(target.transform.position);

        public override bool CanUse(Vector3 position) => Time.time >= lastUsedTime + cooldownTime &&
            Vector2.Distance(owner.transform.position, position) >= MinRange &&
            Vector2.Distance(owner.transform.position, position) <= MaxRange;

        public override bool Use(Vector3 position)
        {
            if (!CanUse(position)) return false;
            lastUsedTime = Time.time;
            Activate(position);
            return true;
        }
    }

    public abstract class SelfSkill : Skill
    {
        public override bool CanUse() => Time.time >= lastUsedTime + cooldownTime;
        public override bool Use()
        {
            if (!CanUse()) return false;
            lastUsedTime = Time.time;
            Activate();
            return true;
        }
    }

    [Serializable]
    public abstract class PassiveSkill : ScriptableObject
    {
        protected Creature owner;
        public Creature Owner => owner;
        public virtual SkillType Type => SkillType.Empty;

        public virtual void Subscribe(Creature owner) => this.owner = owner;
    }

    public enum Alignment { Ally, EvilAlly, Neutral, Evil, Enemy, EvilEnemy, FullyFriendly }
    public enum SkillType { Attack, Defence, PowerUp, Utility, Movement, Empty }
}