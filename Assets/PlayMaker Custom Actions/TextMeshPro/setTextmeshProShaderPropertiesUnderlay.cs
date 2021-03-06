// (c) Copyright HutongGames, LLC. All rights reserved.
// Author Eric Vander Wal www.dumbgamedev.com

using UnityEngine;
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMesh Pro Shader")]
    [Tooltip("Set Text Mesh Pro underlay shaders.")]
    public class setTextmeshProShaderPropertiesUnderlay : ComponentAction<TextMeshPro>
    {
        [RequiredField]
        [CheckForComponent(typeof(TextMeshPro))]
        [Tooltip("Textmesh Pro component is required.")]
        public FsmOwnerDefault gameObject;

        [TitleAttribute("Enable Underlay Shader")]
        public FsmBool enableUnderLay;

        [TitleAttribute("Enable Inner Underlay Shader")]
        public FsmBool enableInnerUnderLay;

        public FsmColor underlayColor;

        [HasFloatSlider(-1, 1)]
        public FsmFloat offsetX;

        [HasFloatSlider(-1, 1)]
        public FsmFloat offsetY;

        [HasFloatSlider(-1, 1)]
        public FsmFloat dilate;

        [HasFloatSlider(0, 1)]
        public FsmFloat softness;

        [Tooltip("Check this box to preform this action every frame.")]
        public FsmBool everyFrame;

        public override void Reset()
        {
            gameObject = null;
            
            underlayColor = new FsmColor(){UseVariable = true};
            offsetX = new FsmFloat(){UseVariable = true};
            offsetY = new FsmFloat(){UseVariable = true};
            dilate = new FsmFloat(){UseVariable = true};
            softness = new FsmFloat(){UseVariable = true};
            enableUnderLay = new FsmBool(){UseVariable = true};
            enableInnerUnderLay = new FsmBool(){UseVariable = true};
            
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

            if (enableUnderLay.Value == true)
            {
                this.cachedComponent.fontSharedMaterial.EnableKeyword("UNDERLAY_ON");
            }
            else
            {
                this.cachedComponent.fontSharedMaterial.DisableKeyword("UNDERLAY_ON");
            }
            
            if (enableInnerUnderLay.Value == true)
            {
                this.cachedComponent.fontSharedMaterial.EnableKeyword("UNDERLAY_INNER");
            }
            else
            {
                this.cachedComponent.fontSharedMaterial.DisableKeyword("UNDERLAY_INNER");
            }

            if(!underlayColor.IsNone) this.cachedComponent.fontSharedMaterial.SetColor("_UnderlayColor", underlayColor.Value);
            if(!offsetX.IsNone) this.cachedComponent.fontSharedMaterial.SetFloat("_UnderlayOffsetX", offsetX.Value);
            if(!offsetY.IsNone) this.cachedComponent.fontSharedMaterial.SetFloat("_UnderlayOffsetY", offsetY.Value);
            if(!dilate.IsNone) this.cachedComponent.fontSharedMaterial.SetFloat("_UnderlayDilate", dilate.Value);
            if(!softness.IsNone) this.cachedComponent.fontSharedMaterial.SetFloat("_UnderlaySoftness", softness.Value);
        }
    }
}