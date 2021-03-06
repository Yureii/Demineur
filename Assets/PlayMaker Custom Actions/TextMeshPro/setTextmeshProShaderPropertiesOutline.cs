// (c) Copyright HutongGames, LLC. All rights reserved.
// Author Eric Vander Wal www.dumbgamedev.com

using UnityEngine;
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMesh Pro Shader")]
    [Tooltip("Set Text Mesh Pro face shaders.")]
    public class setTextmeshProShaderPropertiesOutline : ComponentAction<TextMeshPro>
    {
        [RequiredField]
        [CheckForComponent(typeof(TextMeshPro))]
        [Tooltip("Textmesh Pro component is required.")]
        public FsmOwnerDefault gameObject;

        [TitleAttribute("Enable Outline Shader")]
        public FsmBool enableOutline;

        public FsmColor outlineColor;

        public FsmTexture texture;

        public FsmVector2 textureTiling;

        public FsmVector2 textureOffset;

        [HasFloatSlider(-5, 5)]
        public FsmFloat speedX;

        public FsmFloat speedY;

        [HasFloatSlider(0, 1)]
        public FsmFloat thickness;

        [Tooltip("Check this box to perform this action every frame.")]
        public FsmBool everyFrame;

        public override void Reset()
        {
            gameObject = null;
            
            enableOutline = new FsmBool(){UseVariable = true};
            outlineColor = new FsmColor(){UseVariable = true};
            thickness = new FsmFloat(){UseVariable = true};
            everyFrame = new FsmBool(){UseVariable = true};
            texture = new FsmTexture(){UseVariable = true};
            speedX = new FsmFloat(){UseVariable = true};
            speedY = new FsmFloat(){UseVariable = true};

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

            if (enableOutline.Value == true)
            {
                this.cachedComponent.fontSharedMaterial.EnableKeyword("OUTLINE_ON");
            }
            else
            {
                this.cachedComponent.fontSharedMaterial.DisableKeyword("OUTLINE_ON");
            }
            
            if(!outlineColor.IsNone) this.cachedComponent.fontSharedMaterial.SetColor("_OutlineColor", outlineColor.Value);
            if(!thickness.IsNone) this.cachedComponent.fontSharedMaterial.SetFloat("_OutlineWidth", thickness.Value);
            if(!texture.IsNone) this.cachedComponent.fontSharedMaterial.SetTexture("_OutlineTex", texture.Value);
            if(!speedX.IsNone) this.cachedComponent.fontSharedMaterial.SetFloat("_OutlineUVSpeedX", speedX.Value);
            if(!speedY.IsNone) this.cachedComponent.fontSharedMaterial.SetFloat("_OutlineUVSpeedY", speedY.Value);
            if(!textureOffset.IsNone) this.cachedComponent.fontSharedMaterial.SetTextureOffset("_OutlineTex", textureOffset.Value);
            if(!textureTiling.IsNone) this.cachedComponent.fontSharedMaterial.SetTextureScale("_OutlineTex", textureTiling.Value);
        }
    }
}