using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalSky.Tracker
{
    public class TrackRetargeter : MonoBehaviour
    {
        /// <summary>
        /// 重定向目标对象
        /// </summary>
        public string retargetId = "";

        /// <summary>
        /// 重定向目标对象
        /// </summary>
        public GameObject target;

        /// <summary>
        /// 是否初始化成功
        /// </summary>
        public virtual bool isInit
        {
            get { return false; }
        }

        /// <summary>
        /// 是否绑定成功
        /// </summary>
        public virtual bool isBinding
        {
            get { return false; }
        }

        /// <summary>
        /// 重定向绑定源数据
        /// </summary>
        protected object _bindingData;
        public virtual object bindingData
        {
            get { return _bindingData; }
            set { _bindingData = value; }
        }

        /// <summary>
        /// 重定向列表
        /// </summary>
        public virtual List<ITrackBinding> trackBindings
        {
            get { return new List<ITrackBinding>(); }
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual bool Init()
        {
            return false;
        }

        /// <summary>
        /// 更新Track数据到重定向对象
        /// </summary>
        public virtual void OnUpdateTrackDatas(ITracker tracker)
        {

        }

        /// <summary>
        /// 通过bindName查找TrackBinding对象
        /// </summary>
        public ITrackBinding FindTrackBindingByName(string bindName)
        {
            if (trackBindings == null)
                return null;

            for(int i = 0; i < trackBindings.Count; i++)
            {
                if (trackBindings[i].bindName == bindName)
                    return trackBindings[i];
            }

            return null;
        }

        /// <summary>
        /// 重置所有TrackingBinding为初始值
        /// </summary>
        public void ResetTrackBinding()
        {
            if (trackBindings == null)
                return;

            // Reset all to initial value (in case of reconnection).
            if (trackBindings.Count > 0)
            {
                for (int i = 0; i < trackBindings.Count; i++)
                {
                    ITrackBinding binding = trackBindings[i];
                    if (binding.target != null)
                        binding.target.Reset();
                }
            }
        }

        /// <summary>
        /// 生成重定向对象关系表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual bool CreateTrackBinding()
        {
            return false;
        }

        /// <summary>
        /// 生成重定向对象关系表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual bool CreateTrackBinding(object data)
        {
            return false;
        }

        /// <summary>
        /// 解析Visage重定向关系数据
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        protected List<ITrackBinding> ParseBinding(TextAsset configuration)
        {
            List<ITrackBinding> bindings = new List<ITrackBinding>();
            SkinnedMeshRenderer[] rigSkinnedMeshRenderers = target.GetComponentsInChildren<SkinnedMeshRenderer>(true);

            string text = configuration.text;
            string[] lines = text.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                // skip comments
                if (line.StartsWith("#") || line.Trim() == "")
                    continue;

                string[] values = line.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                string[] trimmedValues = new string[8];
                for (int i = 0; i < Mathf.Min(values.Length, 8); i++)
                {
                    // trim values
                    trimmedValues[i] = values[i].Trim();
                }

                // parse au name
                string au = trimmedValues[0];

                // parse blendshape identifier
                string blendshapeName = trimmedValues[1];

                TrackBindingTarget bindingTarget = CreateBindingTargetByName(rigSkinnedMeshRenderers, blendshapeName);
                if (bindingTarget == null)
                {
                    Debug.LogError("[TrackRetargeter.ParseBinding] -> Invalid blendshape_indentifier value ''" + blendshapeName + " in configuration '" + configuration.name + "'.");
                    return null;
                }

                // parse min limit
                float min = -1f;
                if (!float.TryParse(trimmedValues[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out min))
                {
                    Debug.LogError("[TrackRetargeter.ParseBinding] -> Invalid min_limit value in binding configuration '" + configuration.name + "'.");
                    return null;
                }

                // parse max limit
                float max = 1f;
                if (!float.TryParse(trimmedValues[3], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out max))
                {
                    Debug.LogError("[TrackRetargeter.ParseBinding] -> Invalid max_limit value in binding configuration '" + configuration.name + "'.");
                    return null;
                }

                // parse inverted
                bool inverted = false;
                if (!string.IsNullOrEmpty(trimmedValues[4]))
                    inverted = trimmedValues[4] == "1";

                // parse weight
                float weight = 1f;
                if (!string.IsNullOrEmpty(trimmedValues[5]))
                {
                    if (!float.TryParse(trimmedValues[5], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out weight))
                    {
                        Debug.LogError("[TrackRetargeter.ParseBinding] -> Invalid weight value in binding configuration '" + configuration.name + "'.");
                        return null;
                    }
                }

                // parse filter window
                int filterWindow = 6;
                if (!string.IsNullOrEmpty(trimmedValues[6]))
                {
                    if (!int.TryParse(trimmedValues[6], out filterWindow) || filterWindow < 0 || filterWindow > 16)
                    {
                        Debug.LogError("[TrackRetargeter.ParseBinding] -> Invalid filter_window value in binding configuration '" + configuration.name + "'.");
                        return null;
                    }
                }

                // parse filter amount
                float filterAmount = 0.3f;
                if (!string.IsNullOrEmpty(trimmedValues[7]))
                {
                    if (!float.TryParse(trimmedValues[7], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out filterAmount))
                    {
                        Debug.LogError("[TrackRetargeter.ParseBinding] -> Invalid filter_amount value in binding configuration '" + configuration.name + "'.");
                        return null;
                    }
                }

                // add new binding
                TrackBinding binding = new TrackBinding();

                // 接口必要信息
                binding.bindName = au;
                binding.bindWeight = weight;
                binding.filterSize = filterWindow;
                binding.limits = new Vector2(min, max);
                binding.target = bindingTarget;

                // 特定Tracker的补充信息
                binding.inverted = inverted;
                binding.filterConstant = filterAmount;

                binding.Init();
                bindings.Add(binding);
            }

            return bindings;
        }

        public static TrackBindingTarget CreateBindingTargetByName(SkinnedMeshRenderer[] rigSkinnedMeshRenderers, string bindName)
        {
            TrackBindingTarget bindingTarget = null;

            string[] blendShapeNames = bindName.Split(':');
            string blendShapeName = blendShapeNames.Length == 2 ? blendShapeNames[1] : blendShapeNames[0];

            // Iterate through the meshes and search for the blendshape.
            if (blendShapeNames.Length == 2)
            {
                // First search algorithm: exact name.
                foreach (SkinnedMeshRenderer m in rigSkinnedMeshRenderers)
                {
                    if (m.name != blendShapeNames[0])
                        continue;

                    int vBshapeIdx = m.sharedMesh.GetBlendShapeIndex(blendShapeNames[1]);
                    if (vBshapeIdx == -1)
                        continue;

                    // We have found the corresponding blendshape in the mesh. 
                    // Store the reference to constraint the blendshape and its index.
                    bindingTarget = new TrackBindingTarget(m.gameObject, vBshapeIdx);
                    break;
                }
            }
            else
            {
                foreach (SkinnedMeshRenderer m in rigSkinnedMeshRenderers)
                {
                    int vBshapeIdx = m.sharedMesh.GetBlendShapeIndex(blendShapeNames[0]);
                    if (vBshapeIdx == -1)
                        continue;

                    // We have found the corresponding blendshape in the mesh. 
                    // Store the reference to constraint the blendshape and its index.
                    bindingTarget = new TrackBindingTarget(m.gameObject, vBshapeIdx);
                    break;
                }
            }

            return bindingTarget;
        }
    }
}

