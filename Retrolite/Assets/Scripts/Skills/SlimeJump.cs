using UnityEngine;
using System.Collections;

namespace CreatureAI
{
    [CreateAssetMenu(fileName = "JumpAttack", menuName = "CreatureAI/Skills/JumpAttack")]
    public class JumpAttack : PositionSkill
    {
        public float Speed = 3f;
        public float JumpTime = 0.5f;

        public override SkillType Type => SkillType.Movement;

        public override void Init(Creature owner)
        {
            base.Init(owner);
        }

        public override void Activate(Vector3 position)
        {
            owner.Animator.SetTrigger("Attack");
            owner.Animator.SetFloat("JumpTime", 1 / JumpTime);
            owner.StartCoroutine(JumpCoroutine(position - owner.transform.position));
        }

        private IEnumerator JumpCoroutine(Vector2 direction)
        {
            float t = JumpTime;

            while (t > 0)
            {
                owner.transform.position = Vector2.MoveTowards(owner.transform.position, (Vector2)owner.transform.position + direction, Speed * Time.deltaTime);
                owner.transform.position = new Vector3(owner.transform.position.x, owner.transform.position.y, owner.transform.position.y + t);
                t -= Time.deltaTime;
                yield return null;
            }
        }
    }
}