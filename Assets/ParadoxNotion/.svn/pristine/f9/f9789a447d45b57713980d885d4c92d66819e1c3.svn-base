using System;
using System.Linq;
using UnityEngine;

namespace Slate {

    public static class IDirectableExtensions {

		///The length of the directable
		public static float GetLength(this IDirectable directable){
			return directable.endTime - directable.startTime;
		}

		///The local current time of the directable
		public static float RootToLocalTime(this IDirectable directable){
			return directable.RootToLocalTime(directable.root.currentTime);
		}

		///The local time based on provided root time
		public static float RootToLocalTime(this IDirectable directable, float time){
			return Mathf.Clamp(time - directable.startTime, 0, directable.GetLength());
		}

		///The local current time of the directable
		public static float RootToLocalTimeUnclamped(this IDirectable directable){
			return directable.RootToLocalTimeUnclamped(directable.root.currentTime);
		}

		///The local time based on provided root time
		public static float RootToLocalTimeUnclamped(this IDirectable directable, float time){
			return time - directable.startTime;
		}

		///Is root current time within directable range?
		public static bool RootTimeWithinRange(this IDirectable directable){
			return directable.root.currentTime >= directable.startTime && directable.root.currentTime <= directable.endTime && directable.root.currentTime > 0;
		}

		///----------------------------------------------------------------------------------------------

		///Returns the first child directable of provided name
		public static IDirectable FindChild(this IDirectable directable, string name){
			if (directable.children == null){ return null; }
			return directable.children.FirstOrDefault(d => d.name.ToLower() == name.ToLower());
		}

		///Returns the previous sibling in the parent (eg previous clip)
		public static T GetPreviousSibling<T>(this IDirectable directable) where T:IDirectable{ return (T)GetPreviousSibling(directable); }
		public static IDirectable GetPreviousSibling(this IDirectable directable){
			if (directable.parent != null){
				return directable.parent.children.LastOrDefault(d => d != directable && d.startTime < directable.startTime);
			}
			return null;
		}

		///Returns the next sibling in the parent (eg next clip)
		public static T GetNextSibling<T>(this IDirectable directable) where T:IDirectable{ return (T)GetNextSibling(directable); }
		public static IDirectable GetNextSibling(this IDirectable directable){
			if (directable.parent != null){
				return directable.parent.children.FirstOrDefault(d => d != directable && d.startTime > directable.startTime);
			}
			return null;
		}

		///Going upwards, returns the first parent of type T
		public static T GetFirstParentOfType<T>(this IDirectable directable) where T:IDirectable{
			var current = directable.parent;
			while(current != null){
				if (current is T){
					return (T)current;
				}
				current = current.parent;
			}
			return default(T);
		}

		///----------------------------------------------------------------------------------------------

		///The current weight based on blend properties and based on root current time.
		public static float GetWeight(this IDirectable directable){ return GetWeight(directable, RootToLocalTime(directable) ); }
		///The weight at specified local time based on its blend properties.
		public static float GetWeight(this IDirectable directable, float time){ return GetWeight(directable, time, directable.blendIn, directable.blendOut); }
		///The weight at specified local time based on provided override blend in/out properties
		public static float GetWeight(this IDirectable directable, float time, float blendInOut){ return GetWeight(directable, time, blendInOut, blendInOut); }
		public static float GetWeight(this IDirectable directable, float time, float blendIn, float blendOut){
			var length = GetLength(directable);
			if (time <= 0){
				return blendIn <= 0? 1 : 0;
			}

			if (time >= length){
				return blendOut <= 0? 1 : 0;
			}

			if (time < blendIn){
				return time/blendIn;
			}

			if (time > length - blendOut){
				return (length - time)/blendOut;
			}

			return 1;			
		}


	}
}