using System.Reflection;
using Creatures;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class IntEdit : MonoBehaviour, IEdibleParameterUI
{
    public TextMeshProUGUI nameLabel;
    public TMP_InputField inputField;
    public int originalValue;
    public int pendingValue;

    private PropertyInfo field;
    private IntParam param;

    public void ChangeValue(int i)
    {
        int t = pendingValue;
        pendingValue += i;
        pendingValue = Mathf.Clamp(pendingValue, param.min, param.max);
        param.totalCost = GetEditCost();
        CodeRedactSystem.CountTotalCost();

        if (PlayerController.Player.Resources.Get(ResourceType.Bits).CanSpend(CodeRedactSystem.TotalCost) && IsSuitableValue(pendingValue))
        {
            inputField.text = "" + pendingValue;
            param.totalCost = GetEditCost();
        }
        else
        {
            Hints.Show("<color=\"red\">Can't afford</color>", 0.5f);
            pendingValue = t;
        }
        CodeRedactSystem.CountTotalCost();
    }

    public void ChangeValue(PointerEventData data)
    {
        int t = pendingValue;
        int i = (int)(Input.mousePosition.x - data.pressPosition.x) / Screen.width * 2;
        pendingValue += i;
        pendingValue = Mathf.Clamp(pendingValue, param.min, param.max);
        param.totalCost = GetEditCost();
        CodeRedactSystem.CountTotalCost();

        if (PlayerController.Player.Resources.Get(ResourceType.Bits).CanSpend(CodeRedactSystem.TotalCost) && IsSuitableValue(pendingValue))
        {
            inputField.text = "" + pendingValue;
            param.totalCost = GetEditCost();
        }
        else
        {
            Hints.Show("<color=\"red\">Can't afford</color>", 0.5f);
            pendingValue = t;
        }
        CodeRedactSystem.CountTotalCost();
    }

    public void ChangeValueSlider(float value)
    {
        pendingValue = (int)Mathf.Lerp(param.min, param.max, value);
        inputField.text = "" + pendingValue;
        param.totalCost = GetEditCost();
        CodeRedactSystem.CountTotalCost();
    }

    public void ChangeValue(string i)
    {
        int t = pendingValue;
        pendingValue = int.Parse(i);
        pendingValue = Mathf.Clamp(pendingValue, param.min, param.max);
        param.totalCost = GetEditCost();
        CodeRedactSystem.CountTotalCost();

        if (!PlayerController.Player.Resources.Get(ResourceType.Bits).CanSpend(CodeRedactSystem.TotalCost))
        {
            Hints.Show("<color=\"red\">Can't afford</color>", 0.5f);
            pendingValue = t;
        }
        CodeRedactSystem.CountTotalCost();
    }

    public IEdibleParameterUI Set(EditableParam editable)
    {
        param = editable as IntParam;

        nameLabel.text = param.displableName;

        field = param.component.GetType().GetProperty(param.fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        originalValue = (int)field.GetValue(param.component);
        pendingValue = originalValue;

        inputField.text = "" + pendingValue;
        EventTrigger trigger = inputField.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new(){eventID = EventTriggerType.Drag};
        entry.callback.AddListener((data) => { ChangeValue((PointerEventData)data); });
        trigger.triggers.Add(entry);

        return this;
    }

    public bool IsSuitableValue(int v) => v >= param.min && v <= param.max;

    public int GetEditCost() => Mathf.Abs(originalValue - pendingValue) * param.cost;

    public void Apply()
    {
        field.SetValue(param.component, pendingValue);
    }
}