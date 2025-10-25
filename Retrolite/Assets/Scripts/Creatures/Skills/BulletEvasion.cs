using UnityEngine;

namespace CreatureAI
{
    [CreateAssetMenu(fileName = "BulletEvasion", menuName = "CreatureAI/Skills/BulletEvasion")]
    public class BulletEvasion : PassiveSkill
    {
        public float Speed = 5f;
        public float DetectionRadius = 2f;
        public float SpeedThreshold = 0.25f;
        public GameObject TriggerAreaPrefab;

        public override SkillType Type => SkillType.Movement;

        public override void Init(Creature owner)
        {
            base.Init(owner);

            GameObject triggerArea = Instantiate(TriggerAreaPrefab, owner.transform);

            CircleCollider2D collider = triggerArea.GetComponent<CircleCollider2D>();
            collider.radius = DetectionRadius;

            triggerArea.GetComponent<TriggerEvent>().OnTriggered += (other) =>
            {
                if (owner.HealthComponent.IsDead || owner.Corruption.isCorrupted) return;
                if (other.CompareTag("Bullet"))
                {
                    if (other.TryGetComponent(out BulletBase bullet))
                        if (!bullet.Context.Owner.IsEnemyTo(owner)) return;
                    if (owner.Rb.velocity.magnitude < SpeedThreshold) Evade(other.transform.up);
                }
            };
        }

        private void Evade(Vector2 position)
        {
            Vector2 leftDodge = new Vector2(-position.y + Random.Range(-0.25f, 0.25f), position.x + Random.Range(-0.25f, 0.25f)).normalized;
            Vector2 rightDodge = new Vector2(position.y + Random.Range(-0.25f, 0.25f), -position.x + Random.Range(-0.25f, 0.25f)).normalized;

            float leftDist = CheckFreeDistance(leftDodge);
            float rightDist = CheckFreeDistance(rightDodge);

            Vector2 dodgeDirection = (leftDist == rightDist) ?
                (Random.Range(0, 2) == 0 ? leftDodge : rightDodge) :
                (leftDist > rightDist ? leftDodge : rightDodge);

            owner.StartKnockback(Speed, dodgeDirection);
        }

        private float CheckFreeDistance(Vector2 dir)
        {
            RaycastHit2D hit = Physics2D.Raycast(owner.transform.position, dir, 3f, LayerMask.GetMask("Walls"));
            return hit.collider == null ? 3f : hit.distance;
        }
    }
}