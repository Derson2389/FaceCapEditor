using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class BlendShape : System.Object
{
    /// <summary>
    /// The blendable indeces.
    /// </summary>
    [SerializeField]
    public int blendableIndex;

    /// <summary>
    /// The blendable name. Used for re-syncing
    /// </summary>
    [SerializeField]
    public string blendableName;

    /// <summary>
    /// The associated weight.
    /// </summary>
    [SerializeField]
    public float weight;
}

