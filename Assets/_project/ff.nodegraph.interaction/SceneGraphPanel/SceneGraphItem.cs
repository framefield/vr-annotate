using System.Collections;
using System.Collections.Generic;
using ff.vr.interaction;
using UnityEngine;


namespace ff.nodegraph.interaction
{
    public class SceneGraphItem : MonoBehaviour
    {
        [Header("--- internal prefab references ----")]
        [SerializeField]
        LaserPointerButton _button;

        [SerializeField]
        TMPro.TextMeshPro _label;

        public Color HighlightBackgroundColor = Color.blue;
        public Color HighlightLabelColor = Color.white;
        public Color BackgroundColor = Color.gray;
        public Color LabelColor = Color.white;

        private Node _node;

        public Node Node
        {
            get { return _node; }
            set
            {
                _node = value;
                UpdateUI();
            }
        }


        public bool IsSelected
        {
            get
            {
                return SelectionManager.Instance.IsItemSelected(_node);
            }
        }

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

        public void UpdateUI()
        {
            _button.Color = IsSelected ? HighlightBackgroundColor : BackgroundColor;
            _label.color = IsSelected ? HighlightLabelColor : LabelColor;
            _button.UpdateUI();

            if (IsSelected)
            {
                Debug.Log("It's selected", this);
            }
        }

        public SceneGraphPanel SceneGraphPanel { get; set; }
        private float INDENTATION_WIDHT = 0.1f;
    }
}