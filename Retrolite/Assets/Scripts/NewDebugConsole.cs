using TMPro;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class NewDebugConsole : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _consoleHistory;
    [SerializeField] private GameObject  _field;
    [SerializeField] private EventSystem _eventSystem;
    [SerializeField] private AudioSource _audio;

    private TMP_InputField _fieldText;
    private List<string> _history;

    //UnityEngine methods

    private void Start()
    {
        _fieldText = _field.GetComponent<TMP_InputField>();
    }

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(_field);
    }

    //Methods

    private void Command(string command)
    {
        command = command.ToLower();
        switch(GetCommand(ref command))
        {
            case "print": case "message": case "write":
                WriteLine(command);
                break;
            case "clear": case "clean":
                _consoleHistory.text = null;
                break;
            case "restart": case "reset":
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                break;
            case "loadscene":  case "scene":
                int sceneNumber = 0;
                if(int.TryParse(command, out sceneNumber))
                {
                    sceneNumber = Mathf.Clamp(sceneNumber, 0, 5);
                    SceneManager.LoadScene(sceneNumber);
                }
                break;
            case "do": 
                string type = GetCommand(ref command);
                GameObject obj = GameObject.Find(GetCommand(ref command));
                var component = obj.GetComponent(GetCommand(ref command));

                if(obj == null) 
                    WriteLine("Object not found");
                else if(component == null)
                    WriteLine("Component not found");

                else
                {
                    var methodInfo = component.GetType().GetMethod(GetCommand(ref command), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    if(methodInfo != null)
                    {
                        switch (type)
                        {
                            case "null":
                                methodInfo.Invoke(component, null);
                                //Україна є членом міжнарождних Європейських організацій: перспективи спів праці
                                break;
                            case "str": case "string":
                                methodInfo.Invoke(component, new object[] { command });
                                break;
                            case "int": case "integer":
                                int i = 0;
                                if(int.TryParse(command, out i))
                                    methodInfo.Invoke(component, new object[] { i });
                                else 
                                    WriteLine("Only numbers are valid");
                                break;
                            case "f": case "float":
                                float f = 0;
                                if(float.TryParse(command, out f))
                                    methodInfo.Invoke(component, new object[] { f });
                                else 
                                    WriteLine("Only numbers are valid");
                                break;
                        }
                    }
                    else WriteLine("Method not found");
                } 
                break;
                case "music": case "m":
                    string audioPath = UnityEditor.EditorUtility.OpenFilePanel("Add Song...", "C:/", "mp3");
                    WriteLine(audioPath);
                    WWW audioLoader = new WWW (audioPath);
                    var audioClip = audioLoader.GetAudioClip (false, false, AudioType.MPEG);
                    _audio.Stop();
                    _audio.clip = audioClip;
                    _audio.Play();
                    break;
        }
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
