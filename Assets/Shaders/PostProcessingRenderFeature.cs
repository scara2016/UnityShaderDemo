using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingRenderFeature : ScriptableRendererFeature
{
    private RenderTargetIdentifier source;
    public Shader antiAliasingShader;
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        
    }
    AntiAliasingPass antiAliasingPass;
    public override void Create()
    {
        antiAliasingPass = new AntiAliasingPass(source , antiAliasingShader);
        // Configures where the render pass should be injected.
        antiAliasingPass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

}
