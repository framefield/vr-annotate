using System.Collections;
using System.Collections.Generic;
using System.IO;
using ff.nodegraph;
using ff.utils;
using ff.vr.annotate.datamodel;
using ff.vr.annotate.viz;
using UnityEngine;
using UnityEngine.Networking;

public static class Serialization
{
    const string SERVER_TARGETS_URI = "http://127.0.0.1:8301/targets/";
    static string LocalTargetDirectory { get { return Application.dataPath + "/db/targets/"; } }

    static string GetLocalAnnotationDirectory(Target target) { return LocalTargetDirectory + "/" + target.JsonLdId._guid + "/"; }
    static string GetLocalAnnotationDirectory(Annotation annotation) { return LocalTargetDirectory + "/" + annotation.Target.JsonLdId._guid + "/" + annotation.JsonLdId._guid + ".json"; }

    static string GetRemoteTargetURI(Target target) { return SERVER_TARGETS_URI + target.JsonLdId; }
    static string GetRemoteAnnotationURI(Target target) { return SERVER_TARGETS_URI + target.JsonLdId + "/annotations/"; }
    static string GetRemoteAnnotationURI(Annotation annotation) { return SERVER_TARGETS_URI + annotation.Target.JsonLdId + "/annotations/" + annotation.JsonLdId; }

    #region local

    public static void SyncWithLocalDirectory(Target target)
    {
        var path = LocalTargetDirectory + target.JsonLdId._guid + ".json";

        var targetExistsInDB = File.Exists(path);
        if (!targetExistsInDB)
        {
            File.WriteAllText(path, target.ToJson());
        }
        var annotationDirectoryForTargetExists = Directory.Exists(GetLocalAnnotationDirectory(target));
        if (!annotationDirectoryForTargetExists)
        {
            Directory.CreateDirectory(GetLocalAnnotationDirectory(target));
        }

        ReadAllAnnotationsFromLocalDirectory(target);
    }


    private static void ReadAllAnnotationsFromLocalDirectory(Target target)
    {
        Debug.Log("ReadingAnnotationsFromLocalDirectory ... downloading: " + GetLocalAnnotationDirectory(target));

        var filesInDirectory = Directory.GetFiles(GetLocalAnnotationDirectory(target), "*.json");
        Debug.Log("found " + filesInDirectory.Length + " annotations");
        foreach (var file in filesInDirectory)
        {
            var newAnnotation = new Annotation(File.ReadAllText(file));
            if (newAnnotation.TargetNode == null)
                continue;

            AnnotationManager.Instance.CreateAnnotationGizmo(newAnnotation);
            Debug.Log("created annotations " + newAnnotation.JsonLdId);
        }
    }


    public static void WriteAnnotationToLocalDirectory(Annotation annotation)
    {
        var annotationJson = annotation.ToJson();
        Debug.Log(GetLocalAnnotationDirectory(annotation));
        File.WriteAllText(GetLocalAnnotationDirectory(annotation), annotationJson);
    }

    #endregion

    #region remote

    public static IEnumerator SyncWithServerCoroutine(Target target)
    {
        UnityWebRequest www = UnityWebRequest.Get(GetRemoteTargetURI(target));
        yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
            yield break;
        }

        if (www.responseCode == 404)
        {
            yield return WriteTargetToServer(target);
            Debug.Log("Wrote Target " + target.JsonLdId.ToString() + " to  Server");
        }

        Debug.Log("Found Target " + target.JsonLdId.ToString() + " on  Server: ");
        yield return GetAnnotationsForTargetFromServer(target);
    }


    private static IEnumerator WriteTargetToServer(Target target)
    {
        var targetJson = target.ToJson();

        UnityWebRequest www = UnityWebRequest.Put(GetRemoteTargetURI(target), System.Text.Encoding.UTF8.GetBytes(targetJson));
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
            yield break;
        }
        Debug.Log("Upload complete with: " + www.error);
    }


    private static IEnumerator GetAnnotationsForTargetFromServer(Target target)
    {
        Debug.Log("ReadingAnnotationsFromServer ... downloading: " + GetRemoteAnnotationURI(target));
        UnityWebRequest www = UnityWebRequest.Get(GetRemoteAnnotationURI(target));

        yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
            yield break;
        }
        else
        {
            var allAnnotationsJson = www.downloadHandler.text;
            JSONObject allAnnotationJSON = new JSONObject(allAnnotationsJson);
            Debug.Log(allAnnotationJSON);
            foreach (var singleAnnotationJSON in allAnnotationJSON)
            {
                Debug.Log("Downloaded annotation: " + singleAnnotationJSON);
                if (singleAnnotationJSON["type"].str != "Annotation")
                    continue;

                var newAnnotation = new Annotation(singleAnnotationJSON);
                if (newAnnotation.TargetNode == null)
                    continue;

                AnnotationManager.Instance.CreateAnnotationGizmo(newAnnotation);
            }
        }
    }

    public static IEnumerator WriteAnnotationToServer(Annotation annotation)
    {
        var annotationJson = annotation.ToJson();

        UnityWebRequest www = UnityWebRequest.Put(GetRemoteAnnotationURI(annotation), System.Text.Encoding.UTF8.GetBytes(annotationJson));
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
            yield break;
        }
        Debug.Log("Upload of Annotation complete with: " + www.error);
    }
    #endregion

}
