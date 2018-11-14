using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class BlendGridController: IController
{
    public enum ControllerDirection
    {
        Top = 0,
        Left,
        Bottom,
        Right,
        Count
    }

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

    public float upValue = -1.0f;
    public float downValue = 1.0f;
    public float leftValue = -1.0f;
    public float rightValue = 1.0f;

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
    /// the index is top, left, bottom, right.
	/// </summary>
	[SerializeField]
    public List<int> blendShapeIndexs = new List<int>();

    public BlendGridController()
    {
        blendShapeIndexs.AddRange(new int[4] { -1, -1, -1, -1 });
        GetIsSelect = false;
    }

    public int top
    {
        get { return blendShapeIndexs[0]; }
        set { blendShapeIndexs[0] = value; }
    }


    public int left
    {
        get { return blendShapeIndexs[1]; }
        set { blendShapeIndexs[1] = value; }
    }

    public int bottom
    {
        get { return blendShapeIndexs[2]; }
        set { blendShapeIndexs[2] = value; }
    }

    public int right
    {
        get { return blendShapeIndexs[3]; } 
        set { blendShapeIndexs[3] = value; }
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
    public static float GetWeightFromPosition(ControllerDirection dir, Vector2 pos)
    {
        float value = 0f;

        // 当不在象限里面，统统返回PositiveInfinity， 对于值为PositiveInfinity的，应该忽略掉，使用原始shape里面的数值
        switch (dir)
        {
            case ControllerDirection.Top:
                if (pos.y > 0f)
                    value = float.PositiveInfinity;
                else
                    value = Mathf.Abs(pos.y) * 100;
                break;

            case ControllerDirection.Bottom:
                if (pos.y < 0f)
                    value = float.PositiveInfinity;
                else
                    value = Mathf.Abs(pos.y) * 100;
                break;

            case ControllerDirection.Left:
                if (pos.x > 0f)
                    value = float.PositiveInfinity;
                else
                    value = Mathf.Abs(pos.x) * 100;
                break;

            case ControllerDirection.Right:
                if (pos.x < 0f)
                    value = float.PositiveInfinity;
                else
                    value = Mathf.Abs(pos.x) * 100;
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


    
