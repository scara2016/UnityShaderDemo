using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ComputeFeature : ScriptableRendererFeature
{
    private ComputePass _computePass;
    private bool _initialized;
    public ComputeShader computeShader;
    private string KernelName = "CSMain";
    [Range(2, 40)] public int BlockSize = 3;
    
    public override void Create()
    {
        if (computeShader == null)
        {
            _initialized = false;
            return;
        }
        int renderTargetId = Shader.PropertyToID("Result");
        
        _computePass = new ComputePass(computeShader, KernelName, BlockSize, renderTargetId);
        _computePass.renderPassEvent = RenderPassEvent.AfterRendering;
        _initialized = true;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if(_initialized)
            renderer.EnqueuePass(_computePass);
    }
}
