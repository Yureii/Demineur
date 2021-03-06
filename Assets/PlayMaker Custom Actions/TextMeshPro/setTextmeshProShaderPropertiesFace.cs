// (c) Copyright HutongGames, LLC. All rights reserved.
// Author Eric Vander Wal www.dumbgamedev.com

using UnityEngine;
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMesh Pro Shader")]
    [Tooltip("Set Text Mesh Pro face shaders.")]
    public class setTextmeshProShaderPropertiesFace : ComponentAction<TextMeshPro>
    {
        [RequiredField]
        [CheckForComponent(typeof(TextMeshPro))]
        [Tooltip("Textmesh Pro component is required.")]
        public FsmOwnerDefault gameObject;
        
        public FsmColor faceColor;
        
        public FsmTexture texture;
        
        public FsmVector2 textureTiling;

        public FsmVector2 textureOffset;
        
        [HasFloatSlider(-5, 5)]
        public FsmFloat speedX;

        [HasFloatSlider(-5, 5)]
        public FsmFloat speedY;
        
        [HasFloatSlider(0, 1)]
        public FsmFloat softness;

        [HasFloatSlider(-1, 1)]
        public FsmFloat dilate;

        [Tooltip("Check this box to preform this action every frame.")]
        public FsmBool everyFrame;

        public override void Reset()
        {
            gameObject = null;
            
            faceColor = new FsmColor(){UseVariable = true};
            softness = new FsmFloat(){UseVariable = true};
            dilate = new FsmFloat() {UseVariable = true};
            speedX = new FsmFloat() {UseVariable = true};
            speedY = new FsmFloat(){UseVariable = true};
            texture = new FsmTexture() {UseVariable = true};
            
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

            if(!faceColor.IsNone) this.cachedComponent.fontSharedMaterial.SetColor("_FaceColor", faceColor.Value);
            if(!dilate.IsNone) this.cachedComponent.fontSharedMaterial.SetFloat("_FaceDilate", dilate.Value);
            if(!softness.IsNone) this.cachedComponent.fontSharedMaterial.SetFloat("_OutlineSoftness", softness.Value);
            if(!speedX.IsNone) this.cachedComponent.fontSharedMaterial.SetFloat("_FaceUVSpeedX", speedX.Value);
            if(!speedY.IsNone) this.cachedComponent.fontSharedMaterial.SetFloat("_FaceUVSpeedY", speedY.Value);
            if(!texture.IsNone) this.cachedComponent.fontSharedMaterial.SetTexture("_FaceTex", texture.Value);
            if(!textureOffset.IsNone) this.cachedComponent.fontSharedMaterial.SetTextureOffset("_FaceTex", textureOffset.Value);
            if(!textureTiling.IsNone) this.cachedComponent.fontSharedMaterial.SetTextureScale("_FaceTex", textureTiling.Value);
        }
    }
}