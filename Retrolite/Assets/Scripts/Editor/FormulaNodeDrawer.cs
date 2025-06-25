#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using CalculatingSystem;

[CustomPropertyDrawer(typeof(FormulaNode), true)]
public class FormulaNodeDrawer : PropertyDrawer
{
    private const float ButtonWidth = 60f;
    private const float VerticalSpacing = 4f;
    private const float IndentOffset = 15f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Базовий рядок завжди однаковий
        float height = EditorGUIUtility.singleLineHeight;

        // Додаємо висоту вмісту, якщо розгорнуто і є дані
        if (property.isExpanded && property.managedReferenceValue != null)
        {
            // Отримуємо висоту всіх вкладених властивостей
            SerializedProperty iterator = property.Copy();
            SerializedProperty endProperty = property.GetEndProperty();
            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iterator, endProperty))
            {
                height += EditorGUI.GetPropertyHeight(iterator, true) + VerticalSpacing;
                enterChildren = false;
            }
        }

        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // 1. Заголовок з foldout і кнопкою
        Rect headerRect = new Rect(
            position.x,
            position.y,
            position.width - ButtonWidth,
            EditorGUIUtility.singleLineHeight
        );

        // Foldout, який контролює розгортання
        property.isExpanded = EditorGUI.Foldout(headerRect, property.isExpanded, label, true);

        // Кнопка зміни типу
        Rect buttonRect = new Rect(
            position.x + position.width - ButtonWidth,
            position.y,
            ButtonWidth,
            EditorGUIUtility.singleLineHeight
        );

        if (GUI.Button(buttonRect, "Change"))
        {
            ShowTypeMenu(property);
        }

        // 2. Відображення вмісту
        if (property.isExpanded && property.managedReferenceValue != null)
        {
            // Початкова позиція для вмісту
            float contentY = position.y + EditorGUIUtility.singleLineHeight + VerticalSpacing;

            // Ітерація по всіх вкладених властивостях
            SerializedProperty iterator = property.Copy();
            SerializedProperty endProperty = property.GetEndProperty();
            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iterator, endProperty))
            {
                float propertyHeight = EditorGUI.GetPropertyHeight(iterator, true);

                Rect propertyRect = new Rect(
                    position.x + IndentOffset,
                    contentY,
                    position.width - IndentOffset,
                    propertyHeight
                );

                EditorGUI.PropertyField(propertyRect, iterator, true);
                contentY += propertyHeight + VerticalSpacing;

                enterChildren = false;
            }
        }

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
        property.managedReferenceValue = node;
        property.serializedObject.ApplyModifiedProperties();
    }
}
#endif
