namespace CalculatingSystem
{
    using UnityEngine;
    public static class Utilities
    {
        public static float CalculateHomingAngle(Context context)
        {
            if (context.Bullet == null || context.Owner == null)
                return 0f;

            Vector2 bulletDir = context.Bullet.transform.up;
            Debug.Log(context.Target == null);
            Vector2 pos = (context.Target == null ? context.Owner.Controller.GetTargetPosition() : context.Target.transform.position) - context.Bullet.transform.position;
            if (pos.magnitude == 0) return 0;
            Vector2 targetDir = pos.normalized;

            float angle = Mathf.Atan2(targetDir.y, targetDir.x) - Mathf.Atan2(bulletDir.y, bulletDir.x);
            angle = Mathf.DeltaAngle(0, angle * Mathf.Rad2Deg) * Mathf.Deg2Rad;

            return angle;
        }

    }
}
