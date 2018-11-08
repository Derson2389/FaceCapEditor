/*
Copyright 2015 Pim de Witte All Rights Reserved.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;

/// Author: Pim de Witte (pimdewitte.com) and contributors
/// <summary>
/// A thread-safe class which holds a queue with actions to execute on the next Update() method. It can be used to make calls to the main thread for
/// things such as UI Manipulation in Unity. It was developed for use in combination with the Firebase Unity plugin, which uses separate threads for event handling
/// </summary>
public class UnityMainThreadDispatcher : MonoBehaviour {
    public class Job
    {
        public Job(IEnumerator f) {
            func = f;
            sem = new Semaphore(0, 1);
        }

        public IEnumerator func { get; set; }
        public Semaphore sem { get; set; }
    }

    private static readonly Queue<Job> _executionQueue = new Queue<Job>();
    
	/// <summary>
	/// Locks the queue and adds the IEnumerator to the queue
	/// </summary>
	/// <param name="action">IEnumerator function that will be executed from the main thread.</param>
	public void Enqueue(IEnumerator action) {
        lock (_executionQueue) {
			_executionQueue.Enqueue(new Job(action));
		}
	}

    /// <summary>
	/// Locks the queue and adds the IEnumerator to the queue
	/// </summary>
	/// <param name="action">IEnumerator function that will be executed from the main thread.</param>
	public void EnqueueWait(IEnumerator action)
    {
        Job j = new Job(action);
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(j);
        }

        j.sem.WaitOne();
    }

    /// <summary>
    /// Locks the queue and adds the Action to the queue
    /// </summary>
    /// <param name="action">function that will be executed from the main thread.</param>
    public void Enqueue(Action action) {
		Enqueue(_instance.ActionWrapper(action));
	}


	/// <summary>
	/// This ensures that there's exactly one UnityMainThreadDispatcher in every scene, so the singleton will exist no matter which scene you play from.
	/// </summary>

	public void Update() {
		lock(_executionQueue) {
			while (_executionQueue.Count > 0) {
                Job j = _executionQueue.Dequeue();

#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    CoroutineManager.StartCoroutine(j.func);
                }
                else
                {
                    StartCoroutine(j.func);
                }
#else
                StartCoroutine(j.func);
#endif

                j.sem.Release();
			}
		}
	}

	IEnumerator ActionWrapper(Action a) {
		a();
		yield return null;
	}

    private static UnityMainThreadDispatcher _instance = null;

    public static bool Exists()
    {
        return _instance != null;
    }

    public static UnityMainThreadDispatcher Instance()
    {
        if (!Exists())
        {
			//GameObject obj = new GameObject ("MainThreadDispatcher");
			//_instance = obj.AddComponent<UnityMainThreadDispatcher>();
			//DontDestroyOnLoad(obj);
            throw new Exception("UnityMainThreadDispatcher could not find the UnityMainThreadDispatcher object. Please ensure you have added the MainThreadExecutor Prefab to your scene.");
        }
        return _instance;
    }


    public void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
    }

    void OnDestroy()
    {
        _instance = null;
    }
}

