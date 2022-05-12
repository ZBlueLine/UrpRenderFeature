using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("ES/UIBlur")]
    public class ESUIBlur : VolumeComponent, IPostProcessComponent
    {
        public BoolParameter EnableUIBlur = new BoolParameter(false);
        public FloatParameter BlurSize = new FloatParameter(1.0f);
        public IntParameter BlurIterations = new IntParameter(1);
        public IntParameter DownSample = new IntParameter(1);
        public BoolParameter ForceRefresh = new BoolParameter(false);
        //public VolumeParameter<ESBlurAlgorithm> BlurMethod = new VolumeParameter<ESBlurAlgorithm>();

        public bool IsActive() => active;
        public bool IsTileCompatible() => false;
    }
}
