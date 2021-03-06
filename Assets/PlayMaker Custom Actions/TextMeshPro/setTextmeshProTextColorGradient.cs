// (c) Copyright HutongGames, LLC. All rights reserved.
// Author Eric Vander Wal www.dumbgamedev.com

using UnityEngine;
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMesh Pro Basic")]
    [Tooltip("Set Text Mesh Pro text color using a gradient.")]
    public class setTextmeshProTextColorGradient : ComponentAction<TextMeshPro>
    {
        [RequiredField]
        [CheckForComponent(typeof(TextMeshPro))]
        [Tooltip("Textmesh Pro component is required.")]
        public FsmOwnerDefault gameObject;

        public FsmColor topLeft;
        public FsmColor topRight;
        public FsmColor bottomLeft;
        public FsmColor bottomRight;

        private Color _bottomRight;

        [Tooltip("Check this box to preform this action every frame.")]
        public FsmBool everyFrame;

        VertexGradient cG;
        
        public override void Reset()
        {
            gameObject = null;
            
            topLeft = new FsmColor(){UseVariable = true};
            topRight = new FsmColor(){UseVariable = true};
            bottomRight = new FsmColor(){UseVariable = true};
            bottomLeft = new FsmColor(){UseVariable = true};
            
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
            
            cG = this.cachedComponent.colorGradient;

            if (!topLeft.IsNone) cG.topLeft = topLeft.Value;
            if (!topRight.IsNone)  cG.topRight = topRight.Value;
            if (!bottomLeft.IsNone)  cG.bottomLeft = bottomLeft.Value;
            if (!bottomRight.IsNone)  cG.bottomRight = bottomRight.Value;

            this.cachedComponent.colorGradient = cG;
        }
    }
}