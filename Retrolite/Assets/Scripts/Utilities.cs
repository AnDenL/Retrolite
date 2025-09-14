using UnityEngine;
using CalculatingSystem;

public static class Utilities
{
    public static float CalculateHomingAngle(FormulaContext context)
    {
        if (context.Bullet == null || context.Owner == null)
            return 0f;

        Creature target = null;
        if (context.Owner.Alignment == Alignment.Enemy || context.Owner.Alignment == Alignment.EvilEnemy)
            target = Player.instance.Creature;
        else
            target = context.Owner.FindTarget();

        if (target == null) return 0f;

        Vector2 bulletDir = context.Bullet.transform.up;
        Vector2 targetDir = (target.transform.position - context.Bullet.transform.position).normalized;

        float angle = Mathf.Atan2(targetDir.y, targetDir.x) - Mathf.Atan2(bulletDir.y, bulletDir.x);
        angle = Mathf.DeltaAngle(0, angle * Mathf.Rad2Deg) * Mathf.Deg2Rad;

        return angle;
    }

}