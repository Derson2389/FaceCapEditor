using UnityEngine;

namespace DStoryEngine
{
    [ExecuteInEditMode]
    public class CameraLookAt : MonoBehaviour
    {
        [SerializeField]
        public GameObject lookAtObject;

        [System.NonSerialized]
        private Camera cameraObject;

        private Camera CameraObject
        {
            get
            {
                if (cameraObject == null)
                {
                    cameraObject = gameObject.GetComponent<Camera>();
                }

                return cameraObject;
            }
        }

        private void Update()
        {
            if (lookAtObject == null)
            {
                return;
            }

            if (CameraObject == null)
            {
                return;
            }

            CameraObject.transform.LookAt(lookAtObject.transform);
        }
    }
}

