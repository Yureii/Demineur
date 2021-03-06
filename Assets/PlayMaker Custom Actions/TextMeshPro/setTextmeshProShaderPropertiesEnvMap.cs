// (c) Copyright HutongGames, LLC. All rights reserved.
// Author Eric Vander Wal www.dumbgamedev.com

using UnityEngine;
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMesh Pro Shader")]
    [Tooltip("Set Text Mesh Pro env map shaders.")]
    public class setTextmeshProShaderPropertiesEnvMap : ComponentAction<TextMeshPro>
    {
        [RequiredField]
        [CheckForComponent(typeof(TextMeshPro))]
        [Tooltip("Textmesh Pro component is required.")]
        public FsmOwnerDefault gameObject;

        public FsmColor faceColor;
        
        public FsmColor outlineColor;

        [ActionSection("Texture")]
        public FsmTexture texture;

        public FsmVector3 rotation;

        [Tooltip("Check this box to preform this action every frame.")]
        public FsmBool everyFrame;
        
        public override void Reset()
        {
            gameObject = null;
            
            outlineColor = new FsmColor(){UseVariable = true};
            faceColor = new FsmColor(){UseVariable = true};
            texture = new FsmTexture(){UseVariable = true};
            rotation = new FsmVector3(){UseVariable = true};
            
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

            if(!rotation.IsNone) this.cachedComponent.fontSharedMaterial.SetVector("_EnvMatrixRotation", rotation.Value);
            if(!texture.IsNone) this.cachedComponent.fontSharedMaterial.SetTexture("_Cube", texture.Value);
            if(!faceColor.IsNone) this.cachedComponent.fontSharedMaterial.SetColor("_ReflectFaceColor", faceColor.Value);
            if(!outlineColor.IsNone) this.cachedComponent.fontSharedMaterial.SetColor("_ReflectOutlineColor", outlineColor.Value);
        }
    }
}