using UnityEngine;
using System;

namespace Creatures
{
    [Serializable]
    public class Skill : ScriptableObject
    {
        public string Name;

        protected Creature owner;
        public Creature Owner => owner;

        public int Priority;

        public float cooldownTime;
        protected float lastUsedTime;

        public virtual SkillType Type => SkillType.Empty;
        public virtual void Init(Creature owner)
        {
            this.owner = owner;
            lastUsedTime = Time.time + cooldownTime;
        }

        public virtual bool CanUse(Creature target) => false;
        public virtual bool CanUse(Vector2 position) => false;
        public virtual bool CanUse() => false;

        public virtual bool Use(Creature target) => false;
        public virtual bool Use(Vector2 position) => false;
        public virtual bool Use() => false;

        public virtual void Activate(Creature target) { }
        public virtual void Activate(Vector2 position) { }
        public virtual void Activate() { }
    }

    public abstract class TargetedSkill : Skill
    {
        public float MinRange = 0f;
        public float MaxRange = 5f;

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

    public abstract class EnemyTargetedSkill : TargetedSkill
    {
        public override bool CanUse(Creature target) => target != null && Time.time >= lastUsedTime + cooldownTime &&
            Vector2.Distance(owner.transform.position, target.transform.position) >= MinRange &&
            Vector2.Distance(owner.transform.position, target.transform.position) <= MaxRange &&
            owner.IsEnemyTo(target);
    }

    public abstract class AllyTargetedSkill : TargetedSkill
    {
        public override bool CanUse(Creature target) => target != null && Time.time >= lastUsedTime + cooldownTime &&
            Vector2.Distance(owner.transform.position, target.transform.position) >= MinRange &&
            Vector2.Distance(owner.transform.position, target.transform.position) <= MaxRange &&
            !owner.IsEnemyTo(target);

    }

    public abstract class DirectionSkill : Skill
    {
        public override bool CanUse(Creature target) => Time.time >= lastUsedTime + cooldownTime && target != null;
        public override bool CanUse(Vector2 position) => Time.time >= lastUsedTime + cooldownTime && position != Vector2.zero;

        public override bool Use(Creature target) => Use(target.transform.position);
        public override bool Use(Vector2 position)
        {
            if (!CanUse(position)) return false;
            lastUsedTime = Time.time;
            Activate(position.normalized);
            return true;
        }
    }

    public abstract class PositionSkill : Skill
    {
        public float MinRange = 0f;
        public float MaxRange = 5f;

        public override bool CanUse(Creature target) => CanUse(target.transform.position);

        public override bool Use(Creature target) => Use(target.transform.position);

        public override bool CanUse(Vector2 position) => Time.time >= lastUsedTime + cooldownTime &&
            Vector2.Distance(owner.transform.position, position) >= MinRange &&
            Vector2.Distance(owner.transform.position, position) <= MaxRange;

        public override bool Use(Vector2 position)
        {
            if (!CanUse(position)) return false;
            lastUsedTime = Time.time;
            Activate(position);
            return true;
        }
    }

    public abstract class SelfSkill : Skill
    {
        public override bool CanUse(Creature target) => CanUse();
        public override bool CanUse(Vector2 position) => CanUse();
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

        public virtual void Init(Creature owner) => this.owner = owner;
    }

    public enum SkillType { Attack, Defense, PowerUp, Utility, Movement, Empty }
}