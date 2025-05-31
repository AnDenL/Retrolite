using UnityEngine;

public abstract class AbstractGenerator : MonoBehaviour
{
    [SerializeField] protected Vector2Int _startPosition = Vector2Int.zero;
    [SerializeField] protected TilemapGenerator _generator;

    public void GenerateDungeon()
    {
        _generator.Clear();
        Generate();
    }

    protected abstract void Generate();
}
