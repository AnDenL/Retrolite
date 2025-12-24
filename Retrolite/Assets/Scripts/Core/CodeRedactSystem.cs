using System;
using System.Collections;
using System.Reflection;
using Creatures;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CodeEditManager : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Transform window;
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Slider slider;

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
        Transform win = instance.window;
        Time.timeScale = 0.08f;
        PlayerController.CanInteract = false;

        instance.label.text = name;

        for (int i = 1; i < win.childCount - 1; i++)
            Destroy(win.GetChild(i).gameObject);
        
        win.gameObject.SetActive(true);
        win.transform.position = Game.mainCamera.WorldToScreenPoint(position);

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
                case EnumParam param:
                    Instantiate(instance.prefabs[3], win.transform);
                    break;
                case ActionParam param:
                    var button = Instantiate(instance.prefabs[4], win.transform).GetComponent<Button>();
                    button.transform.SetSiblingIndex(1);
                    button.GetComponentInChildren<TextMeshProUGUI>().text = param.displableName;

                    var target = param.component;
                    var method = target.GetType().GetMethod(param.fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    if (method == null)
                    {
                        Debug.LogError($"Method not found: {param.fieldName}");
                        break;
                    }

                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() =>
                    {
                        if (param.singleuse) instance.Apply();
                        method.Invoke(target, new object[1] {PlayerController.Player});
                    });
                    break;
            }
        }
        instance.StartCoroutine(instance.Animate());
    }

    private IEnumerator Animate()
    {
        float t = 0;
        CameraController.SetZoom(32);
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            slider.value = t*2;

            yield return null;
        }

        Apply();
    }

    public bool Apply()
    {
        Time.timeScale = Game.TimeSpeed;
        IsEditing = false;
        CameraController.SetZoom(16);
        PlayerController.CanInteract = true;
        window.gameObject.SetActive(false);

        return true;
    }
}

[Serializable]
public abstract class EditableParam
{
    public string displableName;
    public string fieldName;
    public Component component;
    public int cost;

    public abstract int CalculateCost();
}

[Serializable]
public class IntParam : EditableParam
{
    public int originalValue;
    public int pendingValue;
    public int min;
    public int max;

    public override int CalculateCost() => Math.Abs(originalValue - pendingValue) * cost;
}

[Serializable]
public class FloatParam : EditableParam
{
    public float originalValue;
    public float pendingValue;
    public float min;
    public float max;

    public override int CalculateCost() => (int)(Math.Abs(originalValue - pendingValue) * cost);
}

[Serializable]
public class EnumParam : EditableParam
{
    public int originalValue;
    public int pendingValue;

    public override int CalculateCost() => originalValue == pendingValue ? 0 : cost;
}

[Serializable]
public class ActionParam : EditableParam
{
    public bool singleuse;

    public override int CalculateCost() => cost;
}
