//Modify from URP ESUIBlurPass
using System;

namespace UnityEngine.Rendering.Universal.Internal
{
    /// <summary>
    /// Copy the given color buffer to the given destination color buffer.
    ///
    /// You can use this pass to copy a color buffer to the deBlurCountstination,
    /// so you can use it later in rendering. For example, you can copy
    /// the opaque texture to use it for distortion effects.
    /// </summary>
    public class ESUIBlurPass : ScriptableRenderPass
    {

        private RenderTargetIdentifier source { get; set; }
        private RenderTargetHandle destination { get; set; }
        private UnityEngine.UI.RawImage rwImg { get; set; }

        private RenderTextureDescriptor m_descriptor;

        const string m_ProfilerTag = "UI Gaussian Blur";

        private int m_Downsampling;
        private ESBlurAlgorithm m_BlurMethod;
        private RenderTexture m_BlurUIRT;
        private Material m_GaussianMat;
        private float m_BlurValue;
        private float m_Blueiterations;
        private bool m_ForceRefresh;

        /// <summary>
        /// Create the ESUIBlurPass
        /// </summary>
        public ESUIBlurPass(RenderPassEvent evt,  
            ESBlurAlgorithm blurMethod, Material GaussianMat)
        {
            renderPassEvent = evt;
            m_BlurMethod = ESBlurAlgorithm.Blur;
            m_GaussianMat = GaussianMat;
            m_BlurMethod = blurMethod;
            m_ForceRefresh = false;
        }

        /// <summary>
        /// Configure the pass with the source and destination to execute on.
        /// </summary>
        /// <param name="source">Source Render Target</param>
        /// <param name="destination">Destination Render Target</param>
        public void Setup(RenderTargetIdentifier source, RenderTargetHandle destination, int downsampling, float BlurValue,
                            float BlurIterations, bool forcerefresh)
        {
            this.source = source;
            m_BlurValue = BlurValue;
            this.destination = destination;
            m_Blueiterations = BlurIterations;
            m_Blueiterations = m_Blueiterations < 0 ? 0 : m_Blueiterations;
            m_Downsampling = downsampling;
            m_Downsampling = m_Downsampling <= 0 ? 1 : m_Downsampling;
            m_ForceRefresh = forcerefresh;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescripor)
        {
            RenderTextureDescriptor descriptor = cameraTextureDescripor;
            m_descriptor = descriptor;
            descriptor.msaaSamples = 1;
            descriptor.depthBufferBits = 0;
            descriptor.width /= m_Downsampling;
            descriptor.height /= m_Downsampling;
            descriptor.width = descriptor.width < 1 ? 1 : descriptor.width;
            descriptor.height = descriptor.height < 1 ? 1 : descriptor.height;
            if (m_BlurUIRT == null || m_ForceRefresh)
            {
                if(m_BlurUIRT != null)
                {
                    m_BlurUIRT.Release();
                }
                m_BlurUIRT = new RenderTexture(descriptor);
                m_BlurUIRT.name = "UIBlurRT";
            }
            cmd.GetTemporaryRT(destination.id, descriptor, m_Downsampling == 1 ? FilterMode.Point : FilterMode.Bilinear);
        }

        /// <inheritdoc/>
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
            RenderTargetIdentifier opaqueColorRT = destination.Identifier();

            if (m_GaussianMat != null)
            {
                m_GaussianMat.SetFloat(Shader.PropertyToID("_BlurSize"), m_BlurValue);
            }

            switch (m_BlurMethod)
            {
                case ESBlurAlgorithm.Blur:
                    Blit(cmd, source, m_BlurUIRT, m_GaussianMat, 0);
                    for (int i = 0; i < m_Blueiterations; ++i)
                    {
                        Blit(cmd, m_BlurUIRT, opaqueColorRT, m_GaussianMat, 0);
                        Blit(cmd, opaqueColorRT, m_BlurUIRT, m_GaussianMat, 0);
                    }
                    break;
                case ESBlurAlgorithm.GaussianBlur:
                    Blit(cmd, source, m_BlurUIRT);
                    for(int i = 0; i < m_Blueiterations; ++i)
                    {
                        Blit(cmd, m_BlurUIRT, opaqueColorRT, m_GaussianMat, 1);
                        Blit(cmd, opaqueColorRT, m_BlurUIRT, m_GaussianMat, 2);
                    }
                    break;
            }

            context.ExecuteCommandBuffer(cmd);
            Shader.SetGlobalTexture(Shader.PropertyToID("_BlurUITex"), m_BlurUIRT);
            CommandBufferPool.Release(cmd);
        }

        /// <inheritdoc/>
        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (cmd == null)
                throw new ArgumentNullException("cmd");
            //if(m_BlurUIRT != null)
            //{
                
            //}
            if (destination != RenderTargetHandle.CameraTarget)
            {
                cmd.ReleaseTemporaryRT(destination.id);
                destination = RenderTargetHandle.CameraTarget;
            }
        }
    }
}
