using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TracingPass : ScriptableRenderPass
{
    private ComputeShader computeShader;
    string _kernelName;
    private int _renderTargetId;
    RenderTargetIdentifier _renderTargetIdentifier;
    int _renderTextureWidth;
    int _renderTextureHeight;
    private RenderTexture rendTexArray;
    public TracingPass(ComputeShader computeShader, string _kernelName, int renderTargetId)
    {
        this.computeShader = computeShader;
        this._kernelName = _kernelName;
        _renderTargetId = renderTargetId;
        
        
    }
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get();
        var mainKernel = computeShader.FindKernel("TracingMain");
        computeShader.GetKernelThreadGroupSizes(mainKernel, out uint xGroupSize, out uint yGroupSize, out _);
        cmd.Blit(renderingData.cameraData.targetTexture, _renderTargetIdentifier);
        cmd.SetComputeTextureParam(computeShader, mainKernel, "PrevFrames", rendTexArray);
        cmd.SetComputeTextureParam(computeShader, mainKernel, _renderTargetId, _renderTargetIdentifier);
        cmd.DispatchCompute(computeShader, mainKernel,
            Mathf.CeilToInt(_renderTextureWidth /(float) xGroupSize),
            Mathf.CeilToInt(_renderTextureHeight /(float) yGroupSize),1);
        cmd.Blit(_renderTargetIdentifier, renderingData.cameraData.renderer.cameraColorTarget);
        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        rendTexArray = new RenderTexture(renderingData.cameraData.cameraTargetDescriptor);
        rendTexArray.dimension = TextureDimension.Tex2DArray;
        rendTexArray.enableRandomWrite = true;
        rendTexArray.volumeDepth = 10;
        rendTexArray.Create();
        var cameraTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
        cameraTargetDescriptor.enableRandomWrite = true;
        cmd.GetTemporaryRT(_renderTargetId, cameraTargetDescriptor);
        _renderTargetIdentifier = new RenderTargetIdentifier(_renderTargetId);
        _renderTextureWidth = Screen.width;
        _renderTextureHeight = Screen.height;
    }
    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(_renderTargetId);
    }
}
