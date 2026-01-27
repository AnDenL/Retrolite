using UnityEngine;
using CalculatingSystem;
using System.Collections;

namespace Creatures
{
    [CreateAssetMenu(fileName = "ShootBullet", menuName = "CreatureAI/Skills/ShootBullet")]
    public class ShootBullet : EnemyTargetedSkill
    {
        private static WaitForSeconds _waitForSeconds0_5 = new(0.5f);
        public BulletPool Pool;
        public GameObject BulletPrefab;
        public BulletData BulletData;
        public Context Context;

        public float Knockback;

        public override SkillType Type => SkillType.Attack;

        private Transform Clip;

        public override void Init(Creature owner)
        {
            base.Init(owner);

            var Context = new Context
            {
                Owner = owner
            };

            Clip = owner.transform.Find("Clip");
            if (Clip == null)
            {
                GameObject clip = new("Clip");
                clip.transform.parent = owner.transform;
                clip.transform.position = owner.transform.position;
                Clip = clip.transform;
            }

            Pool = new BulletPool(BulletPrefab, Clip, BulletData, Context);
        }

        public void OnDestroy()
        {
            Pool?.Clear();
        }

        public override void Activate(Creature target)
        {
            if (target == null) return;
            owner.Cast();
            owner.Cast(Shoot(target));
        }

        private IEnumerator Shoot(Creature target)
        {
            if (!Owner.Controller.IsPlayer) ParticleManager.PlayParticle("Agr", Clip.position);

            yield return _waitForSeconds0_5;

            lastUsedTime = Time.time + cooldownTime;
            Vector2 direction = target.transform.position - owner.transform.position;

            RaycastHit2D hit = Physics2D.Raycast(owner.transform.position, direction, direction.magnitude, LayerMask.GetMask("Walls"));
            if (hit.collider == null)
            {
                Vector2 dir = owner.transform.position - target.transform.position;
                owner.Rb.AddForce(Knockback * dir, ForceMode2D.Impulse);
                Clip.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + (owner.transform.localScale.x == 1 ? 0 : 180));
                ParticleManager.PlayParticle("Impact", Clip.position);
                Pool.Get().Fire(0, 1);
            }
        }
    }
}