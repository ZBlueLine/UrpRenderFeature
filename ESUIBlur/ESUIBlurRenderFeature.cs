using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;


public enum ESBlurAlgorithm
{
    Blur,
    GaussianBlur
}

public class ESUIBlurRenderFeature : ScriptableRendererFeature
{
    public ESBlurAlgorithm BlurMethod;

    private Material m_GaussianMat;
    private string m_CameraTag = "UICamera";
    private string m_ColorTextureName = "_CameraUITexture";
    private RenderPassEvent m_CopyColorEvent = RenderPassEvent.AfterRenderingTransparents;
    private RenderTargetHandle m_destination;
    private ESUIBlurPass m_CopyColorPass;


    public override void Create()
    {
        Shader shader = Shader.Find("EsShader/GaussianBlur");
        m_GaussianMat = CoreUtils.CreateEngineMaterial(shader);
        m_destination.Init(m_ColorTextureName);
        m_CopyColorPass = new ESUIBlurPass(m_CopyColorEvent,
                                BlurMethod, m_GaussianMat);
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        var correctSetting = VolumeManager.instance.stack.GetComponent<ESUIBlur>();
        if (correctSetting.EnableUIBlur == false)return;
        if (renderingData.cameraData.camera.tag != m_CameraTag) return;
        m_CopyColorPass.Setup(renderer.cameraColorTarget, m_destination, (int)correctSetting.DownSample, (float)correctSetting.BlurSize,
                                (int)correctSetting.BlurIterations, (bool)correctSetting.ForceRefresh);
        renderer.EnqueuePass(m_CopyColorPass);
    }
}


