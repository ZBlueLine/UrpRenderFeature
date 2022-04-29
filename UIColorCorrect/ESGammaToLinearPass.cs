using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class ESGammaToLinearSetting
{
    public RenderPassEvent Event = RenderPassEvent.AfterRenderingTransparents;
}

public class ESGammaToLinearPass : ScriptableRenderPass
{
    const string TEXTURE_NAME = "_GammaToLinearTexture";
    const string PASS_TAG = "GammaToLinear";
    const string SHADER_TAG = "GammaToLinear";

    RenderTargetIdentifier source;

    ProfilingSampler profilingSampler = new ProfilingSampler(PASS_TAG);

    // 配置
    ESGammaToLinearSetting mSetting;

    RenderTargetHandle mGammaToLinearRT;

    ShaderTagId mShaderTagID = new ShaderTagId(SHADER_TAG);

    Material mGammaToLinearMat;

    public ESGammaToLinearPass(ESGammaToLinearSetting setting)
    {
        mSetting = setting;
        mGammaToLinearMat = new Material(Shader.Find("EsShaders/GammaToLinear"));
        renderPassEvent = mSetting.Event;
    }

    public void SetUp(RenderTargetIdentifier source)
    {
        mGammaToLinearRT.Init(TEXTURE_NAME);
        this.source = source;
    }
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (!renderingData.cameraData.postProcessEnabled) return;

        // 主摄像头和场景相机
        //if (renderingData.cameraData.camera != mCam) return;

        if (mGammaToLinearMat == null || mGammaToLinearMat.shader == null)
            return;

        CommandBuffer cmd = CommandBufferPool.Get(PASS_TAG);

        using (new ProfilingScope(cmd, profilingSampler))
        {
            var descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.depthBufferBits = 0;
            cmd.GetTemporaryRT(mGammaToLinearRT.id, descriptor);

            cmd.Blit(source, mGammaToLinearRT.Identifier(), mGammaToLinearMat);
            cmd.Blit(mGammaToLinearRT.Identifier(), source);
        }
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}