// (c) Copyright HutongGames, LLC. All rights reserved.
// Author Eric Vander Wal www.dumbgamedev.com

using UnityEngine;
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMesh Pro Advanced")]
    [Tooltip("Set sorting layers for Text Mesh Pro.")]
    public class setTextmeshSortingLayer : ComponentAction<TextMeshPro>
    {
        [RequiredField]
        [CheckForComponent(typeof(TextMeshPro))]
        [Tooltip("Textmesh Pro component is required.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Set Geometry Sorting Order.")]
        [TitleAttribute("Geometry Sorting")]
        [ObjectType(typeof(VertexSortingOrder))]
        public FsmEnum geoSorting;

        [Tooltip("Check this box to preform this action every frame.")]
        public FsmBool everyFrame;

        private Vector4 margin4;

        public override void Reset()
        {
            gameObject = null;
            geoSorting = null;
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

            this.cachedComponent.geometrySortingOrder = (VertexSortingOrder) geoSorting.Value;
        }
    }
}