using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TracingPass : ScriptableRenderPass
{
    Material tracingMaterial;
    Material emptyMaterial;
    private int frameCounter = 0;
    private RenderTargetHandle TempRT;
    public TracingPass(Shader tracingShader, Shader blackShader)
    {
        this.tracingMaterial = new Material(tracingShader);
        this.emptyMaterial = new Material(blackShader);
    }
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        frameCounter++;
        CommandBuffer cmd = CommandBufferPool.Get();
        cmd.GetTemporaryRT(TempRT.id, renderingData.cameraData.cameraTargetDescriptor);
        if (frameCounter % 1000 == 0)
        {
            
            Debug.Log(frameCounter);
            cmd.Blit(renderingData.cameraData.renderer.cameraColorTarget, TempRT.Identifier(), tracingMaterial);
            cmd.Blit(TempRT.Identifier(), renderingData.cameraData.renderer.cameraColorTarget);
                       
        }
       /* else
        {
            cmd.Blit(renderingData.cameraData.renderer.cameraColorTarget, TempRT.Identifier(), emptyMaterial);
            cmd.Blit(TempRT.Identifier(), renderingData.cameraData.renderer.cameraColorTarget);

        }*/
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);

    }

    // Start is called before the first frame update

}
