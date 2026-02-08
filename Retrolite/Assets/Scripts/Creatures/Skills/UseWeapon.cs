using UnityEngine;

namespace Creatures
{
    [CreateAssetMenu(fileName = "UseWeapon", menuName = "CreatureAI/Skills/UseWeapon")]
    public class UseWeapon : SelfSkill
    {
        public override SkillType Type => SkillType.Attack;

        [SerializeField] private GameObject weaponManagerPrefab;
        
        private WeaponManager weaponManager;

        public override void Init(Creature owner)
        {
            base.Init(owner);
            weaponManager = Instantiate(weaponManagerPrefab, owner.transform).GetComponent<WeaponManager>();
            weaponManager.Init(owner);
        }

        public override bool CanUse()
        {
            return weaponManager.CanShoot();
        }

        public override void Activate()
        {
            weaponManager.Shoot();
        }
    }
}