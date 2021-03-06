// (c) Copyright HutongGames, LLC. All rights reserved.
// Author Eric Vander Wal www.dumbgamedev.com

using UnityEngine;
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMesh Pro Advanced")]
    [Tooltip("Enable Text Mesh Pro auto size text.")]
    public class enableTextmeshProAutoSizeText : ComponentAction<TextMeshPro>
    {
        [RequiredField]
        [CheckForComponent(typeof(TextMeshPro))]
        [Tooltip("Textmesh Pro component is required.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [TitleAttribute("Enable Size Text")]
        [Tooltip("Enable Auto Size Text.")]
        public FsmBool autoSizeText;

        [Tooltip("Max font size.")]
        public FsmFloat fontSizeMax;

        [Tooltip("Min font size.")]
        public FsmFloat fontSizeMin;

        [Tooltip("Line Spacing.")]
        public FsmFloat lineSpacing;

        [Tooltip("Width Adjustment.")]
        public FsmFloat widthAdjustment;

        [Tooltip("Check this box to preform this action every frame.")]
        public FsmBool everyFrame;

        public override void Reset()
        {
            gameObject = null;
            
            autoSizeText = new FsmBool(){UseVariable = true};
            fontSizeMax = new FsmFloat(){UseVariable = true};
            fontSizeMin = new FsmFloat(){UseVariable = true};
            lineSpacing = new FsmFloat(){UseVariable = true};
            widthAdjustment = new FsmFloat(){UseVariable = true};
            
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

            if (!autoSizeText.IsNone) this.cachedComponent.enableAutoSizing = autoSizeText.Value;
            if (!fontSizeMax.IsNone) this.cachedComponent.fontSizeMax = fontSizeMax.Value;
            if (!fontSizeMin.IsNone) this.cachedComponent.fontSizeMin = fontSizeMin.Value;
            if (!lineSpacing.IsNone) this.cachedComponent.lineSpacingAdjustment = lineSpacing.Value;
            if (!widthAdjustment.IsNone) this.cachedComponent.characterWidthAdjustment = widthAdjustment.Value;
        }
    }
}