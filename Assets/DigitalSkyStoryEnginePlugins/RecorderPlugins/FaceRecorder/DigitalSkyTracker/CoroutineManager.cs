using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public static class CoroutineManager
{
    public static IEnumerator StartCoroutine(IEnumerator iterator)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            return StartEditorCoroutine(iterator);
        }
#endif
        return null;
    }

    public static float deltaTime
    {
        get
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return (float)_deltaTime;
#endif
            return Time.deltaTime;
        }
    }

    public static float timeSinceStartup
    {
        get
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return (float)EditorApplication.timeSinceStartup;
#endif
            return Time.realtimeSinceStartup;
        }
    }

#if UNITY_EDITOR
    private class EditorCoroutine : IEnumerator
    {
        private Stack<IEnumerator> _executionStack;

        public EditorCoroutine(IEnumerator iterator)
        {
            _executionStack = new Stack<IEnumerator>();
            _executionStack.Push(iterator);
        }

        public bool MoveNext()
        {
            IEnumerator i = _executionStack.Peek();

            while (true)
            {
                if (i.MoveNext())
                {
                    object result = i.Current;
                    if (result != null && result is IEnumerator)
                    {
                        _executionStack.Push((IEnumerator)result);
                        i = (IEnumerator)result;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    if (_executionStack.Count > 1)
                    {
                        _executionStack.Pop();
                        i = _executionStack.Peek();
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            /*if (i.MoveNext())
            {
                object result = i.Current;
                if (result != null && result is IEnumerator)
                {
                    this.executionStack.Push((IEnumerator)result);
                }

                return true;
            }
            else
            {
                if (this.executionStack.Count > 1)
                {
                    this.executionStack.Pop();
                    return true;
                }
            }

            return false;*/
        }

        public void Reset()
        {
            throw new System.NotSupportedException("This Operation Is Not Supported.");
        }

        public object Current
        {
            get { return _executionStack.Peek().Current; }
        }

        public bool Find(IEnumerator iterator)
        {
            return _executionStack.Contains(iterator);
        }
    }

    private static List<EditorCoroutine> _editorCoroutineList;
    private static List<IEnumerator> _buffer;

    private static double _currentTime = 0f;
    private static double _deltaTime = 0f;

    static IEnumerator StartEditorCoroutine(IEnumerator iterator)
    {
        if (_editorCoroutineList == null)
        {
            _editorCoroutineList = new List<EditorCoroutine>();
        }
        if (_buffer == null)
        {
            _buffer = new List<IEnumerator>();
        }
        if (_editorCoroutineList.Count == 0)
        {
            _currentTime = EditorApplication.timeSinceStartup;
            _deltaTime = 0f;
            EditorApplication.update += Update;
        }

        // add iterator to buffer first  
        //_buffer.Add(iterator);

        EditorCoroutine newEditorCoroutine = new EditorCoroutine(iterator);
        if (newEditorCoroutine.MoveNext())
        {
            // Added this as new EditorCoroutine  
            _editorCoroutineList.Add(newEditorCoroutine);
        }

        return iterator;
    }

    private static bool Find(IEnumerator iterator)
    {
        // If this iterator is already added  
        // Then ignore it this time  
        foreach (EditorCoroutine editorCoroutine in _editorCoroutineList)
        {
            if (editorCoroutine.Find(iterator))
            {
                return true;
            }
        }

        return false;
    }

    private static void Update()
    {
        _deltaTime = (EditorApplication.timeSinceStartup - _currentTime) * Time.timeScale;
        _currentTime = EditorApplication.timeSinceStartup;

        // EditorCoroutine execution may append new iterators to buffer  
        // Therefore we should run EditorCoroutine first  
        _editorCoroutineList.RemoveAll
        (
            coroutine => { return coroutine.MoveNext() == false; }
        );

        // If we have iterators in buffer  
        /*if (_buffer.Count > 0)
        {
            foreach (IEnumerator iterator in _buffer)
            {
                // If this iterators not exists  
                if (!Find(iterator))
                {
                    // Added this as new EditorCoroutine  
                    _editorCoroutineList.Add(new EditorCoroutine(iterator));
                }
            }

            // Clear buffer  
            _buffer.Clear();
        }*/

        // If we have no running EditorCoroutine  
        // Stop calling update anymore  
        if (_editorCoroutineList.Count == 0)
        {
            _deltaTime = 0f;
            EditorApplication.update -= Update;
        }
    }
#endif
}
