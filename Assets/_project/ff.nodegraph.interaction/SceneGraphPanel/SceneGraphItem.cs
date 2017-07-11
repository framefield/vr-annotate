using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ff.nodegraph.interaction
{
    public class SceneGraphItem : MonoBehaviour
    {
        void Awake()
        {
            _textMeshPro = GetComponent<TMPro.TextMeshPro>();
        }

        public int Indentation = 0;

        public string text
        {
            get { return _textMeshPro.text; }
            set { _textMeshPro.text = value; }
        }

        public string indentation
        {
            get { return _textMeshPro.text; }
            set { _textMeshPro.text = value; }
        }

        private TMPro.TextMeshPro _textMeshPro;
    }
}