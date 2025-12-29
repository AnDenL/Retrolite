namespace CalculatingSystem
{
    using UnityEngine;
    public static class Utilities
    {
        public static float CalculateHomingAngle(Context context)
        {
            if (context.Bullet == null || context.Owner == null || context.Owner.Target == null)
                return 0f;

            Vector2 bulletDir = context.Bullet.transform.up;
            Vector2 targetDir = (context.Owner.Target.transform.position - context.Bullet.transform.position).normalized;

            float angle = Mathf.Atan2(targetDir.y, targetDir.x) - Mathf.Atan2(bulletDir.y, bulletDir.x);
            angle = Mathf.DeltaAngle(0, angle * Mathf.Rad2Deg) * Mathf.Deg2Rad;

            return angle;
        }

    }
}
