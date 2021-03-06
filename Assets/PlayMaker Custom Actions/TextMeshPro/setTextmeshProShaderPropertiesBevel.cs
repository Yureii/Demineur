// (c) Copyright HutongGames, LLC. All rights reserved.
// Author Eric Vander Wal www.dumbgamedev.com

using UnityEngine;
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMesh Pro Shader")]
    [Tooltip("Set Text Mesh Pro bevel shaders.")]
    public class setTextmeshProShaderPropertiesBevel : ComponentAction<TextMeshPro>
    {
        [RequiredField]
        [CheckForComponent(typeof(TextMeshPro))]
        [Tooltip("Textmesh Pro component is required.")]
        public FsmOwnerDefault gameObject;

        [TitleAttribute("Enable Bevel")]
        public FsmBool enableBevel;

        [HasFloatSlider(0, 1)]
        public FsmFloat amount;

        [HasFloatSlider(-0.5f, 0.5f)]
        public FsmFloat offset;

        [HasFloatSlider(-0.5f, 0.5f)]
        public FsmFloat width;

        [HasFloatSlider(0, 1)]
        public FsmFloat roundness;

        [HasFloatSlider(0, 1)]
        public FsmFloat clamp;

        [TitleAttribute("Enable for Inner Bevel")]
        public FsmBool innerBevel;

        [Tooltip("Check this box to preform this action every frame.")]
        public FsmBool everyFrame;

        public override void Reset()
        {
            enableBevel = null;
            
            amount = null;
            innerBevel = null;
            offset = null;
            width = null;
            roundness = null;
            clamp = null;
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

            if (enableBevel.Value == true)
            {
                this.cachedComponent.fontSharedMaterial.EnableKeyword("BEVEL_ON");
            }
            else
            {
                this.cachedComponent.fontSharedMaterial.DisableKeyword("BEVEL_ON");
            }

            this.cachedComponent.fontSharedMaterial.SetFloat("_ShaderFlags", innerBevel.Value?1:0);
            
            this.cachedComponent.fontSharedMaterial.SetFloat("_Bevel", amount.Value);
            this.cachedComponent.fontSharedMaterial.SetFloat("_BevelOffset", offset.Value);
            this.cachedComponent.fontSharedMaterial.SetFloat("_BevelWidth", width.Value);
            this.cachedComponent.fontSharedMaterial.SetFloat("_BevelClamp", clamp.Value);
            this.cachedComponent.fontSharedMaterial.SetFloat("_BevelRoundness", roundness.Value);
        }
    }
}