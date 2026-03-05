using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Creatures;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CodeRedactSystem : MonoBehaviour
{
    public static EditableParam[] Params;
    public static int TotalCost;

    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Transform window;
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private TextMeshProUGUI costLabel;
    [SerializeField] private Slider slider;
    [SerializeField] private List<IEdibleParameterUI> uiEditable;
    [SerializeField] private CorruptibleBase target;

    public static bool IsEditing;
    public static CodeRedactSystem instance;

    private void Awake()
    {
        instance = this;
    }   

    public static void Redact(string name, Vector3 position, EditableParam[] editables, CorruptibleBase corruptible)
    {
        if (IsEditing) return;
        Params = editables;
        instance.target = corruptible;

        IsEditing = true;
        Transform win = instance.window;
        Game.TimeSpeed = 0.08f;
        Time.timeScale = Game.TimeSpeed;
        PlayerController.CanInteract = false;

        instance.label.text = name;

        for (int i = 2; i < win.childCount - 2; i++)
            Destroy(win.GetChild(i).gameObject);
        
        win.gameObject.SetActive(true);
        win.transform.position = Game.mainCamera.WorldToScreenPoint(position);
        instance.uiEditable = new();

        foreach (EditableParam editable in Params)
        {
            switch (editable)
            {
                case IntParam param:
                    var intp = Instantiate(instance.prefabs[1], win.transform);
                    intp.transform.SetSiblingIndex(2);
                    instance.uiEditable.Add(intp.GetComponent<IntEdit>().Set(param));
                    break;
                case FloatParam param:
                    var fp = Instantiate(instance.prefabs[2], win.transform);
                    fp.transform.SetSiblingIndex(2);
                    instance.uiEditable.Add(fp.GetComponent<FloatEdit>().Set(param));
                    break;
                case EnumParam param:
                    var enump = Instantiate(instance.prefabs[3], win.transform);
                    enump.transform.SetSiblingIndex(2);
                    instance.uiEditable.Add(enump.GetComponent<EnumEdit>().Set(param));
                    break;
                case ActionParam param:
                    var button = Instantiate(instance.prefabs[4], win.transform).GetComponent<Button>();
                    button.transform.SetSiblingIndex(2);
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
                case MessageParam param:
                    var message = Instantiate(instance.label.gameObject, win.transform);
                    message.transform.SetSiblingIndex(2);
                    message.GetComponent<TextMeshProUGUI>().text = param.Text;
                    break;
            }
        }
        instance.StartCoroutine(instance.Animate());
    }

    public static void CountTotalCost()
    {
        int i = 0;
        foreach(EditableParam editable in Params)
        {
            i += editable.totalCost;
        }
        TotalCost = i;

        instance.costLabel.text = "" + TotalCost;
    }

    private IEnumerator Animate()
    {
        float t = 0;
        CameraController.SetZoom(32);
        while (t < 0.5f)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
                t = 0.5f;

            t += Time.deltaTime;
            slider.value = t*2;

            yield return null;
        }

        Apply();
    }

    public void Apply()
    {
        Time.timeScale = 1;
        Game.TimeSpeed = 1;
        IsEditing = false;
        CameraController.SetZoom(16);
        PlayerController.CanInteract = true;
        window.gameObject.SetActive(false);
        target.IsBedingEdited = false;
        target.Invoke(nameof(CorruptibleBase.ResetStability), 1f);

        if (PlayerController.Player.Resources.Get(ResourceType.Bits).TrySpend(TotalCost))
        {
            foreach(var editable in uiEditable)
                editable.Apply();
        }
    }
}

[Serializable]
public abstract class EditableParam
{
    public string displableName;
    public string fieldName;
    public Component component;
    public int cost;
    public int totalCost;
}

[Serializable]
public class IntParam : EditableParam
{
    public int min;
    public int max;
}

[Serializable]
public class MessageParam : EditableParam
{
    [TextArea] public string Text;
}

[Serializable]
public class FloatParam : EditableParam
{
    public float min;
    public float max;
}

[Serializable]
public class EnumParam : EditableParam
{

}

[Serializable]
public class ActionParam : EditableParam
{
    public bool singleuse;
}
