using UnityEngine;
using System;

namespace Slate {

	[AttributeUsage(AttributeTargets.Class)]
	///Use to override the naming of a type
	public class NameAttribute : Attribute{
		public string name;
		public NameAttribute(string name){
			this.name = name;
		}
	}

	[AttributeUsage(AttributeTargets.Class)]
	///Use to set the category of a type
	public class CategoryAttribute : Attribute{
		public string category;
		public CategoryAttribute(string category){
			this.category = category;
		}
	}

	[AttributeUsage(AttributeTargets.Class)]
	///Use to set the description of a type
	public class DescriptionAttribute : Attribute{
		public string description;
		public DescriptionAttribute(string description){
			this.description = description;
		}
	}

	[AttributeUsage(AttributeTargets.Class)]
	///Use to specify an icon for a type
	public class IconAttribute : Attribute{
		public string iconName;
		public IconAttribute(string iconName){
			this.iconName = iconName;
		}
	}

	[AttributeUsage(AttributeTargets.Class)]
	///Use to define which other types this type can be attached to in regards to IDirectrables
	public class AttachableAttribute : Attribute{
		public Type[] types;
		public AttachableAttribute(params Type[] types){
			this.types = types;
		}
	}

	[AttributeUsage(AttributeTargets.Class)]
	///Use to define that this Type should be unique within it's IDirectable parent
	public class UniqueElementAttribute : Attribute{}


	///Attribute used along with a Vector3 to show it's trajectory in the scene
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class ShowTrajectoryAttribute : Attribute{}

	///Attribute used along with a Vector3 to control it with a position handle in the scene
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class PositionHandleAttribute : Attribute{}
}