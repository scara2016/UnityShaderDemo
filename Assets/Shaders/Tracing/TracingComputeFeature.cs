using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TracingComputeFeature : ScriptableRendererFeature
{
    public ComputeShader tracingShader;
    private TracingPass _tracingPass;
    private string KernelName = "TracingMain";
    private bool _initialized;

    public override void Create()
    {
        Debug.Log("2");
        if (tracingShader == null)
        {
            _initialized = false;
            return;
        }
        int renderTargetId = Shader.PropertyToID("Result");
        _tracingPass = new TracingPass(tracingShader, KernelName, renderTargetId);
        _tracingPass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        _initialized = true;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if(_initialized)
            renderer.EnqueuePass(_tracingPass);
    }
}
