#if USE_SLATE
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Slate;
using System.Reflection;

[Attachable(typeof(ActorActionTrack))]
public class BlendShapeCtrlClip : ActionClip, ICrossBlendable
{
    public Action<BlendShapeCtrlClip, float> onEditKeyAction;

    [SerializeField]
    [HideInInspector]
    private float _length = 5f;
    [SerializeField]
    [HideInInspector]
    private float _blendIn = 0f;
    [SerializeField]
    [HideInInspector]
    private float _blendOut = 0f;
    [SerializeField]
    [Required]
    public TextAsset CtrlConfigDataFile;

    public PrevizCtrlHandler CtrlHandler = null;

    [AnimatableParameter(-1, 1)]
    public Vector2 r_out_brow_facialControl = Vector2.zero;
    [AnimatableParameter(-1, 1)]
    public Vector2 r_mid_brow_facialControl = Vector2.zero;
    [AnimatableParameter(-1, 1)]
    public Vector2 r_in_brow_facialControl = Vector2.zero;
    [AnimatableParameter(-1, 1)]
    public Vector2 l_out_brow_facialControl = Vector2.zero;
    [AnimatableParameter(-1, 1)]
    public Vector2 l_mid_brow_facialControl = Vector2.zero;
    [AnimatableParameter(-1, 1)]
    public Vector2 l_in_brow_facialControl = Vector2.zero;
    [AnimatableParameter(-1, 1)]
    public Vector2 r_brow_move_facialControl = Vector2.zero;
    [AnimatableParameter(-1, 1)]
    public Vector2 l_brow_move_facialControl = Vector2.zero;

    //eye_list
    [AnimatableParameter(-1, 1)]
    public Vector2 r_upper_eyelid_facialControl = Vector2.zero;
    [AnimatableParameter(-1, 1)]
    public Vector2 r_lower_eyelid_facialControl = Vector2.zero;
    [AnimatableParameter(-1, 1)]
    public Vector2 l_upper_eyelid_facialControl = Vector2.zero;
    [AnimatableParameter(-1, 1)]
    public Vector2 l_lower_eyelid_facialControl = Vector2.zero;
    [AnimatableParameter(-1, 1)]
    public Vector2 r_eyeBall_facialControl = Vector2.zero;
    [AnimatableParameter(-1, 1)]
    public Vector2 l_eyeBall_facialControl = Vector2.zero;
    [AnimatableParameter(-1, 1)]
    public Vector2 r_pupil_scale_facialControl = Vector2.zero;
    [AnimatableParameter(-1, 1)]
    public Vector2 l_pupil_scale_facialControl = Vector2.zero;
    [AnimatableParameter(-1, 1)]
    public Vector2 r_eyeHightLight_move_facialControl = Vector2.zero;
    [AnimatableParameter(-1, 1)]
    public Vector2 l_eyeHightLight_move_facialControl = Vector2.zero;
    [AnimatableParameter(-1, 1)]
    public Vector2 r_eyeHightLight_scale_facialControl = Vector2.zero;
    [AnimatableParameter(-1, 1)]
    public Vector2 l_eyeHightLight_scale_facialControl = Vector2.zero;
    [AnimatableParameter(-1, 0)]
    public Vector2 r_eyelid_blink_facialControl = Vector2.zero;
    [AnimatableParameter(-1, 0)]
    public Vector2 l_eyelid_blink_facialControl = Vector2.zero;


    //cheek_list
    [AnimatableParameter(-1, 1)]
    public Vector2 r_cheek_facialControl = Vector2.zero;
    [AnimatableParameter(-1, 1)]
    public Vector2 l_cheek_facialControl = Vector2.zero;
    [AnimatableParameter(-1, 1)]
    public Vector2 nose_facialControl = Vector2.zero;
    [AnimatableParameter(0, 1)]
    public Vector2 r_nose_facialControl = Vector2.zero;
    [AnimatableParameter(0, 1)]
    public Vector2 l_nose_facialControl = Vector2.zero;

