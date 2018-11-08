using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CameraDirectorInstance : MonoBehaviour
{
    public virtual Camera Camera
    {
        get
        {
            return null;
        }
    }

    public virtual bool CanDelete
    {
        get
        {
            return false;
        }
    }

    public virtual bool CanSave
    {
        get
        {
            return false;
        }
    }

    public virtual int Order
    {
        get
        {
            return -1;
        }

        set
        {

        }
    }

    public virtual bool IsFixed
    {
        get
        {
            return true;
        }
    }

    public virtual bool IsDynamic
    {
        get
        {
            return true;
        }

        set
        {

        }
    }

    public virtual byte[] CameraData
    {
        get
        {
            return null;
        }
    }

    public virtual long CameraTime
    {
        get
        {
            return 0;
        }
    }

    public virtual bool AutoSmooth
    {
        get
        {
            return false;
        }
    }


    public virtual void Save()
    {
    }

    public virtual void Reset()
    {
    }

    public virtual void HorizontalAdjust()
    {

    }


    public virtual void Up(float speed)
    {

    }

    public virtual void Forward(float speed)
    {

    }

    public virtual void Right(float speed)
    {

    }

    public virtual void Yaw(float speed)
    {

    }

    public virtual void Pitch(float speed)
    {

    }

    public virtual void Roll(float speed)
    {

    }

    public virtual void ReOrder(int order)
    {
        
    }

    public virtual void SetRenderTargetSize(int w, int h)
    {
        Camera.targetTexture = new RenderTexture(w, h, 24);
    }

    public virtual void UpdateDirector()
    {

    }
}

