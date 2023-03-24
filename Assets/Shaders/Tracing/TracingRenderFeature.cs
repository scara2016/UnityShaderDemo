using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TracingRenderFeature : ScriptableRendererFeature
{
    private TracingPass tracingPass;
    [SerializeField]
    private Shader tracingShader;
    [SerializeField]
    private Shader blackShader;

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {

        renderer.EnqueuePass(tracingPass);
    }

    public override void Create()
    {
        tracingPass = new TracingPass(tracingShader, blackShader);
        tracingPass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        
    }

}
