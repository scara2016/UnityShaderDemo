using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class ComputePass : ScriptableRenderPass
{
    private ComputeShader computeShader;
    string _kernelName;
    private int _blockSize;
    private int _renderTargetId;
    
    RenderTargetIdentifier _renderTargetIdentifier;
    int _renderTextureWidth;
    int _renderTextureHeight;
    public ComputePass(ComputeShader computeShader, string _kernelName, int blockSize, int renderTargetId)
    {
        this.computeShader = computeShader;
        this._kernelName = _kernelName;
        this._blockSize = blockSize;
        _renderTargetId = renderTargetId;
    }
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get();
        var mainKernel = computeShader.FindKernel(_kernelName);
        computeShader.GetKernelThreadGroupSizes(mainKernel, out uint xGroupSize, out uint yGroupSize, out _);
        cmd.Blit(renderingData.cameraData.targetTexture, _renderTargetIdentifier);
        cmd.SetComputeTextureParam(computeShader, mainKernel, _renderTargetId, _renderTargetIdentifier);
        cmd.DispatchCompute(computeShader, mainKernel,
            Mathf.CeilToInt(_renderTextureWidth / (float) _blockSize / xGroupSize),
            Mathf.CeilToInt(_renderTextureHeight / (float) _blockSize / yGroupSize),1);
        cmd.Blit(_renderTargetIdentifier, renderingData.cameraData.renderer.cameraColorTarget);
        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        var cameraTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
        _renderTargetIdentifier = new RenderTargetIdentifier(_renderTargetId);
        _renderTextureWidth = cameraTargetDescriptor.width;
        _renderTextureHeight = cameraTargetDescriptor.height;
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(_renderTargetId);
    }
}
