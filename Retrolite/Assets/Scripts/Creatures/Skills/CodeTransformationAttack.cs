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

            creaturesLayer = LayerMask.GetMask("Creature", "OnlyHits");    
            attackHash = Animator.StringToHash("Attack");
        }

        public override void Activate(Vector2 direction)
        {
            owner.StartCoroutine(Attack(direction));
            owner.UpdateAnimationState();
        }

        private IEnumerator Attack(Vector2 direction)
        {
            owner.Animator.SetTrigger(attackHash);
            owner.Rb.velocity -= 20f * direction;

            ParticleManager.PlayParticle("FastSparcles", owner.transform.position);
            float s = owner.Speed;
            float t = 0;
            owner.Speed /= 10;

            while(t < 1)
            {
                t += Time.deltaTime / AttackTime;

                owner.Rb.velocity += Speed * t * Time.deltaTime * direction;

                yield return null;
            }
            owner.Speed = s;
            
            Vector2 pos = (Vector2)owner.transform.position + direction * 2;

            ParticleManager.PlayParticle("Glitch", pos);
            var colls = Physics2D.OverlapCircleAll(pos, 0.75f, creaturesLayer);

            foreach(var coll in colls)
            {
                if (coll.gameObject.TryGetComponent(out Creature creature))
                {
                    if (creature.Break()) 
                        owner.Resources.Add(ResourceType.Bits, Random.Range(2,6));
                    creature.Rb.AddForce(direction * 50, ForceMode2D.Impulse);
                }
                if (coll.gameObject.TryGetComponent(out Corruptible corruptible))
                {
                    if (corruptible.IsCorrupted)
                    {
                        corruptible.Redact();
                        break;
                    }
                    corruptible.ApplyCorruption(1);
                }
            }
        }
    }
}