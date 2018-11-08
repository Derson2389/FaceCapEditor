using UnityEngine;
using System.Collections.Generic;

public class SmoothCamera {

	public static int smoothingFrames = 10;

	private static  Quaternion smoothedRotation;
	private static Vector3 smoothedPosition;

	private static Queue<Quaternion> rotations;
    private static Queue<Vector3> positions;

    public static Vector3 SmoothPositionPost(Vector3 position)
    {
        if (positions.Count >= smoothingFrames)
        {
            positions.Dequeue();
        }
        positions.Enqueue(position);

        Vector3 avgp = Vector3.zero;
        foreach (Vector3 singlePosition in positions)
        {
            avgp += singlePosition;
        }
        avgp /= positions.Count;

        smoothedPosition = avgp;
        return smoothedPosition;
    }
    

	public static Quaternion SmoothRotation(Quaternion  rotation) {

		if (rotations.Count >= smoothingFrames) {
			rotations.Dequeue();
			
		}
		rotations.Enqueue(rotation);
		Vector4 avgr = Vector4.zero;
		foreach (Quaternion singleRotation in rotations) {
			Math3d.AverageQuaternion(ref avgr, singleRotation, rotations.Peek(), rotations.Count);
		}
		smoothedRotation = new Quaternion(avgr.x, avgr.y, avgr.z, avgr.w);
        return smoothedRotation;
    }

    // Use this for initialization
    public static void StartSmooth(int frames = 10)
    {
        smoothingFrames = frames;
        rotations = new Queue<Quaternion>(smoothingFrames);
        positions = new Queue<Vector3>(smoothingFrames);
    }
   
}