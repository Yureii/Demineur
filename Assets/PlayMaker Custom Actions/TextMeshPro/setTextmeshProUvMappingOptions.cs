// (c) Copyright HutongGames, LLC. All rights reserved.
// Author Eric Vander Wal www.dumbgamedev.com

using UnityEngine;
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMesh Pro Advanced")]
    [Tooltip("Set Text Mesh Pro Texture Mapping Options.")]
    public class setTextmeshProUvMappingOptions : ComponentAction<TextMeshPro>
    {
        [RequiredField]
        [CheckForComponent(typeof(TextMeshPro))]
        [Tooltip("Textmesh Pro component is required.")]
        public FsmOwnerDefault gameObject;


        [ObjectType(typeof(TextureMappingOptions))]
        [TitleAttribute("Mapping Options Horizontal")]
        [Tooltip("Texture Mapping Options Horizontal")]
        public FsmEnum UvMappingHorizontal;

        [ObjectType(typeof(TextureMappingOptions))]
        [TitleAttribute("Mapping Options Vertical")]
        [Tooltip("Texture Mapping Options Vertical")]
        public FsmEnum UvMappingVertical;

        [Tooltip("Enable overflow and wrapping mode.")]
        public FsmBool everyFrame;
        
        public override void Reset()
        {
            gameObject = null;
            
            UvMappingHorizontal = new FsmEnum(){UseVariable = true};
            UvMappingVertical = new FsmEnum(){UseVariable = true};;
            
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

            if(!UvMappingHorizontal.IsNone) this.cachedComponent.horizontalMapping = (TextureMappingOptions) UvMappingHorizontal.Value;
            if(!UvMappingVertical.IsNone) this.cachedComponent.verticalMapping = (TextureMappingOptions) UvMappingVertical.Value;
        }
    }
}