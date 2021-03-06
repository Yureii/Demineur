// (c) Copyright HutongGames, LLC. All rights reserved.
// Author Eric Vander Wal www.dumbgamedev.com

using UnityEngine;
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMesh Pro Shader")]
    [Tooltip("Set Text Mesh Pro lighting shaders.")]
    public class setTextmeshProShaderPropertiesLighting : ComponentAction<TextMeshPro>
    {
        [RequiredField]
        [CheckForComponent(typeof(TextMeshPro))]
        [Tooltip("Textmesh Pro component is required.")]
        public FsmOwnerDefault gameObject;

        [HasFloatSlider(0, 6.2831853f)]
        public FsmFloat lightAngle;

        public FsmColor specularColor;

        [HasFloatSlider(0, 4f)]
        public FsmFloat specularPower;

        [HasFloatSlider(5, 15.0f)]
        public FsmFloat reflectivityPower;

        [HasFloatSlider(0, 1f)]
        public FsmFloat diffuseShadow;

        [HasFloatSlider(1, 0)]
        public FsmFloat ambientShadow;

        [Tooltip("Check this box to preform this action every frame.")]
        public FsmBool everyFrame;

        public override void Reset()
        {
            gameObject = null;
            
            specularPower = new FsmFloat(){UseVariable = true};
            specularColor = new FsmColor(){UseVariable = true};
            reflectivityPower = new FsmFloat(){UseVariable = true};
            diffuseShadow = new FsmFloat(){UseVariable = true};
            lightAngle = new FsmFloat(){UseVariable = true};
            ambientShadow = new FsmFloat(){UseVariable = true};
            
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

            if(!lightAngle.IsNone) this.cachedComponent.fontSharedMaterial.SetFloat("_LightAngle", lightAngle.Value);
            if(!specularColor.IsNone) this.cachedComponent.fontSharedMaterial.SetColor("_SpecularColor", specularColor.Value);
            if(!specularPower.IsNone) this.cachedComponent.fontSharedMaterial.SetFloat("_SpecularPower", specularPower.Value);
            if(!lightAngle.IsNone) this.cachedComponent.fontSharedMaterial.SetFloat("_LightAngle", lightAngle.Value);
            if(!reflectivityPower.IsNone) this.cachedComponent.fontSharedMaterial.SetFloat("_Reflectivity", reflectivityPower.Value);
            if(!diffuseShadow.IsNone) this.cachedComponent.fontSharedMaterial.SetFloat("_Diffuse", diffuseShadow.Value);
            if(!ambientShadow.IsNone) this.cachedComponent.fontSharedMaterial.SetFloat("_Ambient", ambientShadow.Value);
        }
    }
}