using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMarkerDirectorInstance : CameraDirectorInstance
{
    private CameraDirectorMarker m_Marker = null;
    private Camera m_Owner = null;
    private Camera m_Mapper = null;
    private int m_OrderFalg = 0;

    public void InitData(CameraDirectorMarker marker, Camera owner, Camera mapper)
    {
        m_Marker = marker;
        m_Owner = owner;
        m_Mapper = mapper;
    }

    public override bool CanDelete
    {
        get
        {
            return true;
        }
    }

    public override int Order
    {
        get
        {
            return m_Marker.Order;
        }

        set
        {
            m_Marker.Order = value;
        }
    }

    public override bool CanSave
    {
        get
        {
            return true;
        }
    }

    public override Camera Camera
    {
        get
        {
            return m_Owner;
        }
    }

    public int OrderFalg
    {
        get
        {
            return m_OrderFalg;
        }
    }

    public CameraDirectorMarker Marker
    {
        get
        {
            return m_Marker;
        }
    }

    public override bool IsDynamic
    {
        get
        {
            return m_Marker.IsDynamic;
        }

        set
        {
            m_Marker.IsDynamic = value;
        }
    }

    public override void Save()
    {
        if (m_Mapper != null && m_Owner != null)
        {
            m_Mapper.fieldOfView = m_Owner.fieldOfView;
            m_Mapper.transform.position = m_Owner.transform.position;
            m_Mapper.transform.rotation = m_Owner.transform.rotation;
        }
    }

    public override void Reset()
    {
        if (m_Mapper != null && m_Owner != null)
        {
            m_Owner.fieldOfView = m_Mapper.fieldOfView;
            m_Owner.transform.position = m_Mapper.transform.position;
            m_Owner.transform.rotation = m_Mapper.transform.rotation;
        }
    }

    public void DelectMapper()
    {
        if(m_Mapper != null)
        {
            GameObject.DestroyImmediate(m_Mapper.gameObject);
            m_Mapper = null;
        }
    }

    public override void Up(float speed)
    {
        transform.position += transform.up * speed;
    }

    public override void Forward(float speed)
    {
        transform.position += transform.forward * speed;
    }

    public override void Right(float speed)
    {
        transform.position += transform.right * speed;
    }

    public override void Yaw(float speed)
    {
        Vector3 rot = transform.localEulerAngles;
        transform.localEulerAngles = new Vector3(rot.x, rot.y + speed, rot.z);
    }

    public override void Pitch(float speed)
    {
        Vector3 rot = transform.localEulerAngles;
        transform.localEulerAngles = new Vector3(rot.x + speed, rot.y, rot.z);
    }

    public override void Roll(float speed)
    {
        Vector3 rot = transform.localEulerAngles;
        transform.localEulerAngles = new Vector3(rot.x, rot.y, rot.z + speed);
    }

    public override void ReOrder(int flag)
    {
        m_OrderFalg = flag;
    }

    public override void HorizontalAdjust()
    {
        Vector3 rot = m_Owner.transform.eulerAngles;
        rot.z = 0;
        m_Owner.transform.eulerAngles = rot;
        Save();
    }
}

