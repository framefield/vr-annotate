using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ff.utils
{
    public class JsonTemplate
    {
        public static string FillTemplate(string template, Dictionary<string, string> attributesValues, bool validate = true)
        {
            // We're using StringBuild to avoid heap impact from countless reassignments
            var output = new System.Text.StringBuilder(template);
            output.Replace("'", "\"");

            foreach (var kvp in attributesValues)
            {
                var templateKey = "{" + kvp.Key + "}";
                if (validate)
                {
                    if (output.ToString().IndexOf(templateKey) == -1)
                    {
                        Debug.LogErrorFormat(" Trying to set undefined template key {0} -> {1}", kvp.Key, kvp.Value);
                    }
                }
                output.Replace(templateKey, kvp.Value);
            }
            var jsonResult = output.ToString();
            if (validate)
            {
                Regex r = new Regex(@"\{[A-Z][a-z]+\}", RegexOptions.IgnoreCase);
                var match = r.Match(jsonResult);
                if (match.Success)
                {
                    foreach (var result in match.Captures)
                    {
                        Debug.LogWarning("templateAttribute not set:" + result);
                    }
                }
            }
            return output.ToString();
        }
    }
}