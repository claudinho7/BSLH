using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OtherScripts
{
    public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public Transform parentAfterDrag;
        public Image image;

        public bool isWeapon;
        public bool isArmor;
        public bool isEssence;
        public bool isLeather;
        public bool isBone;

        public void OnBeginDrag(PointerEventData eventData)
        {
            var parent = transform.parent;
            parentAfterDrag = parent;
            var target = parent.parent;
            transform.SetParent(target);
            transform.SetAsLastSibling();
            image.raycastTarget = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            transform.SetParent(parentAfterDrag);
            image.raycastTarget = true;
        }
    }
}
