using System.Collections;
using System.Collections.Generic;
using ff.vr.interaction;
using UnityEngine;


namespace ff.nodegraph.interaction
{
    public class NodeGraphOutlinerItem : MonoBehaviour
    {
        public Color BackgroundColor = Color.gray;
        public Color LabelColor = Color.white;

        public Color SelectedBackgroundColor = Color.blue;
        public Color SelectedLabelColor = Color.white;

        public bool IsHighlighted;

        [Header("--- internal prefab references ----")]
        [SerializeField]
        LaserPointerButton _button;

        [SerializeField]
        TMPro.TextMeshPro _label;


        public NodeGraphOutliner SceneGraphPanel { get; set; }

        public Node Node
        {
            get { return _node; }
            set
            {
                _node = value;
                UpdateUI();
            }
        }

        public bool IsSelected { get; set; }

        // public bool IsSelected
        // {
        //     get
        //     {
        //         return SelectionManager.Instance.IsItemSelected(_node) || IsHighlighted;
        //     }
        // }


        public string Text
        {
            get { return _label.text; }
            set { _label.text = value; }
        }

        public int Indentation
        {
            set { _label.transform.localPosition = Vector3.right * value * INDENTATION_WIDHT; }
        }


        /** Called from LaserPointButton */
        public void OnClicked()
        {
            SelectionManager.Instance.SelectItem(_node);
        }

        private void UpdateUI()
        {
            var on = IsSelected || IsHighlighted;
            _button.SetColor(on ? SelectedBackgroundColor : BackgroundColor);
            _label.color = on ? SelectedLabelColor : LabelColor;
        }

        public void OnHover()
        {
            NodeSelectionManager._instance.SetHoveredNode(_node);
        }

        public void OnUnhover()
        {
            NodeSelectionManager._instance.SetHoveredNodeToNull();
        }

        private float INDENTATION_WIDHT = 0.1f;
        private Node _node;
    }
}