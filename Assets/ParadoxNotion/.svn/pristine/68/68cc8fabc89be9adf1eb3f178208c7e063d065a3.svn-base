#if UNITY_EDITOR

using System.Collections;
using UnityEngine;

namespace Slate {

    public class FixDeltaTime : MonoBehaviour {
    
        public float targetFramerate = 30.0f;
        private float originalMaxDelta;

        void OnEnable(){
        	originalMaxDelta = Time.maximumDeltaTime;
            ApplyFrameRate();
        }

        void Update(){
            ApplyFrameRate();
            StartCoroutine(Wait());
        }

        void OnDisable(){
        	Time.maximumDeltaTime = originalMaxDelta;
        }

        public IEnumerator Wait(){
            yield return new WaitForEndOfFrame();

            // wait until current dt reaches target dt
            float wt = Time.maximumDeltaTime;
            while (Time.realtimeSinceStartup - Time.unscaledTime < wt){
                System.Threading.Thread.Sleep(1);
            }
        }

        public void ApplyFrameRate(){
            Time.maximumDeltaTime = (1.0f / targetFramerate);
        }
    }
}

#endif