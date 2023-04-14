using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TracingPass : ScriptableRenderPass
{
    private ComputeShader computeShader;
    string _kernelName;
    private int _renderTargetId;
    //private int _renderArrayId;
    RenderTargetIdentifier _renderTargetIdentifier;
    //private RenderTargetIdentifier _renderArrayIdentifier;
    int _renderTextureWidth;
    int _renderTextureHeight;
    bool flag = true;
    RenderTexture texture2DArray;

    private int prevFramesVolume = 100;
    //private Texture2DArray texture2DArray;
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
        computeShader.GetKernelThreadGroupSizes(mainKernel, out uint xGroupSize, out uint yGroupSize, out uint _zGroupSize);
        cmd.Blit(renderingData.cameraData.renderer.cameraColorTarget, _renderTargetIdentifier);
        //_renderArrayIdentifier = new RenderTargetIdentifier(_renderArrayId);
        if (flag)
        {
            texture2DArray = new RenderTexture(renderingData.cameraData.cameraTargetDescriptor);
            texture2DArray.dimension = TextureDimension.Tex2DArray;
            texture2DArray.volumeDepth = prevFramesVolume;
            texture2DArray.enableRandomWrite = true;
            texture2DArray.Create();
            /*texture2DArray = new Texture2DArray(renderingData.cameraData.cameraTargetDescriptor.width,
                renderingData.cameraData.cameraTargetDescriptor.height, 10,
                GraphicsFormat.R8G8B8A8_SRGB, TextureCreationFlags.None);*/
            cmd.Blit(renderingData.cameraData.renderer.cameraColorTarget, texture2DArray, 0, 0);
            
            for (int i = 0; i < prevFramesVolume; i++)
            {
                cmd.Blit(renderingData.cameraData.renderer.cameraColorTarget, texture2DArray, 0, i);
                //texture2DArray.SetPixels(texture2DArray.GetPixels(0),i);
            }
            //texture2DArray.Apply();
            cmd.SetComputeTextureParam(computeShader, computeShader.FindKernel("TracingMain"), "PrevFrames", texture2DArray);
            flag = false;
        }
        else
        {
            cmd.SetComputeTextureParam(computeShader, computeShader.FindKernel("TracingMain"), "PrevFrames", texture2DArray);
        }
        
        
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
        //cmd.GetTemporaryRTArray(_renderArrayId, cameraTargetDescriptor.width, cameraTargetDescriptor.height, 10, cameraTargetDescriptor.depthBufferBits, FilterMode.Bilinear,cameraTargetDescriptor.graphicsFormat, 1, true);
        cmd.GetTemporaryRT(_renderTargetId, cameraTargetDescriptor);
        _renderTargetIdentifier = new RenderTargetIdentifier(_renderTargetId);
        _renderTextureWidth = Screen.width;
        _renderTextureHeight = Screen.height;
    }
    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(_renderTargetId);
        Debug.Log("dumbass");
    }
}
