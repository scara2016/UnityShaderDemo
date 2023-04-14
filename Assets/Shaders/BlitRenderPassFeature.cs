using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlitRenderPassFeature : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        public RenderTargetIdentifier source;
        private List<Material> matList;
        private Material outlineAdditionMaterial;
        private RenderTargetHandle tempRenderTargetHandler;
        private RenderTargetHandle tempOGStorage;
        
        public CustomRenderPass(List<Material> materials, Material outlineAdditionMaterial)
        //public CustomRenderPass(Material material, Material magnitudeThresholdMaterial, Material doubleThresholdMaterial, Material hysterisisMaterial, Material outlineAdditionMaterial)
        {
            this.matList = materials;
            this.outlineAdditionMaterial = outlineAdditionMaterial;
            tempRenderTargetHandler.Init("_TemporaryColorTexture");
            tempOGStorage.Init("_TempOGStorage");
        }
        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer commandBuffer = CommandBufferPool.Get("CustomRenderPass");
            commandBuffer.GetTemporaryRT(tempRenderTargetHandler.id, renderingData.cameraData.cameraTargetDescriptor);
            commandBuffer.GetTemporaryRT(tempOGStorage.id, renderingData.cameraData.cameraTargetDescriptor);
            Blit(commandBuffer, source, tempOGStorage.Identifier());
            int numberOfPasses = 0;
            for(int i = 0; i < matList.Count; i++)
            { 
                    Blit(commandBuffer, source, tempRenderTargetHandler.Identifier(), matList[i], 0);
                    Blit(commandBuffer, tempRenderTargetHandler.Identifier(), source);
//                    Blit(commandBuffer, source, tempRenderTargetHandler.Identifier(), matList[i], 1);
//                    Blit(commandBuffer, tempRenderTargetHandler.Identifier(), source);


            }
            
            RenderTargetIdentifier og = new RenderTargetIdentifier();
            commandBuffer.ConvertTexture(source, og);
           
            if (outlineAdditionMaterial != null)
            {
                RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 8);
                renderTexture.Create();
                commandBuffer.Blit(tempOGStorage.Identifier(), renderTexture);
                outlineAdditionMaterial.SetTexture("_OutlineTex", renderTexture);
            }
            Blit(commandBuffer, source, tempRenderTargetHandler.Identifier(), outlineAdditionMaterial);
            Blit(commandBuffer, tempRenderTargetHandler.Identifier(), source);


            context.ExecuteCommandBuffer(commandBuffer);
            CommandBufferPool.Release(commandBuffer);
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }
    }

    [System.Serializable]
    public class Settings
    {
        public List<Material> matList;
        public Material outlineAdditionMaterial = null;
    }
    public Settings settings = new Settings();
    CustomRenderPass m_ScriptablePass;

    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass(settings.matList, settings.outlineAdditionMaterial);
        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        m_ScriptablePass.source = renderer.cameraColorTarget;
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


