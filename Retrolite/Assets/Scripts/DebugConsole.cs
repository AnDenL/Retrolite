using TMPro;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class DebugConsole : MonoBehaviour
{ 
    [SerializeField] private TextMeshProUGUI _consoleHistory;
    [SerializeField] private GameObject  _field;
    [SerializeField] private EventSystem _eventSystem;
    private TMP_InputField _fieldText;
    private List<string> _history;

    private void Start()
    {
        _fieldText = _field.GetComponent<TMP_InputField>();
    }

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(_field);
    }

    public void InputText(string text)
    {
        //_history.Add(new string(text));

        MethodInfo methodInfo = this.GetType().GetMethod(GetCommand(ref text), BindingFlags.Instance | BindingFlags.NonPublic);
        if(methodInfo != null)
            methodInfo.Invoke(this, new object[] { text });

        _fieldText.text = null;
    }

    public void Tips(string text)
    {
        
    }
    private void LoadScene(string argument)
    {
        int sceneNumber = 0;
        if(int.TryParse(argument, out sceneNumber))
        {
            sceneNumber = Mathf.Clamp(sceneNumber, 0, 5);
            SceneManager.LoadScene(sceneNumber);
        }
        else 
            WriteLine("Only numbers are accepted as arguments to the LoadScene"); 
    }

    private void Restart(string argument)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void GetMethod(string argument)
    {
        GameObject obj = GameObject.Find(GetCommand(ref argument));
        var component = obj.GetComponent(GetCommand(ref argument));
        if(component == null)
            WriteLine("Component not found");
        if(obj == null) 
            WriteLine("Object not found");
        else
        {
            var methodInfo = component.GetType().GetMethod(GetCommand(ref argument), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if(methodInfo != null)
            {
                methodInfo.Invoke(component, null);
            }
            else WriteLine("Method not found");
        } 
    }
                 
    private void GetMethodInt(string argument)
    {
        int f = 0;
        GameObject obj = GameObject.Find(GetCommand(ref argument));
        var component = obj.GetComponent(GetCommand(ref argument));
        if(component == null)
            WriteLine("Component not found");
        if(obj == null) 
            WriteLine("Object not found");
        else
        {
            var methodInfo = component.GetType().GetMethod(GetCommand(ref argument), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if(methodInfo != null)
            {
                if(int.TryParse(argument, out f))
                    methodInfo.Invoke(component, new object[] { f });
                else 
                    WriteLine("Wrong argument");
            }
            else WriteLine("Method not found");
        } 
    }

    private void GetMethodFloat(string argument)
    {
        float f = 0;
        GameObject obj = GameObject.Find(GetCommand(ref argument));
        var component = obj.GetComponent(GetCommand(ref argument));
        if(component == null)
            WriteLine("Component not found");
        if(obj == null) 
            WriteLine("Object not found");
        else
        {
            var methodInfo = component.GetType().GetMethod(GetCommand(ref argument), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if(methodInfo != null)
            {
                if(float.TryParse(argument, out f))
                    methodInfo.Invoke(component, new object[] { f });
                else 
                    WriteLine("Wrong argument");
            }
            else WriteLine("Method not found");
        } 
    }
    private void GetMethodString(string argument)
    {
        GameObject obj = GameObject.Find(GetCommand(ref argument));
        var component = obj.GetComponent(GetCommand(ref argument));
        if(component == null)
            WriteLine("Component not found");
        if(obj == null) 
            WriteLine("Object not found");
        else
        {
            var methodInfo = component.GetType().GetMethod(GetCommand(ref argument), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if(methodInfo != null)
            {
                methodInfo.Invoke(component, new object[] { argument });
            }
            else WriteLine("Method not found");
        }
    }

    private void SetTimeScale(string argument)
    {
        float timeScale = 1;
            if(float.TryParse(argument, out timeScale))
            {
                timeScale = Mathf.Clamp(timeScale, 0, 100); 
                MainManu._timeScale = timeScale;
                WriteLine("Time scale changed to " + timeScale);
            }
    }

    private void Spawn(string argument)
    {
        string name = GetCommand(ref argument);
        int count = 0;
        GameObject prefab = Resources.Load<GameObject>(name);
        if (prefab != null)
        {
            if(int.TryParse(argument, out count))
            {
                Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);  
                for(int i = 0; i < count; i++)Instantiate(prefab, cursorPosition, Quaternion.identity);   
                WriteLine(count + "objects " + name + " created");
            }
            else 
            {
                Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);  
                Instantiate(prefab, cursorPosition, Quaternion.identity);   
                WriteLine("Object " + name + " created");
            }
        }
        else
            WriteLine("Prefab "+ name +" not found");
    }

    private void Print(string text)
    {
        WriteLine(text);
    }
    private void WriteLine(string text)
    {
        _consoleHistory.text += text + "\n";
    }
    private string GetCommand(ref string text)
    {
        string command = "";
        bool idk = false;
        foreach(char letter in text)
        {
            if(letter != ' ') 
                command += letter;
            else 
            {
                idk = true;
                break;
            }
        }
        text = text.Remove(0, command.Length + Convert.ToInt32(idk));
        Debug.Log(command);
        Debug.Log(text);
        return command;
    }
}
