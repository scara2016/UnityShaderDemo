using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AntiAliasingPass : ScriptableRenderPass
{
    RenderTargetIdentifier source;
    Shader AntiAliasingShader;
    public AntiAliasingPass(RenderTargetIdentifier source){
        this.source = source;
    }
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        
    }

    
}
