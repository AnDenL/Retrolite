using System.Collections;
using UnityEngine;

namespace Creatures
{
    [CreateAssetMenu(fileName = "PositionTransformation", menuName = "CreatureAI/Skills/PositionTransformation")]
    public class PositionTransformation : DirectionSkill
    {
        private static readonly WaitForSeconds _waitForSeconds0_2 = new(0.2f);
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
            owner.Cast();
            owner.StartCoroutine(Anim(direction));
        }
        
        private IEnumerator Anim(Vector2 direction)
        {
            float t = 0f;
            float duration = 0.1f;
            Vector3 startPos = owner.transform.position;
            owner.Animator.SetBool("InDash", true);

            ParticleManager.PlayParticle("Transform", startPos, 4);
            ParticleManager.PlayParticle("GlitchTrail", owner.transform.position, 5);
            ownerCollider.enabled = false;

            while (t < duration)
            {
                t += Time.deltaTime;

                float distance = 30 * Time.deltaTime;
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

            ParticleManager.PlayParticle("GlitchTrail", owner.transform.position, 5);
            ParticleManager.PlayParticle("XYZ", startPos, 4);
            ownerCollider.enabled = true;
            yield return _waitForSeconds0_2;
            owner.Animator.SetBool("InDash", false);
        }
    }
}