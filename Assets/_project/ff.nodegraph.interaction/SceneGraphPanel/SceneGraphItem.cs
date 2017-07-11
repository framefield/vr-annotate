using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ff.nodegraph.interaction
{
    public class SceneGraphItem : MonoBehaviour
    {
        [SerializeField]
        TMPro.TextMeshPro _label;

        public string Text
        {
            get { return _label.text; }
            set { _label.text = value; }
        }

        public int Indentation
        {
            set { _label.transform.localPosition = Vector3.right * value * INDENTATION_WIDHT; }
        }

        private float INDENTATION_WIDHT = 0.1f;
    }
}