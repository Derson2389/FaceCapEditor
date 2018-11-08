using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using DigitalSky.Tracker;
using System;

namespace dxyz
{
    public class PrevizBindingData
    {
        public string data;
        public uint size;
    }

    public class PrevizTrackRetargeter : TrackRetargeter
    {

        /// <summary>
        /// 控制器配置信息
        /// </summary>
        public TextAsset controllerConfiguration;

        /// <summary>
        /// 是否初始化成功
        /// </summary>
        private bool mIsInit = false;
        public override bool isInit
        {
            get { return mIsInit; }
        }

        /// <summary>
        /// 是否绑定成功
        /// </summary>
        //private bool mIsBinding = false;
        public override bool isBinding
        {
            get { return trackBindings != null && trackBindings.Count > 0; }
        }

        /// <summary>
        /// 控制器处理逻辑器
        /// </summary>
        private PrevizCtrlHandler _ctrlHandler;
        public PrevizCtrlHandler CtrHandler
        {
            set { _ctrlHandler = value; }
            get { return _ctrlHandler;  }
        }

        /// <summary>
        /// 重定向列表
        /// </summary>
        private List<ITrackBinding> mTrackBindings;        
        public override List<ITrackBinding> trackBindings
        {
            get { return mTrackBindings; }
        }

        // 3D parameters
        public enum AxesOrder
        {
            XYZ, XZY,
            ZXY, ZYX,
            YZX, YXZ
        };

        // For bones
        public AxesOrder Axes;

        public bool InvertXAxis = false;
        public bool InvertYAxis = false;
        public bool InvertZAxis = false;
        public float Scale = 0.01f;

        // For Headbone
        public AxesOrder HeadAxes;

        public bool InvertHeadXAxis = false;
        public bool InvertHeadYAxis = false;
        public bool InvertHeadZAxis = false;

        // Private properties
        private Component[] mRigComponents;
        private SkinnedMeshRenderer[] mRigSkinnedMeshRenderers;

        // byte系数个数
        private uint mNbCoeff;

        // float系数个数
        private float[] mCoeffs;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public override bool Init()
        {
            if (controllerConfiguration == null)
            {
                //Debug.LogError("[ARFaceRetargeter.Init] -> init failed, bindingConfiguration is null");
                return false;
            }


            if (isInit)
                return true;

            if (target == null)
                target = gameObject;

			UnityMainThreadDispatcher dispatcher = target.AddComponent<UnityMainThreadDispatcher>();
			dispatcher.Awake ();

            // Retrieve all components attached to the object that own the script.
            mRigComponents = target.GetComponentsInChildren<Component>(true);

            // Retrieve all meshes on the current object.
            // We need to store them in global variable, because network thread is
            // not allowed to access Unity methods.
            mRigSkinnedMeshRenderers = target.GetComponentsInChildren<SkinnedMeshRenderer>(true);

            // 如果未找到SkinnedMeshRender组件，则初始化失败
            if (mRigSkinnedMeshRenderers == null || mRigSkinnedMeshRenderers.Length == 0)
                return false;

            string meshesStr = "";
            foreach (SkinnedMeshRenderer m in mRigSkinnedMeshRenderers)
            {
                meshesStr += (m.sharedMesh.name + " - ");
            }
            Debug.Log("[PrevizRetargeter.Init] -> Mesh(es) detected: " + System.Environment.NewLine + mRigSkinnedMeshRenderers.Length + meshesStr);

            mTrackBindings = new List<ITrackBinding>();
            mIsInit = true;

            return true;
        }

