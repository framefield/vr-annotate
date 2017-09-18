using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ff.nodegraph;
using ff.vr.annotate.datamodel;
using UnityEngine;
using UnityEngine.Networking;

[ExecuteInEditMode]
public class NeonionInterface : MonoBehaviour
{
    public bool PutAnnotation;
    public bool GetAnnotation;

    public string TargetName;

    void Update()
    {
        if (PutAnnotation)
        {
            StartCoroutine(Upload(TargetName));
            PutAnnotation = false;
        }
        if (GetAnnotation)
        {
            StartCoroutine(Download());
            GetAnnotation = false;
        }
    }

    public static IEnumerator Upload(string targetName)
    {

        var filesInDirectory = Directory.GetFiles("E:/_users/dominik/vr-annotate/Assets/db/annotationCopiesForRESTTest", "*.json");
        foreach (var file in filesInDirectory)
        {
            var a = new Annotation(File.ReadAllText(file));
            Debug.Log(a);
            var aJson = ""; // Annotation.StaticToJson(a);
            Debug.Log(aJson);

            UnityWebRequest www = UnityWebRequest.Put("http://127.0.0.1:8301/targets/" + a.Guid, System.Text.Encoding.UTF8.GetBytes(aJson));
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.Send();

            // if (www.isNetworkError)
            // {
            //     Debug.Log(www.error);
            // }
            // else
            // {
            //     Debug.Log("Upload complete with: " + www.error);
            // }
        }
    }

    public static IEnumerator Download()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://127.0.0.1:8301/targets/");

        yield return www.Send();


        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Download complete: ");
            Debug.Log(www.downloadHandler.text);
        }
    }

    struct ReducedAnnotation
    {
        public string id;
        public string Text;
        public Guid GUID;

    }

}
