using System.Collections;
using System.Collections.Generic;
using ff.vr.annotate.datamodel;
using UnityEngine;

namespace ff.vr.interaction
{
    public class OnTeleportationArrivalOrientation : MonoBehaviour
    {
        [Header("--- internal prefab references -----")]

        [SerializeField]
        private Color _targetColor;


        // Use this for initialization
        void Start()
        {
            _allRenderers = GetComponentsInChildren<Renderer>();
            _visibility = 1;
        }

        // Update is called once per frame
        void Update()
        {

            _visibility = Mathf.Lerp(_visibility, 0f, 0.01f);
            var helpOnTeleportationArrivalIsVisible = _visibility > 0.001f;
            if (helpOnTeleportationArrivalIsVisible)
                RenderHelpOnTeleportationArrivalForCurrentVisibility();
            else
                Destroy(gameObject);
        }

        private void RenderHelpOnTeleportationArrivalForCurrentVisibility()
        {
            foreach (var r in _allRenderers)
            {
                var colorWithAlpha = new Color(_targetColor.r, _targetColor.g, _targetColor.b, _visibility);
                r.material.SetColor("_Color", colorWithAlpha);
                r.material.SetColor("_EmissionColor", colorWithAlpha);
            }
        }

        public void SetAnnotationData(Annotation annotation)
        {
            transform.position = new Vector3(annotation.ViewPortPosition.position.x, 0, annotation.ViewPortPosition.position.z);
        }

        private float _visibility = 0;
        private Renderer[] _allRenderers;
    }
}