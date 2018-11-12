using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class BlendYController:IController
{
    public enum ControllerDirection
    {
        Top = 0,
        Bottom,
        Count
    }

    public float upSliderValue = -1.0f;
    public float downSliderValue = 1.0f;

    /// <summary>
    /// the name of this controller
    /// </summary>
    [SerializeField]
    public string controllerName = "";

    private bool isSelected = false;
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
    public Vector2 windowSize = new Vector2(220, 220);

    /// <summary>
	/// The blendSharps indeces in Sharps. 
    /// the index is top, left.
	/// </summary>
	[SerializeField]
    public List<int> blendShapeIndexs = new List<int>();

    public BlendYController()
    {
        blendShapeIndexs.AddRange(new int[2] { -1, -1});
        GetIsSelect = false;
    }

    public int top
    {
        get { return blendShapeIndexs[0]; }
        set { blendShapeIndexs[0] = value; }
    }

    public int bottom
    {
        get { return blendShapeIndexs[1]; }
        set { blendShapeIndexs[1] = value; }
    }

    public bool GetIsSelect
    {
        set { isSelected = value; }
        get { return isSelected;  }
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
            case ControllerDirection.Top:
                if (sliderValue < 0)
                    value = float.PositiveInfinity;
                else
                    value = Mathf.Abs(sliderValue) * 100;
                break;

            case ControllerDirection.Bottom:
                if (sliderValue > 0)
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

    public string GetControllerName()
    {
        return controllerName;
    }
}


    
