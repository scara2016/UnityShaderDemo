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
    private Matrix4x4 _cameraToWorldMatrix;
    private Matrix4x4 _cameraInverseProjectionMatrix;
    public Texture SkyboxTexture;
    
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
        computeShader.SetTexture(mainKernel, "_SkyboxTexture", SkyboxTexture);
        cmd.SetComputeTextureParam(computeShader, mainKernel, _renderTargetId, _renderTargetIdentifier);
        cmd.SetComputeMatrixParam(computeShader,Shader.PropertyToID("_CameraToWorld"), _cameraToWorldMatrix );
        cmd.SetComputeMatrixParam(computeShader,Shader.PropertyToID("_CameraInverseProjection"), _cameraInverseProjectionMatrix );
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
        var cameraTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
        _cameraToWorldMatrix = renderingData.cameraData.camera.cameraToWorldMatrix;
        _cameraInverseProjectionMatrix = renderingData.cameraData.GetProjectionMatrix();
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
