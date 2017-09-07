using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GridCreator : MonoBehaviour
{
    public bool toggleCreation;
    public int n, m;
    public float dist, yPos;
    private List<List<GameObject>> goGrid = new List<List<GameObject>>();
    public GameObject GOReference;
    public GameObject Root;

    void Start()
    {

    }

    void Update()
    {
        if (toggleCreation)
        {
            foreach (var goRow in goGrid)
                foreach (var go in goRow)
                    DestroyImmediate(go);

            var lastNode = Root;
            for (int i = 0; i < n; i++)
            {
                var nextRow = new List<GameObject>();
                for (int j = 0; j < m; j++)
                {
                    var nextGO = Instantiate(GOReference, lastNode.transform);
                    nextGO.transform.position = new Vector3(i * dist, yPos, j * dist);
                    nextRow.Add(nextGO);
                    lastNode = nextGO;
                }
                goGrid.Add(nextRow);
            }
        }
        toggleCreation = false;
    }
}
