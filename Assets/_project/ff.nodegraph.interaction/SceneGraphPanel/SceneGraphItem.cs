using System.Collections;
using System.Collections.Generic;
using ff.vr.interaction;
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

        public Node Node { get; set; }

        public int Indentation
        {
            set { _label.transform.localPosition = Vector3.right * value * INDENTATION_WIDHT; }
        }

        /** Called from LaserPointButton */
        public void OnClicked()
        {
            SelectionManager.Instance.SelectItem(Node);
            //SceneGraphPanel(Node);
        }

        public SceneGraphPanel SceneGraphPanel { get; set; }
        private float INDENTATION_WIDHT = 0.1f;
    }
}