using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class OverlayCameraScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
     //   this.GetComponent<Camera>().clearFlags = CameraClearFlags.Nothing;
    }

    private float t = 0;
    void FixedUpdate()
    {
        this.transform.Rotate(Vector3.up, Time.fixedDeltaTime);
    }
    // Update is called once per frame
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Debug.Log("1");
    }
}
