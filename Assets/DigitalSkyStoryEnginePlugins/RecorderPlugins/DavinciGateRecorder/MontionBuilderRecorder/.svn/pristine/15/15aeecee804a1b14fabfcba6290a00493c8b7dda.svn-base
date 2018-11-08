using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class MontionRecorderServer : MBR.NetworkServer
{
    private static MontionRecorderServer m_Instance = new MontionRecorderServer();
    private Dictionary<int, MBRootNodeInfo> m_MBNodeDictionary = new Dictionary<int, MBRootNodeInfo>();

    private List<MBRootNodeInfo> m_CameraList = new List<MBRootNodeInfo>();
    private List<MBRootNodeInfo> m_ItemList = new List<MBRootNodeInfo>();
    private List<MBRootNodeInfo> m_CharacterList = new List<MBRootNodeInfo>();
    private List<MBRootNodeInfo> m_MBObjectList = new List<MBRootNodeInfo>();


    private int m_CurrentNodeIndex = 0;

    public delegate void OnNodeListChangedDelegate();
    public OnNodeListChangedDelegate OnNodeListChanged;
    
    public static MontionRecorderServer Instance
    {
        get
        {
            return m_Instance;
        }
    }

    public List<MBRootNodeInfo> CharacterList
    {
        get
        {
            return m_CharacterList;
        }
    }

    public List<MBRootNodeInfo> ItemList
    {
        get
        {
            return m_ItemList;
        }
    }

    public List<MBRootNodeInfo> MBObjectList
    {
        get
        {
            return m_MBObjectList;
        }
    }

    public List<MBRootNodeInfo> CameraList
    {
        get
        {
            return m_CameraList;
        }
    }

    public MontionRecorderServer()
    {
        m_ListenPort = 30001;

        m_RecvMessageDictionary.Add((int)MontionRecorderServerPacketId.Id_CreateNodeTree, typeof(MBR.Recv_CreateNodeTree));
        m_RecvMessageDictionary.Add((int)MontionRecorderServerPacketId.Id_UpdateNodeTree, typeof(MBR.Recv_UpdateNodeTree));
    }

    public Dictionary<int, MBRootNodeInfo> GetMBNodeDictionary()
    {
        return m_MBNodeDictionary;
    }

    protected override void StopCleanup()
    {
        m_MBNodeDictionary.Clear();
        m_CameraList.Clear();
        m_CharacterList.Clear();
        m_ItemList.Clear();
        m_MBObjectList.Clear();
        m_CurrentNodeIndex = 0;
    }

    public void SetListenPort(int port)
    {
        m_ListenPort = port;
    }

    public int GetNodeIndex()
    {
        return m_CurrentNodeIndex++;
    }

    protected override void RemoveClient(Socket socket)
    {
        List<int> willRemoveList = new List<int>();
        foreach(var info in m_MBNodeDictionary)
        {
            if(info.Value.GetSocket().Equals(socket))
            {
                willRemoveList.Add(info.Key);
            }
        }

        if(willRemoveList.Count == 0)
        {
            return;
        }

        foreach(int id in willRemoveList)
        {
            UnregisterNodeInfo(id);
        }

        if (OnNodeListChanged != null)
        {
            OnNodeListChanged();
        }
    }

    public void RegisterNodeInfo(MBRootNodeInfo nodeInfo)
    {
        m_MBNodeDictionary.Add(nodeInfo.GetId(), nodeInfo);
        switch (nodeInfo.GetNodeType())
        {
            case 0:
                m_CharacterList.Add(nodeInfo);
                m_MBObjectList.Add(nodeInfo);
                break;
            case 1:
                m_CameraList.Add(nodeInfo);
                break;
            case 2:
                m_ItemList.Add(nodeInfo);
                m_MBObjectList.Add(nodeInfo);
                break;
            default:
                break;
        }

        if(OnNodeListChanged != null)
        {
            OnNodeListChanged();
        }
    }

    public void UnregisterNodeInfo(int id)
    {
        MBRootNodeInfo nodeInfo = null;
        if (m_MBNodeDictionary.TryGetValue(id, out nodeInfo))
        {
            switch (nodeInfo.GetNodeType())
            {
                case 0:
                    m_CharacterList.Remove(nodeInfo);
                    m_MBObjectList.Remove(nodeInfo);
                    break;
                case 1:
                    m_CameraList.Remove(nodeInfo);
                    break;
                case 2:
                    m_ItemList.Remove(nodeInfo);
                    m_MBObjectList.Remove(nodeInfo);
                    break;
                default:
                    break;
            }

            m_MBNodeDictionary.Remove(id);
            if (OnNodeListChanged != null)
            {
                OnNodeListChanged();
            }
        }
    }

    public MBRootNodeInfo GetNodeInfo(int id)
    {
        MBRootNodeInfo nodeInfo = null;
        if(m_MBNodeDictionary.TryGetValue(id, out nodeInfo))
        {
            return nodeInfo;
        }
        else
        {
            return null;
        }
    }

    private long m_LastUpdateTime;
    protected override void ProcessNetThread()
    {
        long delta = System.DateTime.Now.ToFileTime() - m_LastUpdateTime;
        m_LastUpdateTime = System.DateTime.Now.ToFileTime();
        foreach (MBR.NetworkConnectClient connection in m_Connections.Values)
        {
            connection.IdleTime += (float)delta / 1000000;
            if (connection.IdleTime >= 2.0f)
            {
                connection.IdleTime = 0.0f;
                connection.AddSendMessage(new MBR.Send_DockHeart());
            }
        }
    }
}
