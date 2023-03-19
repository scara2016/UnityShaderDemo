using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AntiAliasingPass : ScriptableRenderPass
{
    RenderTargetIdentifier source;
    Shader antiAliasingShader;
    public AntiAliasingPass(RenderTargetIdentifier source, Shader antiAliasingShader){
        this.source = source;
        this.antiAliasingShader = antiAliasingShader;

    }
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        
    }

    
}
