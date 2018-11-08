using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ConfigAsset
{
    [SerializeField]
    private GameObject obj;
    public Texture icon;
    public bool bNeedRecord;
    public string objName = string.Empty;

    public ConfigAsset(GameObject _obj, Texture _icon, bool _bNeedRecord)
    {
        obj = _obj;
        icon = _icon;
        bNeedRecord = _bNeedRecord;
        objName = _obj.name;
    }

    public GameObject OBJ
    {
        get
        {
            if (obj != null)
                return obj;
            else
            {
                if (objName != string.Empty)
                {
                    GameObject goDummy = GameObject.Find(objName);
                    if (goDummy != null)
                    {
                        obj = goDummy;
                    }
                }          
                return obj;
            }
        }
    }
}

[ExecuteInEditMode]
public class ConfigComponent : MonoBehaviour {

    [SerializeField]
    private List<ConfigAsset> configAssetList = new List<ConfigAsset>();

    [SerializeField]
    private List<ConfigActor> actorRecordConfigList = new List<ConfigActor>();
#if UNITY_EDITOR

    public List<ConfigAsset> GetConfigAssetList()
    {
        return configAssetList;
    }

    public void ChangeRecord(int idx, bool value)
    {
        if (idx< configAssetList.Count)
        {
            configAssetList[idx].bNeedRecord = value;
        }       
    }

    public void DataInit(GameObject go, Texture icon)  
    {
        configAssetList.Add(new ConfigAsset(go, icon, true));

        var config = new ConfigActor(); 
        config.SynDashSekFile = Application.dataPath + "/DigitalSkyStoryEnginePlugins/ViconRecorder/SkeletonFiles/v853_Pair160_D0_T64_M24_Jig2.sk";
        actorRecordConfigList.Add(config);
    }

    public int GetIdxByObj(GameObject o)
    {
        int idx = 0;
        for (int i = 0; i < configAssetList.Count; i++)
        {
            if (configAssetList[i].OBJ == o)
            {
                idx = i;
            }
        }
        return idx;
    }

    public void RefreshActorConfig(GameObject o, ConfigActor newData)
    {
        int idx = GetIdxByObj(o);
        actorRecordConfigList[idx] = newData;
    }

    public ConfigActor GetActorConfigByObj(GameObject o)
    {
        int idx = GetIdxByObj(o);
        return actorRecordConfigList[idx];
    }

#endif
    

}
