using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class ARFaceMeshObject : MonoBehaviour
{
    [SerializeField]
    public MeshFilter meshFilter;

    private Mesh _faceMesh;
    public Mesh faceMesh
    {
        get { return _faceMesh; }
    }

    private bool _loss = false;

    private Vector3 _localPos;
    private Vector3 _localRot;

    private Vector3[] _vertices;
    private Vector2[] _uvs;
    private int[] _triangles;

    // Use this for initialization
    void Start ()
    {
        if (_faceMesh == null)
        {
            _faceMesh = new Mesh();
            meshFilter.mesh = _faceMesh;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void InitAREvent()
    {
        UnityARSessionNativeInterface.ARFaceAnchorAddedEvent += FaceAdded;
        UnityARSessionNativeInterface.ARFaceAnchorUpdatedEvent += FaceUpdated;
        UnityARSessionNativeInterface.ARFaceAnchorRemovedEvent += FaceRemoved;
    }

    void OnDestroy()
    {
        UnityARSessionNativeInterface.ARFaceAnchorAddedEvent -= FaceAdded;
        UnityARSessionNativeInterface.ARFaceAnchorUpdatedEvent -= FaceUpdated;
        UnityARSessionNativeInterface.ARFaceAnchorRemovedEvent -= FaceRemoved;
    }

    public void OnUpdate()
    {
        if (_faceMesh == null)
        {
            return;
        }

        gameObject.transform.localPosition = _localPos;
        gameObject.transform.localEulerAngles = _localRot;

        _faceMesh.vertices = _vertices;
        _faceMesh.uv = _uvs;
        _faceMesh.triangles = _triangles;

        _faceMesh.RecalculateBounds();
        _faceMesh.RecalculateNormals();
    }

    void FaceAdded(ARFaceAnchor anchorData)
    {
        if(_faceMesh == null)
        {
            _faceMesh = new Mesh();
            meshFilter.mesh = _faceMesh;
        }

        _loss = false;

        _localPos = UnityARMatrixOps.GetPosition(anchorData.transform);
        _localRot = UnityARMatrixOps.GetRotation(anchorData.transform).eulerAngles;
        _vertices = anchorData.faceGeometry.vertices;
        _uvs = anchorData.faceGeometry.textureCoordinates;
        _triangles = anchorData.faceGeometry.triangleIndices;
    }

    void FaceUpdated(ARFaceAnchor anchorData)
    {
        if (_loss)
            return;

        _localPos = UnityARMatrixOps.GetPosition(anchorData.transform);
        _localRot = UnityARMatrixOps.GetRotation(anchorData.transform).eulerAngles;

        _vertices = anchorData.faceGeometry.vertices;
        _uvs = anchorData.faceGeometry.textureCoordinates;
        _triangles = anchorData.faceGeometry.triangleIndices;
    }

    void FaceRemoved(ARFaceAnchor anchorData)
    {
        //meshFilter.mesh = null;
        //_faceMesh = null;

        _loss = true;
    }
}
