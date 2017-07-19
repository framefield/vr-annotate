using UnityEngine;

namespace ff.utils.rendering
{
    public class SetMipmapBias : MonoBehaviour
    {
        public float mipMapBias = -0.5f;

        void Start()
        {
            foreach (MeshRenderer r in GetComponentsInChildren<MeshRenderer>())
            {
                r.material.mainTexture.mipMapBias = mipMapBias;
            }
        }
    }
}
