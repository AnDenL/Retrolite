using UnityEngine;

namespace Creatures
{
    [CreateAssetMenu(fileName = "AimPassive", menuName = "CreatureAI/Skills/AimPassive")]
    public class AimPassive : PassiveSkill
    {
        public override SkillType Type => SkillType.Attack;

        private WeaponManager weaponManager;

        public override void Init(Creature owner)
        {
            base.Init(owner);
            weaponManager = owner.GetComponentInChildren<WeaponManager>();
            if (weaponManager) owner.OnUpdateAI += Activate;
        }

        public void Activate()
        {
            weaponManager.Rotate(owner.transform.position + Vector3.up * 0.5f - Game.mainCamera.ScreenToWorldPoint(Input.mousePosition));
        }
    }
}