using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Slate
{

    [InitializeOnLoad]
    public class SlateExtensions
    {
        private static SlateExtensions m_Instance = null;
        public static SlateExtensions Instance
        {
            get
            {
                return m_Instance;
            }
        }

        static SlateExtensions()
        {
            m_Instance = new SlateExtensions();
            m_Instance.OnInitSlateExtensions();
        }

        private List<IScreenEventProcessor> m_ScreenEventProcessorList = new List<IScreenEventProcessor>();
        private SlateRecordUtility m_SlateRecordUtility = null;

        public SlateRecordUtility RecordUtility 
        {
            get
            {
                return m_SlateRecordUtility;
            }
        }

        private SlateExtensions()
        {
            EditorApplication.update += OnInitSlateExtensions;
        }

        private void OnInitSlateExtensions()
        {
            if(m_SlateRecordUtility == null)
            {
                // IScreenEventProcessor
                {
                    foreach (System.Type type in ReflectionTools.GetDerivedTypesOf(typeof(IScreenEventProcessor)))
                    {
                        if (type.GetCustomAttributes(typeof(System.ObsoleteAttribute), true).FirstOrDefault() != null)
                        {
                            continue;
                        } 

                        m_ScreenEventProcessorList.Add((IScreenEventProcessor)System.Activator.CreateInstance(type));
                    }
                }

                m_SlateRecordUtility = new SlateRecordUtility();   
            }
            
            EditorApplication.update -= OnInitSlateExtensions;
        }

        public bool ProcessScreenEvent()
        {
            foreach(IScreenEventProcessor processor in m_ScreenEventProcessorList)
            {
                if(processor.ProcessEvent())
                {
                    return true;
                }
            }

            return false;
        }
    }

}

