using UnityEngine;


namespace ff.utils
{
    public static class Helpers
    {
        public static void FaceCameraAndKeepSize(Transform objectTransform, float defaultSize)
        {
            var distance = Vector3.Distance(objectTransform.position, Camera.main.transform.position);
            var scaleByDistance = Mathf.Sqrt(distance / 2) * defaultSize;
            objectTransform.localScale = Vector3.one * scaleByDistance;

            var d = objectTransform.position - (Camera.main.transform.position - objectTransform.position);
            objectTransform.LookAt(d);
        }
    }

}