        /// <summary>
        /// 更新Track数据到重定向对象
        /// </summary>
        public override void OnUpdateTrackDatas(ITracker tracker)
        {
            PrevizTracker previzTracker = (PrevizTracker)tracker;

            if (mNbCoeff <= 0)
                return;

            System.Buffer.BlockCopy(previzTracker.coeffs, 0, mCoeffs, 0, (int)(mNbCoeff * sizeof(float)));

            Vector3 t = new Vector3();
            Vector3 r = new Vector3();

            for (int i = 0; i < mTrackBindings.Count; i++)
            {
                PrevizTrackBinding binding = (PrevizTrackBinding)mTrackBindings[i];

                PrevizTrackData data = new PrevizTrackData();
                data.bindName = binding.bindName;
                data.used = true;
                data.type = binding.type;

                if (binding.type == PrevizTrackBinding.Type.BLENDSHAPE)
                {
                    data.blendshapeValue = mCoeffs[binding.ecsCoeffIndex];
                }
                else if (binding.type == PrevizTrackBinding.Type.BONE || binding.type == PrevizTrackBinding.Type.HEAD_BONE)
                {
                    t.x = mCoeffs[binding.ecsCoeffIndex + 0];
                    t.y = mCoeffs[binding.ecsCoeffIndex + 1];
                    t.z = mCoeffs[binding.ecsCoeffIndex + 2];

                    // TODO: old code -> if (mEntityType[e].Equals("head"))
                    r.x = mCoeffs[binding.ecsCoeffIndex + 3];
                    r.y = mCoeffs[binding.ecsCoeffIndex + 4];
                    r.z = mCoeffs[binding.ecsCoeffIndex + 5];

                    AxesOrder vBoneAxe;
                    bool vInvertX;
                    bool vInvertY;
                    bool vInvertZ;

                    if (binding.type == PrevizTrackBinding.Type.BONE)
                    {
                        vBoneAxe = Axes;
                        vInvertX = InvertXAxis;
                        vInvertY = InvertYAxis;
                        vInvertZ = InvertZAxis;
                    }
                    else
                    {
                        vBoneAxe = HeadAxes;
                        vInvertX = InvertHeadXAxis;
                        vInvertY = InvertHeadYAxis;
                        vInvertZ = InvertHeadZAxis;
                    }


                    if (vInvertX)
                    {
                        t.x = t.x * -1;
                        r.x = r.x * -1;
                    }

                    if (vInvertY)
                    {
                        t.y = t.y * -1;
                        r.y = r.y * -1;
                    }

                    if (vInvertZ)
                    {
                        t.z = t.z * -1;
                        r.z = r.z * -1;
                    }

                    if (vBoneAxe.Equals(AxesOrder.XZY))
                    {
                        // swap Y-Z on translation.
                        float y = t.y;
                        t.y = t.z;
                        t.z = y;

                        // swap Y-Z on rotation.
                        y = r.y;
                        r.y = r.z;
                        r.z = y;
                    }

                    if (vBoneAxe.Equals(AxesOrder.ZYX))
                    {
                        // swap X-Z on translation.
                        float x = t.x;
                        t.x = t.z;
                        t.z = x;

                        // swap X-Z on rotation.
                        x = r.x;
                        r.x = r.z;
                        r.z = x;
                    }

                    if (vBoneAxe.Equals(AxesOrder.YXZ))
                    {
                        // swap X-Y on translation.
                        float x = t.x;
                        t.x = t.y;
                        t.y = x;

                        // swap X-Y on rotation.
                        x = r.x;
                        r.x = r.y;
                        r.y = x;
                    }

                    if (vBoneAxe.Equals(AxesOrder.ZXY))
                    {
                        float x = t.x;
                        float y = t.y;
                        float z = t.z;

                        t.x = z;
                        t.y = x;
                        t.z = y;

                        // swap Z-X-Y on rotation.
                        x = r.x;
                        r.x = r.z;
                        r.z = r.y;
                        r.y = x;
                    }

                    if (vBoneAxe.Equals(AxesOrder.YZX))
                    {
                        float x = t.x;
                        float y = t.y;
                        float z = t.z;

                        t.x = y;
                        t.y = z;
                        t.z = x;

                        // swap X-Y on rotation.
                        x = r.x;
                        r.x = r.y;
                        r.y = r.z;
                        r.z = x;
                    }

                    data.boneLocalRot = t * Scale;
                    data.boneLocalRot = r;
                }

                binding.OnUpdateTrackData(data);
            }

            //Debug.Log("[PrevizTrackRetargeter.OnUpdateTrackDatas] updated");
        }

        /// <summary>
        /// 根据重定向数据生成重定向关系表
        /// </summary>
        /// <param name="data">previs需要的retarget数据</param>
        /// <returns></returns>
        public override bool CreateTrackBinding(object data)
        {
            if (data == null)
                return false;

            bindingData = data;
            return CreateTrackBinding();
        }