    //mouth_list
    [AnimatableParameter(-1, 1)]
    public Vector2 tongue_facialControl = Vector2.zero;
    [AnimatableParameter(-1, 1)]
    public Vector2 jaw_facialControl = Vector2.zero;
    [AnimatableParameter(-1, 1)]
    public Vector2 r_corners_facialControl = Vector2.zero;
    [AnimatableParameter(-1, 1)]
    public Vector2 l_corners_facialControl = Vector2.zero;
    [AnimatableParameter(-1, 1)]
    public Vector2 upper_lip_facialControl = Vector2.zero;
    [AnimatableParameter(-1, 1)]
    public Vector2 lower_lip_facialControl = Vector2.zero;
    [AnimatableParameter(-1, 1)]
    public Vector2 r_upper_lip_facialControl = Vector2.zero;
    [AnimatableParameter(-1, 1)]
    public Vector2 r_lower_lip_facialControl = Vector2.zero;
    [AnimatableParameter(-1, 1)]
    public Vector2 l_upper_lip_facialControl = Vector2.zero;
    [AnimatableParameter(-1, 1)]
    public Vector2 l_lower_lip_facialControl = Vector2.zero;
    [AnimatableParameter(-1, 1)]
    public Vector2 r_upper_corners_facialControl = Vector2.zero;
    [AnimatableParameter(-1, 1)]
    public Vector2 r_lower_corners_facialControl = Vector2.zero;
    [AnimatableParameter(-1, 1)]
    public Vector2 l_upper_corners_facialControl = Vector2.zero;
    [AnimatableParameter(-1, 1)]
    public Vector2 l_lower_corners_facialControl = Vector2.zero;
    [AnimatableParameter(-1, 1)]
    public Vector2 mouth_rotate_facialControl = Vector2.zero;
    [AnimatableParameter(-1, 1)]
    public Vector2 mouth_move_facialControl = Vector2.zero;

    //kouxing_list
    [AnimatableParameter(0, 1)]
    public Vector2 A_facialControl = Vector2.zero;
    [AnimatableParameter(0, 1)]
    public Vector2 E_facialControl = Vector2.zero;
    [AnimatableParameter(0, 1)]
    public Vector2 I_facialControl = Vector2.zero;
    [AnimatableParameter(0, 1)]
    public Vector2 O_facialControl = Vector2.zero;
    [AnimatableParameter(0, 1)]
    public Vector2 U_facialControl = Vector2.zero;
    [AnimatableParameter(0, 1)]
    public Vector2 F_facialControl = Vector2.zero;
    [AnimatableParameter(-1, 1)]
    public Vector2 M_facialControl = Vector2.zero;

    // other_list
    [AnimatableParameter(-1, 1)]
    public Vector2 upper_teeth_facialControl = Vector2.zero;      
    [AnimatableParameter(-1, 1)]
    public Vector2 lower_teeth_facialControl = Vector2.zero;     
    [AnimatableParameter(0, 1)]
    public Vector2 add_facialControl = Vector2.zero;
    [AnimatableParameter(0, 1)]
    public Vector2 add01_facialControl = Vector2.zero;
    [AnimatableParameter(0, 1)]
    public Vector2 add02_facialControl = Vector2.zero;
    [AnimatableParameter(0, 1)]
    public Vector2 add03_facialControl = Vector2.zero;


#if UNITY_EDITOR
    private BSEditKey _editKey;
    public BSEditKey editKey
    {
        get { return _editKey;  }
        set { _editKey = value; }
    }
#endif

 
    public FaceControllerComponent FaceCtr
    {
        get
        {
            if(actor == null)
            {
                return null;
            }
            return actor.GetComponent<FaceControllerComponent>();
        }
    }

    public override string info
    {
        get
        {           
            return actor.name + "_faceData";
        }
    }

    public override bool isValid
    {
        get { return FaceCtr != null && CtrlConfigDataFile!= null; }
    }
        
    public override float length
    {
        get { return _length; }
        set {
            _length = value;
        }
    }

    public override float blendIn
    {
        get { return _blendIn; }
        set {
            _blendIn = value;
        }
    }

    public override float blendOut
    {
        get { return _blendOut; }
        set {
            _blendOut = value;
        }
    }

    protected override bool OnInitialize()
    {            
        return true;
    }

    protected override void OnAfterValidate()
    {
        if (FaceCtr == null )
            return;
        CtrlHandler = new PrevizCtrlHandler();        
        CtrlHandler.LoadConfig(CtrlConfigDataFile, FaceCtr.gameObject);

    }

    protected override void OnUpdate(float time, float previousTime)
    {
        foreach (var animParam in animationData.animatedParameters.ToArray())
        {
            if (!animParam.isValid || !animParam.enabled || animParam.curves == null)
                        continue;

            Vector2 currentVal = (Vector2)animParam.GetCurrentValue();
            CtrlHandler.SetBlenderShapeByCtrlName(animParam.parameterName,new Vector2(currentVal.x, currentVal.y));            
        }
    }
    
    ////////////////////////////////////////
    ///////////GUI AND EDITOR STUFF/////////
    ////////////////////////////////////////
#if UNITY_EDITOR

    public class BSEditKey
    {
        private BlendShapeCtrlClip _clip;

        private Dictionary<string, AnimatedParameter> _controllerParams;
        public Dictionary<string, AnimatedParameter> controllerParams
        {
            get { return _controllerParams; }
        }

