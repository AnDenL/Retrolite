using System.Collections.Generic;
using UnityEngine;

namespace CreatureAI
{
    [CreateAssetMenu(fileName = "PlayerController", menuName = "CreatureAI/Controllers/PlayerController")]
    public class PlayerController : AIController
    {
        public Dictionary<KeyCode, ISkillSlot> SkillSlots = new();

        public static Player Player => (Player)instance.Owner;

        public static PlayerController instance;
        public static bool CanInteract = true;

        public override void Init(Creature owner)
        {
            base.Init(owner);

            if (instance != null)
            {
                Debug.LogWarning("Multiple PlayerController instances detected!");
                return;
            }
            instance = this;

            target = MouseTarget.instance;
            owner.OnNewSkill += NewSlot;
        }

        public override void UpdateAI()
        {
            if (!CanInteract)
                return;
            
            Vector2 moveDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

            foreach (var slot in SkillSlots)
                if (slot.Value.OnKeyDown ? Input.GetKeyDown(slot.Key) : Input.GetKey(slot.Key)) slot.Value.Use();

            movement.Use(moveDir);
        }

        private void NewSlot(Skill skill)
        {
            KeyCode key = GetKeyCodeByType(skill.Type);
            ISkillSlot skillSlot = null;

            if (skill is SelfSkill selfSkill)
                skillSlot = new SelfSkillSlot(selfSkill);
            else if (skill is PositionSkill positionSkill)
                skillSlot = new PositionSkillSlot(positionSkill);
            else if (skill is DirectionSkill directionSkill)
                skillSlot = new DirectionSkillSlot(directionSkill);
            else if (skill is TargetedSkill targetedSkill)
                skillSlot = new TargetedSkillSlot(targetedSkill);

            SkillSlots.Add(key, skillSlot);
        }

        //Default key bindings for different skill types, can be customized by player later
        private KeyCode GetKeyCodeByType(SkillType type)
        {
            KeyCode result = KeyCode.None;

            KeyCode[] keys;

            switch (type)
            {
                case SkillType.Attack:
                    keys = new KeyCode[] { KeyCode.Mouse0, KeyCode.Mouse1, KeyCode.Q, KeyCode.E, KeyCode.F };
                    break;
                case SkillType.Movement:
                    keys = new KeyCode[] { KeyCode.Space, KeyCode.LeftShift, KeyCode.LeftControl };
                    break;
                case SkillType.Defense:
                    keys = new KeyCode[] { KeyCode.Z, KeyCode.X, KeyCode.C };
                    break;
                case SkillType.PowerUp:
                    keys = new KeyCode[] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3 };
                    break;
                case SkillType.Utility:
                    keys = new KeyCode[] { KeyCode.V, KeyCode.B, KeyCode.N };
                    break;
                default:
                    keys = new KeyCode[0];
                    break;
            }

            foreach (var key in keys)
            {
                if (!SkillSlots.ContainsKey(key))
                    return key;
            }

            return result;
        }
    }
    
    public class SelfSkillSlot  : ISkillSlot
    {
        public SelfSkill Skill;
        public bool OnKeyDown { get; set; }

        public SelfSkillSlot (SelfSkill skill)
        {
            Skill = skill;
        }

        public void Use() => Skill.Use();
    }

    public class PositionSkillSlot : ISkillSlot
    {
        public PositionSkill Skill;
        public bool OnKeyDown { get; set; }

        public PositionSkillSlot(PositionSkill skill)
        {
            Skill = skill;
        }

        public void Use() => Skill.Use(Game.mainCamera.ScreenToWorldPoint(Input.mousePosition));
    }

    public class DirectionSkillSlot : ISkillSlot
    {
        public DirectionSkill Skill;
        public bool OnKeyDown { get; set; }

        public bool UseWalkInput = true;

        public DirectionSkillSlot(DirectionSkill skill)
        {
            Skill = skill;
        }

        public void Use() => Skill.Use(UseWalkInput
            ? new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized
            : (Game.mainCamera.ScreenToWorldPoint(Input.mousePosition) - Skill.Owner.transform.position).normalized);
    }


    public class TargetedSkillSlot : ISkillSlot
    {
        public TargetedSkill Skill;
        public bool OnKeyDown { get; set; }

        public TargetedSkillSlot(TargetedSkill skill)
        {
            Skill = skill;
        }

        public void Use() => Skill.Use(Game.FindNearestToMouse());
    }

    public interface ISkillSlot
    {
        bool OnKeyDown { get; }
        void Use();
    }
}