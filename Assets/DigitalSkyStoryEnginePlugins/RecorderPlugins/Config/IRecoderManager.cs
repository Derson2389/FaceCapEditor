using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace DigitalSky.Recorder
{
    public interface IRecorderManager
    {
        void SetRecordSavePath(string value);
        string GetRecordSavePath();
        string InitSavePath();

        void RenderGeneraConifg(Color color, Color buttonColor);
        void RenderConfigObjectItem(ConfigComponent config, GameObject selectobj, Rect rc);

        bool CanRecord ();
        void Clear();
        void Init();
        void OnUpdate();
        void Destroy();
    }
}

