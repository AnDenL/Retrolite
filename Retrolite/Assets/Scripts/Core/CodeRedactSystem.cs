using System;
using System.Collections;
using Creatures;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CodeEditManager : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Transform window;

    public static bool IsEditing;
    public static CodeEditManager instance;

    private void Awake()
    {
        instance = this;
    }

    public static void Redact(string name, Vector3 position, EditableParam[] editables)
    {
        if (IsEditing) return;

        IsEditing = true;
        var win = instance.window;
        Time.timeScale = 0.1f;
        PlayerController.CanInteract = false;

        foreach (Transform t in win.transform)
            Destroy(t.gameObject);
        
        win.gameObject.SetActive(true);
        win.transform.position = Game.mainCamera.WorldToScreenPoint(position);

        Instantiate(instance.prefabs[0], win.transform).GetComponent<TextMeshProUGUI>().text = name;
        foreach (EditableParam editable in editables)
        {
            switch (editable)
            {
                case IntParam param:
                    Instantiate(instance.prefabs[1], win.transform);
                    break;
                case FloatParam param:
                    Instantiate(instance.prefabs[2], win.transform);
                    break;
                case ActionParam param:
                    Instantiate(instance.prefabs[4], win.transform);
                    break;
            }
        }
        instance.StartCoroutine(instance.Animate());
    }

    private IEnumerator Animate()
    {
        while (Game.pixelCamera.assetsPPU < 48)
        {
            yield return new WaitForSecondsRealtime(2/(60 - Game.pixelCamera.assetsPPU));
            Game.pixelCamera.assetsPPU++;
        }
    }

    public void Apply()
    {
        Time.timeScale = Game.TimeSpeed;
        IsEditing = false;
        Game.pixelCamera.assetsPPU = 16;
    }
}

[Serializable]
public abstract class EditableParam
{
    public string displayName;
    public int cost;
}

[Serializable]
public class IntParam : EditableParam
{
    public Component target;
    public string fieldName;

    public int min;
    public int max;
}

[Serializable]
public class FloatParam : EditableParam
{
    public Component target;
    public string fieldName;

    public float min;
    public float max;
}

[Serializable]
public class ActionParam : EditableParam
{
    public Component target;
    public string methodName;
}
