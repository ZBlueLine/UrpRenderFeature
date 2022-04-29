using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("ES/UIColorCorrect")]
    public class ESUIColorCorrect : VolumeComponent, IPostProcessComponent
    {
        public BoolParameter EnableLinearToGamma = new BoolParameter(false);
        public BoolParameter EnableGammaToLinear = new BoolParameter(false);
        public bool IsActive() => active;
        public bool IsTileCompatible() => false;
    }
}