        public BSEditKey(BlendShapeCtrlClip clip)
        {
            _controllerParams = new Dictionary<string, AnimatedParameter>();
            _clip = clip;
            
            Init(0);
        }

        public BSEditKey(float time, BlendShapeCtrlClip clip)
        {
            _controllerParams = new Dictionary<string, AnimatedParameter>();
            _clip = clip;
   
            Init(time);
        }

        void Init(float time)
        {
            foreach (var animParam in _clip.animationData.animatedParameters.ToArray())
            {
                if (!animParam.isValid || !animParam.enabled)
                    continue;

                string paramName = animParam.parameterName.ToLower();
                controllerParams.Add(paramName, animParam);
                animParam.SetHideChecked(true);
                
            }
        }

        public void ChangeAnimParamState(string name, bool state)
        {
            AnimatedParameter param;
            if (controllerParams.TryGetValue(name.ToLower(),out param))
            {
                param.SetHideChecked(state);
            }
            var track = _clip.parent as ActorActionTrack;
            if (track != null)
            {
                track.InspectedParameterIndex = -1;
            }
        }


        public float GetCurrentTime()
        {
            return _clip.root.currentTime;
        }

        public Vector2 GetControllerParamValue(string controllerName)
        {
            AnimatedParameter param ;
           


            if (controllerParams.TryGetValue(controllerName.ToLower(), out param))
            {
                return (Vector2)param.GetCurrentValue();
            }

            return new Vector2(float.PositiveInfinity, float.PositiveInfinity);
        }

        public void SetControllerParamValue(string controllerName, Vector2 value)
        {
            if (!controllerParams.ContainsKey(controllerName.ToLower()))
                return;

            ////Debug.Log("[BSEditKey.SetControllerParamValue] -> name: " + controllerParams[controllerName].parameterName + " value: " + value);
            controllerParams[controllerName.ToLower()].SetCurrentValue(value);
        }

        public bool HasEnterEditClip()
        {
            // 判断时间是否在范围中
            float time = _clip.root.currentTime - _clip.startTime;
            if (time < 0 || time > _clip.length)
                return false;

            return true;
        }

        public bool HasEnterAnimatableRange()
        {
            var keys = new List<Keyframe>();

            // 获取所有关键帧
            foreach (var item in controllerParams)
            {
                AnimatedParameter animParam = item.Value;

                if (animParam.curves == null)
                    continue;

                foreach (var curve in animParam.curves)
                {
                    keys.AddRange(curve.keys);
                }
            }

            // 如果没有关键帧，表示没有动画区域
            if (keys.Count == 0)
                return false;

            keys = keys.OrderBy(k => k.time).ToList();

            // 如果当前时间介于第一帧和最后一帧之间， 则位于动画区域
            float time = _clip.root.currentTime - _clip.startTime;
            if (time > keys[0].time && time < keys[keys.Count - 1].time)
                return true;

            return false;
        }

        public void Save()
        {
            // 判断时间是否在范围中
            if (!HasEnterEditClip())
                return;

            float time = _clip.root.currentTime - _clip.startTime;
            //TangentMode mode = Prefs.defaultTangentMode;
            //Prefs.defaultTangentMode = TangentMode.Linear;

            foreach (var item in controllerParams)
            {
                AnimatedParameter animParam = item.Value;
                //var hasAnyKey = animParam.HasAnyKey();

                if(animParam.hideChecked)
                {
                    continue;
                }
                // 修改空帧也需要k值
                /*if(!hasAnyKey)
                {
                    animParam.SetKeyCurrent(time);
                }
                else
                {
                    animParam.TryAutoKey(time);
                }*/

                //animParam.SetKeyCurrent(time);
                //Debug.Log("[EmotionEditKey.Save] -> name: " + animParam.parameterName + " Saved value: " + animParam.GetCurrentValue());

                Vector2 val = (Vector2)animParam.GetCurrentValue();
                var index0 = animParam.curves[0].AddKey(time, val.x);
                var index1 = animParam.curves[1].AddKey(time, val.y);

                //// 设置所有的关键帧为线性插值
                //if (animParam.curves != null)
                //{
                //    SetCurveTangentMode(animParam.curves[0]);
                //    SetCurveTangentMode(animParam.curves[1]);
                //}
            }

            //Prefs.defaultTangentMode = mode;
        }

    }

    public override void EditKeyable(float time)
    {
        _editKey = new BSEditKey(time, this);

        if (onEditKeyAction != null)
            onEditKeyAction(this, time);

    }

    public static void SetCurveTangentMode(AnimationCurve curve)
    {
        for (int i = 0; i < curve.length; i++)
        {
            curve.SetKeyTangentMode(i, TangentMode.Linear);
        }

        curve.UpdateTangentsFromMode();
    }

#endif



}
#endif