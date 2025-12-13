using UnityEngine;
using Creatures;
using UnityEngine.Experimental.Rendering.Universal;

public static class Game
{
    public static Camera mainCamera;
    public static PixelPerfectCamera pixelCamera;
    
    public static float TimeSpeed = 1;
    public static bool IsPaused;

    private static readonly Collider2D[] results = new Collider2D[32];
    
    public static Creature FindNearestToMouse(float radius = 1f)
    {
        Vector3 cursorWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursorWorldPos.z = 0f;

        int count = Physics2D.OverlapCircleNonAlloc(cursorWorldPos, radius, results);

        Creature nearestCreature = null;
        float nearestSqrDist = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            var col = results[i];
            if (!col) continue;

            if (!col.TryGetComponent(out Creature creature))
                continue;

            if (creature.Alignment != Alignment.Enemy) continue;

            float sqrDist = ((Vector2)col.transform.position - (Vector2)cursorWorldPos).sqrMagnitude;
            if (sqrDist < nearestSqrDist)
            {
                nearestSqrDist = sqrDist;
                nearestCreature = creature;
            }
        }

        return nearestCreature;
    }
}