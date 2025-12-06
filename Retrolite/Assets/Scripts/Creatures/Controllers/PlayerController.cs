using System;
using System.Collections.Generic;
using UnityEngine;

namespace Creatures
{
    [CreateAssetMenu(fileName = "PlayerController", menuName = "CreatureAI/Controllers/PlayerController")]
    public class PlayerController : AIController
    {
        public Dictionary<KeyCode, ISkillSlot> SkillSlots = new();

        public static Player Player => (Player)Instance.Owner;

        public static PlayerController Instance;
        public static bool CanInteract = true;

        public override bool IsPlayer => true;

        public WeaponManager WeaponManager { get; private set; }

        public override void Init(Creature owner)
        {
            base.Init(owner);

            Instance = this;

            foreach (var skill in owner.ActiveSkills)
                NewSlot(skill);

            target = MouseTarget.instance;
            owner.OnNewSkill += NewSlot;

            WeaponManager = owner.GetComponentInChildren<WeaponManager>();
            if (WeaponManager)
            {
                SkillSlots.Add(KeyCode.R, new EventSkillSlot(WeaponManager.Reload));
            }
        }

        public override void UpdateAI()
        {
            if (!CanInteract)
                return;

            Vector2 moveDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

            owner.LookAt(Game.mainCamera.ScreenToWorldPoint(Input.mousePosition));

            foreach (var slot in SkillSlots)
                if (slot.Value.OnKeyDown ? Input.GetKeyDown(slot.Key) : Input.GetKey(slot.Key)) slot.Value.Use();

            movement.Use(moveDir);
            if (WeaponManager != null) HandleWeaponManager();
        }

        private void HandleWeaponManager()
        {
            WeaponManager.Rotate(Game.mainCamera.ScreenToWorldPoint(Input.mousePosition));

            if (Input.mouseScrollDelta.y == 0) return;
            int direction = Input.mouseScrollDelta.y > 0 ? -1 : 1;
            WeaponManager.Scroll(direction);
        }

        public override Vector3 GetDirectionToTarget() =>
            (Game.mainCamera.ScreenToWorldPoint(Input.mousePosition) - Owner.transform.position).normalized;

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
        private static KeyCode GetKeyCodeByType(SkillType type)
        {
            KeyCode result = KeyCode.None;

            KeyCode[] keys;

            switch (type)
            {
                case SkillType.Attack:
                    keys = new KeyCode[] { KeyCode.Mouse0, KeyCode.Mouse1, KeyCode.Q, KeyCode.F };
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
                    keys = new KeyCode[] { KeyCode.E, KeyCode.V, KeyCode.B, KeyCode.N };
                    break;
                default:
                    keys = new KeyCode[0];
                    break;
            }

            foreach (var key in keys)
            {
                if (!Instance.SkillSlots.ContainsKey(key))
                    return key;
            }

            return result;
        }
    }
    
    public class EventSkillSlot  : ISkillSlot
    {
        public event System.Action OnPressed;
        public bool OnKeyDown { get; set; }

        public EventSkillSlot (System.Action action)
        {
            OnPressed = action;
        }

        public void Use() => OnPressed?.Invoke();
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
            UseWalkInput = skill.Type == SkillType.Movement;
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

        public void Use()
        {
            Creature target = Game.FindNearestToMouse();
            if (!target)
            {
                Hints.Show("No target", 1, AnimationCurve.Linear(0, 1, 1, 0));
                return;
            }
            Skill.Use(target);
        }
    }

    public interface ISkillSlot
    {
        bool OnKeyDown { get; }
        void Use();
    }
}