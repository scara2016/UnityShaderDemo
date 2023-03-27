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
    private int _renderArrayId;
    RenderTargetIdentifier _renderTargetIdentifier;
    private RenderTargetIdentifier _renderArrayIdentifier;
    int _renderTextureWidth;
    int _renderTextureHeight;
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
        _renderArrayIdentifier = new RenderTargetIdentifier(_renderArrayId);
        for (int i = 0; i < 10; i++)
        {
            cmd.Blit(renderingData.cameraData.targetTexture, _renderArrayId, 0, i);
        }
        cmd.SetComputeTextureParam(computeShader, computeShader.FindKernel("TracingMain"), "PrevFrames", _renderArrayIdentifier);
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
        Debug.Log("1");
        var cameraTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
        cameraTargetDescriptor.enableRandomWrite = true;
        cmd.GetTemporaryRTArray(_renderArrayId, cameraTargetDescriptor.width, cameraTargetDescriptor.height, 10, cameraTargetDescriptor.depthBufferBits, FilterMode.Bilinear,cameraTargetDescriptor.graphicsFormat, 1, true);
        cmd.GetTemporaryRT(_renderTargetId, cameraTargetDescriptor);
        _renderTargetIdentifier = new RenderTargetIdentifier(_renderTargetId);
        _renderTextureWidth = Screen.width;
        _renderTextureHeight = Screen.height;
    }
    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(_renderTargetId);
        cmd.ReleaseTemporaryRT(_renderArrayId);
    }
}
