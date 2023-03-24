using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayCameraScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Camera>().clearFlags = CameraClearFlags.Nothing;
    }

    // Update is called once per frame
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Debug.Log("1");
    }
}
