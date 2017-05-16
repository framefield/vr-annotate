using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class VRConsole : MonoBehaviour
{
    private VRConsole _instance;
    public Text TextObject;

    public static List<string> _lines = new List<string>();
    private static bool _textNeedsUpdate;


    void Start()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    void OnEnable () 
    {
        Application.logMessageReceived += HandleLog;
    }


    void OnDisable () 
    {
        Application.logMessageReceived -= HandleLog;
    }


    void HandleLog (String logString,String stackTrace, LogType type) 
    {
        _lines.Add(logString);
        _textNeedsUpdate = true;
    }

     void Update()
    {
        if (_textNeedsUpdate)
        {
            UpdateTextConsole();
            _textNeedsUpdate = false;
        }
    }
        

    VRConsole()
    {
        _instance = this;
    }


    public void LogCallback(string message, string stackTrace, LogType type)
    {
        string line = Time.time.ToString("0.0: ");
        if (type != LogType.Log)
        {
            line += type + ": ";
        }
        line += message;
        _lines.Add(line);
        _textNeedsUpdate = true;
    }
   

    static public void Log(String msg)
    {
        _lines.Add(msg);
        _textNeedsUpdate = true;

    }


    private void UpdateTextConsole() 
    {
        if (_lines.Count > 20)
        {
            _lines.RemoveAt(0);
        }

        var canvas = _instance.gameObject.transform.parent.gameObject;
        if (!canvas.activeSelf)
            canvas.SetActive(true);


        TextObject.text = String.Join("\n", _lines.ToArray());
    }
}

