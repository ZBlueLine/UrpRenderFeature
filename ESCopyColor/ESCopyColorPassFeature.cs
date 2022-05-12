using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;

public class ESCopyColorPassFeature : ScriptableRendererFeature
{
    public string CameraTag = "MainCamera";
    public string ColorTextureName;
    public RenderPassEvent CopyColorEvent;
    public Downsampling ColoeTexureDownsampling;


    private Material m_CopyColorMat;
    private RenderTargetHandle m_destination;

    private CopyColorPass m_CopyColorPass;

    public override void Create()
    {
        Shader shader = Shader.Find("Hidden/Universal Render Pipeline/Sampling");
        m_CopyColorMat = CoreUtils.CreateEngineMaterial(shader);
        m_CopyColorPass = new CopyColorPass(CopyColorEvent, m_CopyColorMat);


        m_CopyColorPass.renderPassEvent = CopyColorEvent;
        m_destination.Init(ColorTextureName);
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (m_CopyColorMat == null) return;
        if(renderingData.cameraData.camera.tag != CameraTag && 
            renderingData.cameraData.cameraType != CameraType.SceneView) return;
        m_CopyColorPass.Setup(renderer.cameraColorTarget, m_destination, ColoeTexureDownsampling);
        renderer.EnqueuePass(m_CopyColorPass);
    }
}


