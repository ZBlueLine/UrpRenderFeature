using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class ESLinearToGammaSetting
{
    public RenderPassEvent Event = RenderPassEvent.BeforeRenderingTransparents;
}

public class ESLinearToGammaPass : ScriptableRenderPass
{
    const string TEXTURE_NAME = "_LinearToGammaTexture";
    const string PASS_TAG = "LinearToGamma";
    const string SHADER_TAG = "LinearToGamma";

    RenderTargetIdentifier source;

    ProfilingSampler profilingSampler = new ProfilingSampler(PASS_TAG);

    // 配置
    ESLinearToGammaSetting mSetting;

    RenderTargetHandle mLinearToGammaRT;

    ShaderTagId mShaderTagID = new ShaderTagId(SHADER_TAG);

    Material mLinearToGammaMat;

    public ESLinearToGammaPass(ESLinearToGammaSetting setting)
    {
        mSetting = setting;
        mLinearToGammaMat = new Material(Shader.Find("EsShaders/LinearToGamma"));
        renderPassEvent = mSetting.Event;
    }

    public void SetUp(RenderTargetIdentifier source)
    {
        mLinearToGammaRT.Init(TEXTURE_NAME);
        this.source = source;
    }
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (!renderingData.cameraData.postProcessEnabled) return;

        // 主摄像头和场景相机
        //if (renderingData.cameraData.camera != mCam) return;

        if (mLinearToGammaMat == null || mLinearToGammaMat.shader == null)
            return;

        CommandBuffer cmd = CommandBufferPool.Get(PASS_TAG);

        using (new ProfilingScope(cmd, profilingSampler))
        {
            var descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.depthBufferBits = 0;
            cmd.GetTemporaryRT(mLinearToGammaRT.id, descriptor);

            cmd.Blit(source, mLinearToGammaRT.Identifier(), mLinearToGammaMat);
            cmd.Blit(mLinearToGammaRT.Identifier(), source);
        }
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}
