using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class BlendXController
{
    public enum ControllerDirection
    {
        Left = 0,
        Right,
        Count
    }

    /// <summary>
    /// the name of this controller
    /// </summary>
    [SerializeField]
    public string controllerName = "";

    /// <summary>
    /// the index of this controller
    /// </summary>
    [SerializeField]
    public int controllerIndex = -1;

    /// <summary>
    /// the panel position in blend controller window
    /// </summary>
    [SerializeField]
    public Vector2 windowPosition = Vector2.zero;

    /// <summary>
    /// the panel size in blend controller window
    /// </summary>
    [SerializeField]
    public Vector2 windowSize = new Vector2(100, 20);

    /// <summary>
	/// The blendSharps indeces in Sharps. 
    /// the index is left, right.
	/// </summary>
	[SerializeField]
    public List<int> blendShapeIndexs = new List<int>();

    public BlendXController()
    {
        blendShapeIndexs.AddRange(new int[2] { -1, -1 });
    }

    public int left
    {
        get { return blendShapeIndexs[0]; }
        set { blendShapeIndexs[0] = value; }
    }
   
    public int right
    {
        get { return blendShapeIndexs[1]; }
        set { blendShapeIndexs[1] = value; }
    }

    /// <summary>
    /// get blend controller's weight from a 2D position
    /// </summary>
    /// <param name="dir">the direction in Blend Controller Window</param>
    /// <param name="pos">the vector2 normalized position in Blend Controller Window</param>
    /// <returns></returns>
    public static float GetWeightFromPosition(ControllerDirection dir, float sliderValue)
    {
        float value = 0f;

        switch (dir)
        {           
            case ControllerDirection.Left:
                if (sliderValue > 0f)
                    value = float.PositiveInfinity;
                else
                    value = Mathf.Abs(sliderValue) * 100;
                break;

            case ControllerDirection.Right:
                if (sliderValue < 0f)
                    value = float.PositiveInfinity;
                else
                    value = Mathf.Abs(sliderValue) * 100;
                break;    
        }

        return value;
    }

    public int GetBlendShapeIndex(ControllerDirection dir)
    {
        return blendShapeIndexs[(int)dir];
    }
}


    
