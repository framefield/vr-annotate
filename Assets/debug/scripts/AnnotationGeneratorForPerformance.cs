using System.Collections;
using System.Collections.Generic;
using System.IO;
using ff.vr.annotate.viz;
using ff.vr.interaction;
using UnityEngine;
using UnityEditor;

public class AnnotationGeneratorForPerformance : MonoBehaviour
{
    public int NumberOfAnnotationsToGenerate;
    public int GeneratedAnnotations = 0;

    void Start()
    {
        _data = new PerfomanceTestData();
        _data.FrameTimeOverAnnotation = new List<float>();
        _data.BatchesOverAnnotation = new List<int>();
        _data.Annotations = new List<int>();
    }

    void Update()
    {

        if (GeneratedAnnotations < NumberOfAnnotationsToGenerate)
        {
            if (Time.frameCount % 2 == 0)
                return;
            if (GeneratedAnnotations % 100 == 1)
            {
                _data.Annotations.Add(GeneratedAnnotations);
                _data.FrameTimeOverAnnotation.Add(Time.deltaTime);
                _data.BatchesOverAnnotation.Add(UnityStats.batches);
            }
            AnnotationManager.Instance.CreateDummyAnnotation(SelectionManager.Instance.SelectedNode, 5 * Random.insideUnitSphere);
            GeneratedAnnotations++;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var directory = Application.dataPath + "/debug/annotationGizmos";
            File.WriteAllText(directory + "performance_Data" + ".json", JsonUtility.ToJson(_data));
        }

    }

    private PerfomanceTestData _data;


    public struct PerfomanceTestData
    {
        public List<int> Annotations;
        public List<float> FrameTimeOverAnnotation;
        public List<int> BatchesOverAnnotation;



    }

}
