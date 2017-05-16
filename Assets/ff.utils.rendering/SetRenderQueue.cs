using UnityEngine;

[AddComponentMenu("Rendering/SetRenderQueue")]

public class SetRenderQueue : MonoBehaviour
{
    [SerializeField]
    protected int[] m_queues = new int[]{ 1999 };
	private  Material[] _materials;

    protected void Awake()
    {
		var renderer = GetComponent<Renderer>();
		if(renderer == null) {
			//Debug.Log("Warning: GameObject is missing Renderer to set render queue" + this.name);
			return;
		}

        _materials = GetComponent<Renderer>().materials;
        for (int i = 0; i < _materials.Length && i < m_queues.Length; ++i)
        {
            _materials[i].renderQueue = m_queues[i];
        }
    }

	void Update() {
		if( m_queues[0] != _firstRenderQueue) {
			//Debug.Log("Updating Render Queue to:" + m_queues[0]);
			_firstRenderQueue = m_queues[0];
			_materials[0].renderQueue = _firstRenderQueue ;
		}		
	}

	int _firstRenderQueue = -1;

}