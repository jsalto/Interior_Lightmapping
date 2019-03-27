using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OcultarVentana : MonoBehaviour
{
    public Transform mCameraReal;
    public Transform wallL;

    void Update()
    {
        Vector3 camPosProjection = new Vector3(mCameraReal.position.x, 0, mCameraReal.position.z);
        Vector3 wallNormal = wallL.transform.TransformDirection(0, 0, -1);
        Vector3 wallNormalProjection = new Vector3(wallNormal.x, 0, wallNormal.z);
        Vector3 wallPosProjection = new Vector3(wallL.position.x, 0, wallL.position.z);
        Vector3 wallToCamVec = camPosProjection - wallPosProjection;

        float dotNormals = Vector3.Dot(wallToCamVec, wallNormalProjection);
        bool hide = (dotNormals < 0);
        foreach(Transform child in transform) {
            child.gameObject.SetActive(!hide);
        }
    }
}
