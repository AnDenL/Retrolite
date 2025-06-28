#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using CalculatingSystem;

[CustomPropertyDrawer(typeof(FormulaNode), true)]
public class FormulaNodeDrawer : PropertyDrawer
{
    private const float ButtonWidth = 35f;
    private const float Spacing = 4f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var node = property.managedReferenceValue;
        if (node == null)
            return EditorGUIUtility.singleLineHeight;

        if (node is Expression)
        {
            // Для Expression беремо висоту 1 рядка
            return EditorGUIUtility.singleLineHeight + Spacing;
        }

        // Для інших — стандартно
        return EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var node = property.managedReferenceValue;

        if (node == null)
        {
            if (GUI.Button(position, "Set Formula"))
                ShowTypeMenu(property);

            EditorGUI.EndProperty();
            return;
        }

        // Загальна область без кнопки
        Rect fieldRect = new Rect(
            position.x,
            position.y,
            position.width - ButtonWidth - Spacing,
            EditorGUIUtility.singleLineHeight
        );

        // Кнопка Set
        Rect buttonRect = new Rect(
            fieldRect.xMax + Spacing,
            position.y,
            ButtonWidth,
            EditorGUIUtility.singleLineHeight
        );

        if (node is ConstantNode)
        {
            var valueProp = property.FindPropertyRelative("Value");
            EditorGUI.PropertyField(fieldRect, valueProp, label);
        }
        else if (node is SinNode)
        {
            var nodeProp = property.FindPropertyRelative("Node");
            EditorGUI.PropertyField(fieldRect, nodeProp, label);
        }
        else if (node is CosNode)
        {
            var nodeProp = property.FindPropertyRelative("Node");
            EditorGUI.PropertyField(fieldRect, nodeProp, label);
        }
        else if (node is VariableNode)
        {
            var varProp = property.FindPropertyRelative("Variable");
            EditorGUI.PropertyField(fieldRect, varProp, label);
        }
        else if (node is Expression)
        {
            float labelWidth = EditorGUIUtility.labelWidth;
            Rect labelRect = new Rect(fieldRect.x, fieldRect.y, labelWidth, fieldRect.height);
            EditorGUI.LabelField(labelRect, label);

            float contentX = labelRect.xMax + Spacing;
            float contentWidth = fieldRect.width - labelWidth - Spacing;

            // Ділимо простір на три частини
            float thirdWidth = (contentWidth - Spacing * 2) / 3f;

            var leftProp = property.FindPropertyRelative("Left");
            var opProp = property.FindPropertyRelative("Operation");
            var rightProp = property.FindPropertyRelative("Right");

            Rect leftRect = new Rect(contentX, fieldRect.y, thirdWidth, fieldRect.height);
            Rect opRect = new Rect(leftRect.xMax + Spacing, fieldRect.y, thirdWidth, fieldRect.height);
            Rect rightRect = new Rect(opRect.xMax + Spacing, fieldRect.y, thirdWidth, fieldRect.height);

            // Увага: тут використовуємо PropertyField без label
            EditorGUI.PropertyField(leftRect, leftProp, GUIContent.none, true);
            EditorGUI.PropertyField(opRect, opProp, GUIContent.none);
            EditorGUI.PropertyField(rightRect, rightProp, GUIContent.none, true);
        }
        else
        {
            EditorGUI.LabelField(fieldRect, $"Unsupported node: {node.GetType().Name}");
        }

        if (GUI.Button(buttonRect, "Set"))
            ShowTypeMenu(property);

        EditorGUI.EndProperty();
    }

    private void ShowTypeMenu(SerializedProperty property)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Constant"), false, () => SetNodeType(property, new ConstantNode(0)));
        menu.AddItem(new GUIContent("SinNode"), false, () => SetNodeType(property, new SinNode()));
        menu.AddItem(new GUIContent("CosNode"), false, () => SetNodeType(property, new CosNode()));
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
