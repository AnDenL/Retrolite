namespace FormulaSystem
{
    using UnityEngine;

    public struct Context
    {
        public Vector2 Position;
        public Creature Target;
        public Creature Owner;
        public GunBase Gun;
        public BulletBase Bullet;
    }
}