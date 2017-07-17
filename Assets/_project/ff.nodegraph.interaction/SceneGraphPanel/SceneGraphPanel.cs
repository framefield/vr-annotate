using System.Collections;
using System.Collections.Generic;
using ff.vr.interaction;
using UnityEngine;

namespace ff.nodegraph.interaction
{
    public class SceneGraphPanel : MonoBehaviour
    {
        public Node root;
        public List<Node> selectedNodes = new List<Node>();

        [Header("--- internal prefab references----")]

        [SerializeField]
        SceneGraphItem _itemPrefab;

        [SerializeField]
        Transform _itemsContainer;

        void Start()
        {
            _nodeSelectionManager = GameObject.FindObjectOfType<NodeSelectionManager>();
            RebuildUI();
        }


        public void SetSelectedNode(Node newNode)
        {
            selectedNodes.Clear();
            if (newNode != null)
            {
                selectedNodes.Add(newNode);

            }
            RebuildUI();
        }


        private void RebuildUI()
        {
            ClearItems();

            // Only show other root nodes
            //if (selectedNodes.Count == 0)
            //{
            foreach (var ng in _nodeSelectionManager.NodeGraphs)
            {
                AppendItem(ng.Node);
            }
            //}

            if (selectedNodes.Count > 1)
            {
                Debug.LogWarning("graph list only supports one item");
            }
            if (selectedNodes.Count == 1)
            {
                var node = selectedNodes[0];
                var parent = node.Parent;
                var path = new List<Node>();

                while (parent != null && parent.Parent != null)
                {
                    path.Insert(0, parent);
                    parent = parent.Parent;
                }

                var pathIndex = 1;
                foreach (var parentNode in path)
                {
                    AppendItem(parentNode, pathIndex);
                    pathIndex++;
                }
                AppendItem(node, pathIndex);
            }
        }


        private SceneGraphItem AppendItem(Node node, int indentation = 0)
        {
            var newItem = GameObject.Instantiate(_itemPrefab);
            _items.Add(newItem);
            newItem.Text = node.Name;
            newItem.Indentation = indentation;

            newItem.name += "-" + node.Name;
            newItem.Node = node;

            newItem.transform.localPosition = new Vector3(
                0,
                -LINE_HEIGHT * _itemsContainer.transform.childCount,
                0);

            newItem.transform.SetParent(_itemsContainer, false);
            newItem.SceneGraphPanel = this;
            return newItem;
        }


        internal void OnItemClicked(SceneGraphItem item)
        {
            SelectionManager.Instance.SelectItem(item.Node);
            //_nodeSelectionManager.SetSelectedNode(item.Node);
            SetSelectedNode(item.Node);
        }


        private void ClearItems()
        {
            foreach (var item in _items)
            {
                GameObject.DestroyImmediate(item.gameObject);
            }
            _items.Clear();
        }


        private float LINE_HEIGHT = 0.2f;
        private List<SceneGraphItem> _items = new List<SceneGraphItem>();
        private NodeSelectionManager _nodeSelectionManager;
    }
}
