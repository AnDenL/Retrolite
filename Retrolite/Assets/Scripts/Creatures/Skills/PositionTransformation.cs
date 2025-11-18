using System.Collections;
using UnityEngine;

namespace Creatures
{
    [CreateAssetMenu(fileName = "PositionTransformation", menuName = "CreatureAI/Skills/PositionTransformation")]
    public class PositionTransformation : DirectionSkill
    {
        public float strength = 0.1f;
        public override SkillType Type => SkillType.Movement;

        private LayerMask obstacleLayer;
        private Collider2D ownerCollider;

        public override void Init(Creature owner)
        {
            base.Init(owner);
            obstacleLayer = LayerMask.GetMask("Walls");
            ownerCollider = owner.GetComponent<Collider2D>();
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
            float t = 0f;
            float duration = 0.06f;
            Vector3 startPos = owner.transform.position;

            ParticleManager.PlayParticle("XYZ", startPos);
            ParticleManager.PlayParticle("Transform", startPos);
            ParticleManager.PlayParticle(8, owner.transform.position);
            ownerCollider.enabled = false;

            while (t < duration)
            {
                t += Time.deltaTime;

                float distance = 60 * Time.deltaTime;
                RaycastHit2D hit = Physics2D.Raycast(
                    owner.transform.position,
                    direction.normalized,
                    distance,
                    obstacleLayer
                );

                if (hit.collider != null)
                {
                    owner.transform.position = hit.point;
                    break;
                }
                else
                {
                    owner.transform.position += (Vector3)(direction.normalized * distance);
                }

                yield return null;
            }

            ParticleManager.PlayParticle(8, owner.transform.position);
            ParticleManager.PlayParticle("XYZ", startPos);
            ownerCollider.enabled = true;
        }
    }
}