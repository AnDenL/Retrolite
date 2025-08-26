using TMPro;
using MoonSharp.Interpreter;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Console : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI consoleOutput;
    [SerializeField] private TMP_InputField inputField;
    
    private static List<string> commandHistory = new List<string>();
    private EventSystem eventSystem;
    private int historyIndex = 0;
    private Script lua;

    private void Start()
    {
        lua = new Script();

        if (commandHistory.Count == 0) commandHistory.Add("");

        eventSystem = EventSystem.current;
        lua.Options.DebugPrint = s => AppendOutput(s);
    }

    private void AppendOutput(string message)
    {
        consoleOutput.text += message + "\n";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) inputField.text += "\n";
            else RunCommand();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            historyIndex++;
            if (historyIndex >= commandHistory.Count)
                historyIndex = commandHistory.Count - 1;
            inputField.text = commandHistory[historyIndex];
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            historyIndex--;
            if (historyIndex < 0)
                historyIndex = 0;

            inputField.text = commandHistory[historyIndex];
        }
    }

    public void RunCommand()
    {
        if (string.IsNullOrEmpty(inputField.text))
            return;
        string code = inputField.text;
        inputField.text = "";

        try
        {
            DynValue result = lua.DoString(code);

            if (result != null && result.Type != DataType.Void && result.Type != DataType.Nil)
            {
                AppendOutput(">" + result.ToString());
            }
        }
        catch (ScriptRuntimeException ex)
        {
            AppendOutput("Error: " + ex.DecoratedMessage);
        }

        commandHistory.Insert(1, code);
        historyIndex = 0;
        eventSystem.SetSelectedGameObject(inputField.gameObject);
    }
}
