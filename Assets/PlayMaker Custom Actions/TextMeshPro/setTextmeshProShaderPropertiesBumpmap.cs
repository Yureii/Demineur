// (c) Copyright HutongGames, LLC. All rights reserved.
// Author Eric Vander Wal www.dumbgamedev.com

using UnityEngine;
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMesh Pro Shader")]
    [Tooltip("Set Text Mesh Pro bump map shaders.")]
    public class setTextmeshProShaderPropertiesBumpmap : ComponentAction<TextMeshPro>
    {
        [RequiredField]
        [CheckForComponent(typeof(TextMeshPro))]
        [Tooltip("Textmesh Pro component is required.")]
        public FsmOwnerDefault gameObject;
        
        public FsmTexture texture;

        [HasFloatSlider(0, 1)]
        public FsmFloat face;

        [HasFloatSlider(0, 1)]
        public FsmFloat outline;

        [Tooltip("Check this box to preform this action every frame.")]
        public FsmBool everyFrame;

        public override void Reset()
        {
            gameObject = null;
            
            face = new FsmFloat(){UseVariable = true};
            outline = new FsmFloat(){UseVariable = true};
            texture = new FsmTexture(){UseVariable = true};
            
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

            if(!face.IsNone) this.cachedComponent.fontSharedMaterial.SetFloat("_BumpFace", face.Value);
            if(!outline.IsNone) this.cachedComponent.fontSharedMaterial.SetFloat("_BumpOutline", outline.Value);
            if(!texture.IsNone) this.cachedComponent.fontSharedMaterial.SetTexture("_BumpMap", texture.Value);
        }
    }
}