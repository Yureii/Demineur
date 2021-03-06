// (c) Copyright HutongGames, LLC. All rights reserved.
// Author Eric Vander Wal www.dumbgamedev.com

using UnityEngine;
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMesh Pro Basic")]
    [Tooltip("Set Text Mesh Pro Gradient Preset.")]
    public class setTextmeshProGradientPreset : ComponentAction<TextMeshPro>
    {
        [RequiredField]
        [CheckForComponent(typeof(TextMeshPro))]
        [Tooltip("Textmesh Pro component is required.")]
        public FsmOwnerDefault gameObject;

        [TitleAttribute("Gradient Preset")]
        [ObjectType(typeof(TMP_ColorGradient))]
        [Tooltip("Choose a gradient preset.")]
        public FsmObject gradientPreset;

        [Tooltip("Check this box to preform this action every frame.")]
        public FsmBool everyFrame;

        public override void Reset()
        {
            gameObject = null;
            gradientPreset = null;
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

            this.cachedComponent.colorGradientPreset = (TMP_ColorGradient) gradientPreset.Value;
        }
    }
}