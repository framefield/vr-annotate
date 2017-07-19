using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(SetRenderQueue))]
public class Highlight : MonoBehaviour
{

    private bool _meshesWereInit;

    Color _lineColor;
    Color _fillColor;

    private Matrix4x4 _assetTransform;

    void Start()
    {
        _assetTransform = transform.parent.parent.worldToLocalMatrix;
        if (!_meshesWereInit)
            InitMeshes();

        _renderer = GetComponent<Renderer>();
        _lineColor = _renderer.material.GetColor("_LineColor");
        _fillColor = _renderer.material.GetColor("_FillColor");
    }

    void Update()
    {
        var hover = _state == State.Default ? 0f : 1f;
        //var damped = Mathf.Lerp(hover, _hoverFactorDamped, 0.85f);
        var damped = hover;
        if (damped == _hoverFactorDamped)
            return;

        _hoverFactorDamped = damped;

        //_hoverFactorDamped = Mathf.Lerp(hover, _hoverFactorDamped, 0.8f);

        if (_hoverFactorDamped > 0.06f)
        {
            if (!_renderer.enabled)
                _renderer.enabled = true;

            if (_hoverFactorDamped > 0.97f)
                _hoverFactorDamped = 1;

            _renderer.material.SetColor("_LineColor", new Color(_lineColor.r, _lineColor.g, _lineColor.b, _lineColor.a * _hoverFactorDamped));
            _renderer.material.SetColor("_FillColor", new Color(_fillColor.r, _fillColor.g, _fillColor.b, _fillColor.a * _hoverFactorDamped));
        }
        else
        {
            if (_renderer.enabled)
                _renderer.enabled = false;
        }
    }

    public void InitMeshes()
    {
        var meshFilters = transform.parent.GetComponentsInChildren<MeshFilter>();
        var combine = new CombineInstance[meshFilters.Length];
        for (int i = 0; i < combine.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = _assetTransform * meshFilters[i].transform.localToWorldMatrix;
        }
        _meshFilter = GetComponent<MeshFilter>();
        _meshFilter.sharedMesh = new Mesh();
        _meshFilter.sharedMesh.CombineMeshes(combine);
        _meshesWereInit = true;
    }

    public void StartHovering()
    {
        _state = State.Hover;
    }

    public void EndHovering()
    {
        _state = State.Default;
    }

    private Color _originalEmissionColor;
    private Renderer _renderer;
    private MeshFilter _meshFilter;
    private Vector3 _cutHighlightPosition;
    private float _timeHoverStarted;
    private float _timeHoverEnded;
    private float _hoverFactorDamped = 2;
    // to trigger initial update

    private State _state;

    enum State
    {
        Default = 0,
        Hover,
        PointerCaptured,
    }
}
