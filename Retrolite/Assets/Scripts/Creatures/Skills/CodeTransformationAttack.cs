using System.Collections;
using UnityEngine;

namespace Creatures
{
    [CreateAssetMenu(fileName = "CodeTransformationAttack", menuName = "CreatureAI/Skills/CodeTransformationAttack")]
    public class CodeTransformationAttack : DirectionSkill
    {
        public override SkillType Type => SkillType.Attack;

        public float Speed;
        public float AttackTime;

        private LayerMask creaturesLayer;
        private int attackHash;

        public override void Init(Creature owner)
        {
            base.Init(owner);

            creaturesLayer = LayerMask.GetMask("Creature", "OnlyHits", "Bullet");    
            attackHash = Animator.StringToHash("Attack");
        }

        public override void Activate(Vector2 direction)
        {
            owner.Cast();
            owner.StartCoroutine(Attack(direction));
            owner.UpdateAnimationState();
        }

        private IEnumerator Attack(Vector2 direction)
        {
            owner.Animator.SetTrigger(attackHash);
            owner.Rb.velocity -= 20f * direction;

            ParticleManager.PlayParticle("FastSparcles", owner.transform.position, 3);
            float t = 0;
            owner.CanAct = false;

            while(t < 1)
            {
                t += Time.deltaTime / AttackTime;

                owner.Rb.velocity += Speed * t * Time.deltaTime * (direction + (Vector2)owner.Controller.GetDirectionToTarget());

                yield return null;
            }
            owner.CanAct = true;
            
            Vector2 pos = (Vector2)owner.transform.position + direction * 2;

            ParticleManager.PlayParticle("Glitch", pos, 3);
            var colls = Physics2D.OverlapCircleAll(pos, 0.75f, creaturesLayer);

            foreach(var coll in colls)
            {
                if (coll.gameObject.TryGetComponent(out Creature creature))
                {
                    if (!creature.IsEnemyTo(Owner))
                    {
                        creature.Corruption.Redact();
                        yield break;
                    }
                    if (creature.Break()) 
                    {
                        int res = Random.Range(2,6);
                        owner.Resources.Add(ResourceType.Bits, res);
                        ParticleManager.PlayParticle(ResourceType.Bits ,creature.transform.position ,owner.transform, res);
                    }
                    creature.Rb.AddForce(direction * 50, ForceMode2D.Impulse);
                }
                if (coll.gameObject.TryGetComponent(out CorruptibleBase corruptible))
                {
                    if (corruptible.IsCorrupted)
                    {
                        corruptible.Redact();
                        break;
                    }
                    corruptible.ApplyCorruption(1, owner);
                }
            }
        }
    }
}