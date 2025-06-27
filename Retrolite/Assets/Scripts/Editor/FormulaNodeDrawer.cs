#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using CalculatingSystem;

[CustomPropertyDrawer(typeof(FormulaNode), true)]
public class FormulaNodeDrawer : PropertyDrawer
{
    private const float ButtonWidth = 35f;
    private const float ContentOffset = 250f;
    private const float Spacing = 4f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var node = property.managedReferenceValue;

        if (node is Expression)
        {
            return EditorGUIUtility.singleLineHeight + Spacing; // Один рядок для виразу
        }

        return EditorGUIUtility.singleLineHeight; // Один рядок для інших
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var node = property.managedReferenceValue;

        if (node == null)
        {
            // Немає значення – кнопка "Set Formula"
            Rect buttonRect1 = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            if (GUI.Button(buttonRect1, "Set Formula"))
                ShowTypeMenu(property);

            EditorGUI.EndProperty();
            return;
        }

        Rect fieldRect = new Rect(position.x, position.y, position.width - ButtonWidth - Spacing, EditorGUIUtility.singleLineHeight);
        Rect buttonRect = new Rect(position.xMax - ButtonWidth, position.y, ButtonWidth, EditorGUIUtility.singleLineHeight);

        if (node is ConstantNode)
        {
            var valueProp = property.FindPropertyRelative("Value");
            EditorGUI.PropertyField(fieldRect, valueProp, label);
        }
        else if (node is VariableNode)
        {
            var varProp = property.FindPropertyRelative("Variable");
            EditorGUI.PropertyField(fieldRect, varProp, label);
        }
        else if (node is Expression)
        {
            // Виводимо label зліва (наприклад "Speed:")
            float labelWidth = EditorGUIUtility.labelWidth + ContentOffset;
            Rect labelRect = new Rect(fieldRect.x, fieldRect.y, labelWidth, fieldRect.height);
            EditorGUI.LabelField(labelRect, label);

            float contentX = labelRect.xMax + Spacing - ContentOffset;
            float contentWidth = fieldRect.width - labelWidth - Spacing + ContentOffset;

            float thirdWidth = (contentWidth - Spacing * 2) / 3f;

            var leftProp = property.FindPropertyRelative("Left");
            var opProp = property.FindPropertyRelative("Operation");
            var rightProp = property.FindPropertyRelative("Right");

            Rect leftRect = new Rect(contentX, fieldRect.y, thirdWidth, fieldRect.height);
            Rect opRect = new Rect(leftRect.xMax + Spacing, fieldRect.y, thirdWidth, fieldRect.height);
            Rect rightRect = new Rect(opRect.xMax + Spacing, fieldRect.y, thirdWidth, fieldRect.height);

            EditorGUI.PropertyField(leftRect, leftProp, GUIContent.none);
            EditorGUI.PropertyField(opRect, opProp, GUIContent.none);
            EditorGUI.PropertyField(rightRect, rightProp, GUIContent.none);
        }
        else
        {
            EditorGUI.LabelField(fieldRect, $"Unsupported node: {node.GetType().Name}");
        }

        // Кнопка зміни типу
        if (GUI.Button(buttonRect, "Set"))
            ShowTypeMenu(property);

        EditorGUI.EndProperty();
    }

    private void ShowTypeMenu(SerializedProperty property)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Constant"), false, () => SetNodeType(property, new ConstantNode(0)));
        menu.AddItem(new GUIContent("Variable"), false, () => SetNodeType(property, new VariableNode()));
        menu.AddItem(new GUIContent("Expression"), false, () => SetNodeType(property, new Expression()));
        menu.ShowAsContext();
    }

    private void SetNodeType(SerializedProperty property, FormulaNode node)
    {
        property.serializedObject.Update();
        property.managedReferenceValue = node;
        property.serializedObject.ApplyModifiedProperties();
    }
}
#endif
