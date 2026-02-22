namespace FormulaSystem
{
    using System;
    using Creatures;
    using UnityEngine;

    using static StatVariable;

    public static class VariableResolver
    {
        public static float Resolve(StatVariable variable, Context context)
        {
            return variable switch
            {
                PH => context.Owner != null ? context.Owner.HealthComponent.GetHealthPercent() : 0,
                H => context.Target != null ? context.Target.HealthComponent.GetHealthPercent() : Break(variable, context),
                T => context.Bullet != null ? context.Bullet.GetLifetime() : Break(variable, context),
                E => context.Gun != null ? context.Gun.Data.Echo : Break(variable, context),
                D => context.Bullet != null ? context.Bullet.GetDistanceTravelled() : Break(variable, context),
                P => Vector2.Distance(PlayerController.Player.transform.position, context.Bullet == null ? context.Bullet.transform.position : Vector3.zero),
                A => context.Bullet != null ? context.Bullet.Number : Break(variable, context),
                M => PlayerController.Player.Resources.Get(ResourceType.Money).Count / 100,
                SP => context.Bullet != null ? context.Bullet.Speed : Break(variable, context),
                S => context.Bullet != null ? context.Bullet.Scale : Break(variable, context),
                R => context.Bullet != null ? context.Bullet.Spread : Break(variable, context) * Mathf.Deg2Rad,
                DT => context.Bullet != null ? context.Bullet.GetDestroyTime() : Break(variable, context),
                V => context.Target != null ? context.Target.Rb.velocity.magnitude : Break(variable, context),
                DR => Utilities.CalculateHomingAngle(context),
                _ => 0f
            };
        }

        public static void FillVariables(Context context, Span<float> resolvedVars)
        {
            resolvedVars[(int)PH] = context.Owner != null ? context.Owner.HealthComponent.GetHealthPercent() : 0;
            resolvedVars[(int)H] = context.Target != null ? context.Target.HealthComponent.GetHealthPercent() : 0;
            resolvedVars[(int)T] = context.Bullet != null ? context.Bullet.GetLifetime() : 0;
            resolvedVars[(int)E] = context.Gun != null ? context.Gun.Data.Echo : 0;
            resolvedVars[(int)D] = context.Bullet != null ? context.Bullet.GetDistanceTravelled() : 0;
            resolvedVars[(int)P] = Vector2.Distance(PlayerController.Player.transform.position, context.Bullet == null ? context.Bullet.transform.position : Vector3.zero);
            resolvedVars[(int)A] = context.Bullet != null ? context.Bullet.Number : 0;
            resolvedVars[(int)M] = PlayerController.Player.Resources.Get(ResourceType.Money).Count / 100;
            resolvedVars[(int)SP] = context.Bullet != null ? context.Bullet.Speed : 0;
            resolvedVars[(int)S] = context.Bullet != null ? context.Bullet.Scale : 0;
            resolvedVars[(int)R] = context.Bullet != null ? context.Bullet.Spread : 0 * Mathf.Deg2Rad;
            resolvedVars[(int)DT] = context.Bullet != null ? context.Bullet.GetDestroyTime() : 0;
            resolvedVars[(int)V] = context.Target != null ? context.Target.Rb.velocity.magnitude : 0;
            resolvedVars[(int)DR] = Utilities.CalculateHomingAngle(context);
        }

        public static float Break(StatVariable variable, Context context)
        {
            return 0; //Add weapon breaking in future
        }
    }

    public enum StatVariable
    {
        PH,
        H,
        T,
        E,
        D,
        P,
        A,
        M,
        SP,
        S,
        R,
        DT,
        V,
        DR
    }
}