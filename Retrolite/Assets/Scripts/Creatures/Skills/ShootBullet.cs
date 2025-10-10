using UnityEngine;
using CalculatingSystem;

namespace CreatureAI
{
    [CreateAssetMenu(fileName = "ShootBullet", menuName = "CreatureAI/Skills/ShootBullet")]
    public class ShootBullet : TargetedSkill
    {
        public BulletPool Pool;
        public GameObject BulletPrefab;
        public BulletData BulletData;
        public FormulaContext Context;

        public float Knockback;

        public override SkillType Type => SkillType.Attack;

        private Transform Clip;

        public override void Init(Creature owner)
        {
            base.Init(owner);

            var Context = new FormulaContext
            {
                Owner = owner
            };

            Clip = owner.transform.Find("Clip");

            Pool = new BulletPool(BulletPrefab, Clip, owner, BulletData, Context);
        }

        public void OnDestroy()
        {
            Pool?.Clear();
        }

        public override void Activate(Creature target)
        {
            if (target == null) return;

            lastUsedTime = Time.time + cooldownTime;

            Vector2 direction = target.transform.position - owner.transform.position;

            if (Random.Range(0, direction.magnitude) > 5) return;

            RaycastHit2D hit = Physics2D.Raycast(owner.transform.position, direction, direction.magnitude, LayerMask.GetMask("Walls"));
            if (hit.collider == null)
            {
                owner.StartKnockback(Knockback, owner.transform.position - target.transform.position);
                Clip.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + (owner.transform.localScale.x == 1 ? 0 : 180));
                ParticleManager.PlayParticle(5, Clip.position);
                Pool.Get().Fire(0);
            }
        }
    }
}