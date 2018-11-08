using UnityEngine;
using System;

namespace Slate{

	///Attribute to mark a field or property as an animatable parameter
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class AnimatableParameterAttribute : PropertyAttribute{
		public string link;
		public float? min;
		public float? max;
		public AnimatableParameterAttribute(){}
		public AnimatableParameterAttribute(float min, float max){
			this.min = min;
			this.max = max;
		}
	}

	///Attribute to mark a field or property to parse for sub animatable parameters
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public class ParseAnimatableParametersAttribute : PropertyAttribute{}

	///Attribute used to show a popup of shader properties of type
	[AttributeUsage(AttributeTargets.Field)]
	public class ShaderPropertyPopupAttribute : PropertyAttribute{
		public Type propertyType;
		public ShaderPropertyPopupAttribute(){}
		public ShaderPropertyPopupAttribute(Type propertyType){
			this.propertyType = propertyType;
		}
	}

    ///Attribute used to restrict float or int to a min value
    [AttributeUsage(AttributeTargets.Field)]
    public class MinAttribute : PropertyAttribute{
        public float min;
        public MinAttribute(float min){
            this.min = min;
        }
    }

    ///Show an example text in place of string field if string is null or empty
    [AttributeUsage(AttributeTargets.Field)]
    public class ExampleTextAttribute : PropertyAttribute{
        public string text;
        public ExampleTextAttribute(string text){
            this.text = text;
        }
    }

    ///Shows a HelpBox bellow field
    [AttributeUsage(AttributeTargets.Field)]
    public class HelpBoxAttribute : PropertyAttribute{
    	public string text;
    	public HelpBoxAttribute(string text){
    		this.text = text;
    	}
    }

	///Shows the property only if another property/field returns the specified value
	///The target value is int type, which means that can both be used for boolean as well as enum targets
	[AttributeUsage(AttributeTargets.Field)]
	public class ShowIfAttribute : PropertyAttribute{
		public string propertyName;
		public int value;
		public ShowIfAttribute(string propertyName, int value){
			this.propertyName = propertyName;
			this.value = value;
		}
	}

	///Callbacks target method when property changes in inspector
	[AttributeUsage(AttributeTargets.Field)]
	public class CallbackAttribute : PropertyAttribute{
		public string methodName;
		public CallbackAttribute(string methodName){
			this.methodName = methodName;
		}
	}

    ///Attribute used on Object or string field to mark them as required (red) if not set
    [AttributeUsage(AttributeTargets.Field)]
    public class RequiredAttribute : PropertyAttribute{}

    ///Attribute used to protect changes when cutscene playing
    [AttributeUsage(AttributeTargets.Field)]
    public class PlaybackProtectedAttribute : PropertyAttribute{}

    ///Attribute used to view field as read-only
    [AttributeUsage(AttributeTargets.Field)]
    public class ReadOnlyAttribute : PropertyAttribute{}

	///Used for a sorting layer popup
	[AttributeUsage(AttributeTargets.Field)]
	public class SortingLayerAttribute : PropertyAttribute { }
}