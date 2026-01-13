using UnityEngine;

namespace CalculatingSystem
{
    public struct Context
    {
        public HealthBase TargetHealth;
        public Vector2 Position;
        public Creature Target;
        public Creature Owner;
        public GunBase Gun;
        public BulletBase Bullet;
    }
}
