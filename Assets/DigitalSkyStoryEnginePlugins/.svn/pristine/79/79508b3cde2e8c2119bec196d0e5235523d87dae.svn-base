using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConfigActor
{
    [SerializeField]
    private string actorMBName = string.Empty;
    [SerializeField]
    private int synDashIndex = -1;
    [SerializeField]
    private bool synBinder_LockState = false;
    [SerializeField]
    private string synDashSekFile = string.Empty;
    [SerializeField]
    private TextAsset emotionData = null;
    [SerializeField]
    private int emotionSelectedIndex = 0;
    
    [SerializeField]
    private bool isUseOriginalFile = true;

    public ConfigActor()
    {
        
    }

    public bool IsUseOriginalFile
    {
        set { isUseOriginalFile = value; }
        get { return isUseOriginalFile;  }

    }

    public string ActorMBName
    {
        get { return actorMBName;  }
        set { actorMBName = value; }
    }
    
    public int SynDashIndex
    {
        get { return synDashIndex;  }
        set { synDashIndex = value; }
    }

    public bool SynBinderLockState
    {
        get { return synBinder_LockState;  }
        set { synBinder_LockState = value; }
    }

    public string SynDashSekFile
    {
        get { return synDashSekFile;  }
        set { synDashSekFile = value; }
    }

    public TextAsset EmotionData
    {
        set { emotionData = value; }
        get { return emotionData;  }    
    }

    public int EmotionSelectedIndex
    {
        set { emotionSelectedIndex = value; }
        get { return emotionSelectedIndex;  }
    } 

}


