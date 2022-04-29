using UnityEngine.Rendering.Universal;
using UnityEngine;
using UnityEngine.Rendering;

public class ESUIColorCorrectFeature : ScriptableRendererFeature
{
    public ESLinearToGammaSetting LinearToGammaSetting;

    public ESGammaToLinearSetting GammaToLinearSetting;

    public bool EnableInSceneView = false;

    Camera mCam;

    ESGammaToLinearPass mGammaToLinearPass;

    ESLinearToGammaPass mLinearToGammaPass;

    public override void Create()
    {
        mGammaToLinearPass = new ESGammaToLinearPass(GammaToLinearSetting);
        mLinearToGammaPass = new ESLinearToGammaPass(LinearToGammaSetting);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // 后处理全局开关
        if (!renderingData.cameraData.postProcessEnabled) return;
#if UNITY_EDITOR
        {
            if (Application.isEditor && EnableInSceneView)
            {
                if (renderingData.cameraData.cameraType == CameraType.SceneView)
                {
                    mGammaToLinearPass.SetUp(renderer.cameraColorTarget);
                    renderer.EnqueuePass(mGammaToLinearPass);
                    return;
                }
            }
        }
#endif

        var correctSetting = VolumeManager.instance.stack.GetComponent<ESUIColorCorrect>();
        if (correctSetting.EnableLinearToGamma == true)
        {
            //3D场景的预制体带有一个校色后处理，保证其仅在多相机情况下启用
#if UNITY_EDITOR
            if (Camera.main != null)
            {
                UniversalAdditionalCameraData mainCameraData = Camera.main.GetUniversalAdditionalCameraData();
                if (mainCameraData.cameraStack.Count != 0)
                {
                    bool flag = false;
                    foreach(Camera cam in mainCameraData.cameraStack)
                    {
                        if (cam.tag == "UICamera")
                        {
                            flag = true;
                            break;
                        }
                    }
                    if(flag)
                    {
                        mLinearToGammaPass.SetUp(renderer.cameraColorTarget);
                        renderer.EnqueuePass(mLinearToGammaPass);
                    }
                }
            }
#else
            mLinearToGammaPass.SetUp(renderer.cameraColorTarget);
            renderer.EnqueuePass(mLinearToGammaPass);
#endif
        }

        if (correctSetting.EnableGammaToLinear == true)
        {
            mGammaToLinearPass.SetUp(renderer.cameraColorTarget);
            renderer.EnqueuePass(mGammaToLinearPass);
        }
    }
}
