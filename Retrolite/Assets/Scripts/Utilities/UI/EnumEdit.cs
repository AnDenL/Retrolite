using System.Linq;
using System.Reflection;
using Creatures;
using TMPro;
using UnityEngine;

public class EnumEdit : MonoBehaviour, IEdibleParameterUI
{
    public TextMeshProUGUI nameLabel;
    public TMP_Dropdown dropdown;
    public int originalValue;
    public int pendingValue;

    private PropertyInfo field;
    private EnumParam param;

    public void ChangeValue(int i)
    {
        pendingValue = i;
    }

    public IEdibleParameterUI Set(EditableParam editable)
    {
        param = editable as EnumParam;

        nameLabel.text = param.displableName;

        field = param.component.GetType().GetProperty(param.fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        var value = field.GetValue(param.component);
        originalValue = (int)value;

        string[] options = value.GetType().GetEnumNames();

        dropdown.ClearOptions();
        dropdown.AddOptions(options.ToList());
        dropdown.SetValueWithoutNotify(originalValue);

        return this;
    }

    public int GetEditCost() => originalValue == pendingValue ? 0 : param.cost;

    public void Apply()
    {
        field.SetValue(param.component, pendingValue);
    }
}