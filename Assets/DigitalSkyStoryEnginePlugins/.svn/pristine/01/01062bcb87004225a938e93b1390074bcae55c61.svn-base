using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DigitalSky.Recorder
{
    public abstract class RecorderManager:  IRecorderManager
    {
        protected string m_recordSavePath;

        public virtual string Device { get; set; }
        public virtual float AnimationFrameFPS { get; set; }

        public static RecorderManager Instance
        {
            get { return Singleton<RecorderManager>.Instance; }
        }
        /// <summary>
        /// 录制配置相关接口
        /// </summary>
        /// <returns></returns>
        public virtual string InitSavePath()
        {
            return string.Empty;
        }

        public virtual void SetRecordSavePath(string value)
        {
            m_recordSavePath = value;
        }

        public virtual bool CanRecord()
        {
            return true;
        }

        public virtual void SaveConfig()
        {
        }

        public virtual void LoadConfig()
        {
        }

        public virtual void Init()
        {

        }

        public virtual void Clear()
        {

        }

        public virtual void AddObject(GameObject obj, bool needRecord, GameObject hipsObj = null)
        {
        }

        public virtual void RemoveObject(object obj)
        {

        }

        public virtual void OnUpdate()
        {
        }

        public virtual void Destroy()
        {

        }

        public virtual void OnDestroy()
        {
           
        }

        public virtual void SetObjRecordEnalbe(GameObject obj, bool value)
        {

        }
        /// <summary>
        /// / GUI显示相关接口
        /// </summary>

        public virtual void RenderConfigObjectItem(ConfigComponent config, GameObject selectobj, Rect rc)
        {
           
        }

        public virtual void RenderGeneraConifg(Color color, Color _buttonColor)
        {
           
        }

        

        public virtual string GetRecordSavePath()
        {
            return m_recordSavePath;
        }
        /// <summary>
        /// 设备数据链接相关接口
        /// </summary>
        /// <returns></returns>
        public virtual bool IsConnected()
        {
            return false;
        }

        public virtual void StartConnect()
        {
            
        }

        public virtual void StopConnect()
        {
            
        }

        public virtual void UpdateConnect()
        {
            
        }

        public virtual void OnNetUpdate()
        {

        }

        public virtual void Cleanup()
        {
            
        }

        /// <summary>
        /// /录制相关接口
        /// </summary>
        public virtual void StartRecord(float startTime)
        {
            
        }
        public virtual void StopRecord(Slate.Cutscene cutscene, float startTime, float endTime, int? recordIdx = null)
        {
            
        }
        public virtual void Record(float currentTime, float totalTime)
        {
            
        }

        
    }
}
