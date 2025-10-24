using System.Collections;
using UnityEngine;

namespace CreatureAI
{
    [CreateAssetMenu(fileName = "PositionTransformation", menuName = "CreatureAI/Skills/PositionTransformation")]
    public class PositionTransformation : DirectionSkill
    {
        public float strength = 0.1f;
        public override SkillType Type => SkillType.Movement;
        public override void Init(Creature owner)
        {
            base.Init(owner);
        }
        public override bool CanUse(Vector2 direction)
        {
            return base.CanUse(direction);
        }

        public override void Activate(Vector2 direction)
        {
            owner.StartCoroutine(Anim(direction));
        }
        
        private IEnumerator Anim(Vector2 direction)
        {
            float t = 1;

            ParticleManager.PlayParticle(7, owner.transform.position);

            while (t > 0f)
            {
                t -= Time.deltaTime * 7;
                float dt = 1 - t * t;
                owner.transform.position += owner.Speed * dt * Time.deltaTime * (Vector3)direction;
                yield return null;
            }

            ParticleManager.PlayParticle(7, owner.transform.position);
        }
    }
}