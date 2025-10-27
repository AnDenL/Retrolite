using UnityEngine;

namespace Creatures
{
    [CreateAssetMenu(fileName = "Interact", menuName = "CreatureAI/Skills/Interact")]
    public class Interact : SelfSkill
    {
        public LayerMask interactMask;
        
        public override SkillType Type => SkillType.Utility;

        private GameObject lastInteractedObject;

        public override void Init(Creature owner)
        {
            base.Init(owner);
            interactMask = LayerMask.GetMask("Interactable");

            if (owner.Controller.IsPlayer)
                owner.OnUpdateAI += OutlineObject;
        }

        public override void Activate()
        {
            var temp = Physics2D.OverlapCircleAll(owner.transform.position, 1.5f, interactMask);

            Collider2D nearestCollider = null;
            float nearestDistance = float.MaxValue;

            foreach (var collider in temp)
            {
                if (collider.CompareTag("Interactable"))
                {
                    float distance = Vector2.Distance(owner.transform.position, collider.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestCollider = collider;
                        nearestDistance = distance;
                    }
                }
            }

            if (nearestCollider != null)
            {
                nearestCollider.GetComponent<Interactable>()?.Interact(owner);
            }
        }

        private void OutlineObject()
        {
            var temp = Physics2D.OverlapCircleAll(owner.transform.position, 1.5f, interactMask);

            Collider2D nearestCollider = null;
            float nearestDistance = float.MaxValue;

            foreach (var collider in temp)
            {
                if (collider.CompareTag("Interactable"))
                {
                    float distance = Vector2.Distance(owner.transform.position, collider.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestCollider = collider;
                        nearestDistance = distance;
                    }
                }
            }

            if (nearestCollider != lastInteractedObject)
            {
                if (lastInteractedObject != null) lastInteractedObject.GetComponent<Interactable>().CancelOutline();
                if (nearestCollider != null) lastInteractedObject = nearestCollider.gameObject;
                else lastInteractedObject = null;
            }

            if (nearestCollider != null)
                nearestCollider.GetComponent<Interactable>().Outline();
        }
    }
}