using UnityEngine;

namespace CreatureAI
{
    public class UseWeapon : DirectionSkill
    {
        public override SkillType Type => SkillType.Attack;

        [SerializeField] private GameObject weaponManagerPrefab;
        
        private WeaponManager weaponManager;

        public override void Init(Creature owner)
        {
            base.Init(owner);
            weaponManager = Instantiate(weaponManagerPrefab, owner.transform).GetComponent<WeaponManager>();
        }

        public override bool CanUse(Creature target)
        {
            return base.CanUse(target) && weaponManager != null;
        }

        public override void Activate(Creature target)
        {
            
        }
    }
}