        /// <summary>
        /// 根据重定向数据生成重定向关系表
        /// </summary>
        /// <returns></returns>
        public override bool CreateTrackBinding()
        {
            // Reset all bones positions to initial value (in case of reconnection).
            if (mTrackBindings.Count > 0)
            {
                for(int i = 0; i < mTrackBindings.Count; i++)
                {
                    ITrackBinding binding = mTrackBindings[i];
                    if (binding.target != null)
                        binding.target.Reset();
                }
                mTrackBindings.Clear();
            }
            if (CtrHandler != null)
            {
                CtrHandler = null;
            }

            PrevizBindingData bingingData = (PrevizBindingData)bindingData;
            mTrackBindings = ParseESCandAllocateBuffers(bingingData.data, bingingData.size);
            TextAsset data = controllerConfiguration;
            if (data != null)
            {
                CtrHandler = new PrevizCtrlHandler();
                CtrHandler.LoadConfig(data ,this.gameObject);
            }
            
            if (mTrackBindings == null || mTrackBindings.Count <= 0)
            {
                Debug.LogError("[PrevizRetargeter.CreateTrackBinding] -> 创建绑定关系失败, target: " + target.name);
                return false;
            }

            return true;
        }

       
        // Parse ESC.
        private List<ITrackBinding> ParseESCandAllocateBuffers(string iEsc, uint iNbCoeff)
        {
            List<ITrackBinding> bindings = new List<ITrackBinding>();

            // All of this code MUST be executed in the main thread. All variable
            // modified here can't be in race condition with update thus.
            XmlReader vRdr = XmlReader.Create(new StringReader(iEsc));

            //PrevizEcsEntity vCurrEntity = null;
            PrevizTrackBinding binding = null;

            int vCurrentBufIdx = 0;
            while (vRdr.Read())
            {
                if (!vRdr.IsStartElement())
                    continue;

                if (vRdr.Name == "entity")
                {
                    binding = new PrevizTrackBinding();
                    binding.bindName = vRdr.GetAttribute("name");
                    binding.ecsCoeffIndex = vCurrentBufIdx;

                    // Retrieve the Transform from the scene if it is either a bone
                    // or a blendshape.
                    if (vRdr.GetAttribute("class") == "component")
                    {
                        // B O N E S

                        // Head bone is handled differently.
                        if (vRdr.GetAttribute("solver").Equals("head"))
                            binding.type =  PrevizTrackBinding.Type.HEAD_BONE;
                        else
                            binding.type = PrevizTrackBinding.Type.BONE;

                        vCurrentBufIdx += 9;

                        // Search the ESC bone into the current script's attached 
                        // GameObject.
                        foreach (Component cp in mRigComponents)
                        {
                            if (cp.name == binding.bindName)
                            {
                                //vCurrEntity.bone_transform = cp.gameObject.transform;

                                binding.target = new TrackBindingTarget(cp.gameObject, cp.gameObject.transform);
                                break;
                            }
                        }

                        if(binding.target == null)
                        {
                            binding.type = PrevizTrackBinding.Type.INVALID;
                            Debug.LogWarning("[PrevizRetargeter.ParseESCandAllocateBuffers]: can't find the bone " + binding.bindName + " in the object " + this.gameObject.name);
                        }
                    }
                    else if (vRdr.GetAttribute("class") == "blendshapetarget")
                    {
                        vCurrentBufIdx += 1;

                        binding.type = PrevizTrackBinding.Type.BLENDSHAPE;
                        // Try to parse the blendshape name. ESC style is
                        // BSHAPE_SUBNAME[BSHAP_NAME]
                        // e.g: level3_Layers_38_Blendshape[grandyeuxD]
                        // The @ describe a C# verbatim string.
                        Match result = Regex.Match(binding.bindName, @"([^\[]*)\[([^\]]+)");

                        if (result.Success)
                        {
                            // Generate the shape name according to Unity 
                            // syntax.
                            // Groups[0] contain the original pattern without
                            // groups. Groups are starting from 1.
                            string vGroupName = result.Groups[1].Value;
							string vBshapeName = result.Groups[2].Value.Substring(0, result.Groups[2].Value.Length);
                            string vUnityBShapeName = vGroupName + "." + vBshapeName;
                            //Debug.Log(vUnityBShapeName);

                            // We do proceed with 2 search algorithm for the blendshapes.
                            // The second algorithm is for some bugs with maya export.

                            // Iterate through the meshes and search for the
                            // blendshape.

                            // First search algorithm: exact name.
                            foreach (SkinnedMeshRenderer m in mRigSkinnedMeshRenderers)
                            {
                                int vBshapeIdx =
                                    m.sharedMesh.GetBlendShapeIndex(vUnityBShapeName);

                                if (vBshapeIdx == -1)
                                    continue;

                                // We have found the corresponding blendshape in the mesh. 
                                // Store the reference to constraint the blendshape and its index.
                                binding.target = new TrackBindingTarget(m.gameObject, vBshapeIdx);

                                break;
                            }

                            // 2nd algorithm
                            // If first search algorithm didn't work. Try to find
                            // the blendshape without the group name.
                            if(binding.target == null)
                            {
                                foreach (SkinnedMeshRenderer m in mRigSkinnedMeshRenderers)
                                {
                                    int vBshapeIdx =
                                        m.sharedMesh.GetBlendShapeIndex(vBshapeName);

                                    if (vBshapeIdx == -1)
                                        continue;

                                    // We have found the corresponding blendshape in the mesh. 
                                    // Store the reference to constraint the blendshape and its index.
                                    binding.target = new TrackBindingTarget(m.gameObject, vBshapeIdx);

                                    break;
                                }
                            }

                            // 3rd algorithm: search any blendshape with name.
                            // Warning! It can create animate the wrong blendshape.
                            if (binding.target == null)
                            {
                                foreach (SkinnedMeshRenderer m in mRigSkinnedMeshRenderers)
                                {
                                    for (int i = 0; i < m.sharedMesh.blendShapeCount; i++)
                                    {
                                        string vBlendshape = m.sharedMesh.GetBlendShapeName(i);

                                        if (!vBlendshape.Contains(vBshapeName))
                                            continue;

                                        // We have found the corresponding blendshape in the mesh. 
                                        // Store the reference to constraint the blendshape and its index.
                                        binding.target = new TrackBindingTarget(m.gameObject, i);

                                        break;
                                    }
                                }
                            }

                            if(binding.target == null)
                            {
                                //Debug.LogWarning("Can't find blendshape \"" + vUnityBShapeName);
								Debug.LogWarning("[PrevizRetargeter.ParseESCandAllocateBuffers]: Can't find blendshape \"" + vBshapeName);
                                binding.type = PrevizTrackBinding.Type.INVALID;
                            }
                        }
                        else
                        {
                            Debug.LogWarning("[PrevizRetargeter.ParseESCandAllocateBuffers]: Can't parse blendshape name \"" + binding.bindName);
                            binding.type = PrevizTrackBinding.Type.INVALID;
                        }
                    }
                    else if (vRdr.GetAttribute("class") == "customproperty")
                    {
                        vCurrentBufIdx += 1;
                        break;
                    }
                    else
                    {
                        Debug.LogError("[PrevizRetargeter.ParseESCandAllocateBuffers]: bad entity type detected.");
                        // TODO: Abort connection? State might be corrupted 
                        // because vCurrentBufIdx can't be incremented correctly.
                    }

                    // TODO: Initialized all offset based on the PRL file? (cf old script code).
                    if (binding.type != PrevizTrackBinding.Type.INVALID)
                    {
                        //mEntities.Add(vCurrEntity);
                        bindings.Add(binding);
                    }
                }
                else if (vRdr.Name == "default" && binding.type == PrevizTrackBinding.Type.BONE)
                {
                    // NOTA: this code is not used currently, but might be in
                    // future.
                    // Parsing default position for bones.
                    bindings.Add(binding);
                }
            }

            // Allocate buffering stuff.
            mNbCoeff = iNbCoeff;
            mCoeffs = new float[mNbCoeff];

            //TODO: save neutral rig pose.
            return bindings;
        }
    }
}

