using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class GUIDHelper : MonoBehaviour
{
    public static string GUIDPattern = "(\\s+)(\\[)(.{36})(\\])";

    public static String ExtractGUIDFromName(string name)
    {
        var match = Regex.Match(name, GUIDPattern);
        if (match.Success)
        {
            var guidString = match.Captures[0].Value;
            guidString = Regex.Replace(guidString, "(\\[|\\])", "");
            return guidString;
        }
        else
            return ErrorGUID.ToString();
    }

    public static String RemoveGUIDFromName(string name)
    {
        return Regex.Replace(name, GUIDPattern, "");
    }
    private static Guid ErrorGUID = new Guid();

}
