// (c) Copyright HutongGames, LLC. All rights reserved.
// Author Eric Vander Wal www.dumbgamedev.com

using UnityEngine;
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMesh Pro Shader")]
    [Tooltip("Set Text Mesh Pro glow shaders.")]
    public class setTextmeshProShaderPropertiesGlow : ComponentAction<TextMeshPro>
    {
        [RequiredField]
        [CheckForComponent(typeof(TextMeshPro))]
        [Tooltip("Textmesh Pro component is required.")]
        public FsmOwnerDefault gameObject;
        
        public FsmBool enableGlow;

        public FsmColor glowColor;

        [HasFloatSlider(-1, 1f)]
        public FsmFloat offset;

        [HasFloatSlider(0, 1)]
        public FsmFloat inner;

        [HasFloatSlider(0, 1)]
        public FsmFloat outer;

        [HasFloatSlider(1, 0)]
        public FsmFloat power;

        [Tooltip("Check this box to preform this action every frame.")]
        public FsmBool everyFrame;

        public override void Reset()
        {
            enableGlow = null;
            offset = null;
            inner = null;
            outer = null;
            power = null;
            glowColor = null;
            gameObject = null;
            everyFrame = false;
        }

        public override void OnEnter()
        {
            DoMeshChange();

            if (!everyFrame.Value)
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            if (everyFrame.Value)
            {
                DoMeshChange();
            }
        }

        void DoMeshChange()
        {
            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                Debug.LogError("No textmesh pro component was found on " + gameObject);
                return;
            }
            
            if (enableGlow.Value == true)
            {
                this.cachedComponent.fontSharedMaterial.EnableKeyword("GLOW_ON");
            }
            else
            {
                this.cachedComponent.fontSharedMaterial.DisableKeyword("GLOW_ON");
            }

            if(!glowColor.IsNone) this.cachedComponent.fontSharedMaterial.SetColor("_GlowColor", glowColor.Value);
            if(!offset.IsNone) this.cachedComponent.fontSharedMaterial.SetFloat("_GlowOffset", offset.Value);
            if(!inner.IsNone) this.cachedComponent.fontSharedMaterial.SetFloat("_GlowInner", inner.Value);
            if(!outer.IsNone) this.cachedComponent.fontSharedMaterial.SetFloat("_GlowOuter", outer.Value);
            if(!power.IsNone) this.cachedComponent.fontSharedMaterial.SetFloat("_GlowPower", power.Value);
        }
    }
}