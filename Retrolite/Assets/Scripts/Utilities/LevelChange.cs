using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChange : Interactable
{
    [SerializeField] private int sceneIndex;
    public override void Interact(Creature creature)
    {
        base.Interact(creature);
        SceneManager.LoadScene(sceneIndex);
    }
}