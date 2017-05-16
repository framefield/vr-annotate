using UnityEngine;
using System;
using System.Net;
using System.Collections.Generic;

namespace FlyingWormConsole3
{
public class ConsoleProRemoteServer : MonoBehaviour
{
	#if !NETFX_CORE && !UNITY_WEBPLAYER && !UNITY_WP8 && !UNITY_METRO && !UNITY_WEBGL
	public class HTTPContext
	{
		public HttpListenerContext context;
		public string path;

		public string Command
		{
			get
			{
				return WWW.UnEscapeURL(context.Request.Url.AbsolutePath);
			}
		}

		public HttpListenerRequest Request
		{
			get
			{
				return context.Request;
			}
		}

		public HttpListenerResponse Response
		{
			get
			{
				return context.Response;
			}
		}

		public HTTPContext(HttpListenerContext inContext)
		{
			context = inContext;
		}

		public void RespondWithString(string inString)
		{
			Response.StatusDescription = "OK";
			Response.StatusCode = (int)HttpStatusCode.OK;

			if (!string.IsNullOrEmpty(inString))
			{
				Response.ContentType = "text/plain";

				byte[] buffer = System.Text.Encoding.UTF8.GetBytes(inString);
				Response.ContentLength64 = buffer.Length;
				Response.OutputStream.Write(buffer,0,buffer.Length);
			}
		}
	}

	[System.SerializableAttribute]
	public class QueuedLog
	{
		public string message;
		public string stackTrace;
		public LogType type;
	}

	public int port = 51000;

	private bool _started;

	private static HttpListener listener = null;
	
	[NonSerializedAttribute]
	public List<QueuedLog> logs = new List<QueuedLog>();

	private static ConsoleProRemoteServer instance = null;

	void Awake()
	{
		if(instance != null)
		{
			Destroy(gameObject);
		}
		
		instance = this;
		
		DontDestroyOnLoad(gameObject);

		StartServer();
	}

	void StartServer()
	{
		if(_started)
		{
			return;
		}
		
		Debug.Log("Starting Console Pro Server on port : " + port);

		_started = true;
		listener = new HttpListener();
		listener.Prefixes.Add("http://*:"+port+"/");
		listener.Start();
		listener.BeginGetContext(ListenerCallback, null);
	}

	void StopServer()
	{
		if(!_started)
		{
			return;
		}
		
		Debug.Log("Stopping Console Pro Server on port : " + port);
		
		_started = false;
		listener.Close();
	}

	#if UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9

	void OnEnable()
	{
		StartServer();
		Application.RegisterLogCallback(LogCallback);
	}

	void Update()
	{
		Application.RegisterLogCallback(LogCallback);
	}

	void LateUpdate()
	{
		Application.RegisterLogCallback(LogCallback);
	}

	void OnDisable()
	{
		Application.RegisterLogCallback(null);
		StopServer();
	}

	#else

	void OnEnable()
	{
		StartServer();
		Application.logMessageReceived += LogCallback;
	}

	void OnDisable()
	{
		Application.logMessageReceived -= LogCallback;
		StopServer();
	}

	#endif

	public void LogCallback(string logString, string stackTrace, LogType type)
	{
		if(!logString.StartsWith("CPIGNORE"))
		{
			QueueLog(logString, stackTrace, type);
		}
	}

	void QueueLog(string logString, string stackTrace, LogType type)
	{
		logs.Add(new QueuedLog() { message = logString, stackTrace = stackTrace, type = type } );
	}

	void ListenerCallback(IAsyncResult result)
	{
		HTTPContext context = new HTTPContext(listener.EndGetContext(result));

		HandleRequest(context);
		
		listener.BeginGetContext(new AsyncCallback(ListenerCallback), null);
	}

	void HandleRequest(HTTPContext context)
	{
		// Debug.LogError("HANDLE " + context.Request.HttpMethod);
		// Debug.LogError("HANDLE " + context.path);

		bool foundCommand = false;

		switch(context.Command)
		{
			case "/NewLogs":
			{
				foundCommand = true;

				if(logs.Count > 0)
				{
					string response = "";

					//  foreach(QueuedLog cLog in logs)
					for(int i = 0; i < logs.Count; i++)
					{
						QueuedLog cLog = logs[i];
						response += "::::" + cLog.type;
						response += "||||" + cLog.message;
						response += ">>>>" + cLog.stackTrace + ">>>>";
					}

					context.RespondWithString(response);

					logs.Clear();
				}
				else
				{
					context.RespondWithString("");
				}
				break;
			}
		}

		if(!foundCommand)
		{
			context.Response.StatusCode = (int)HttpStatusCode.NotFound;
			context.Response.StatusDescription = "Not Found";
		}

		context.Response.OutputStream.Close();
	}
	#endif
}
}