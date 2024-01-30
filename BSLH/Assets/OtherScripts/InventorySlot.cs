using UnityEngine;
using UnityEngine.EventSystems;

namespace OtherScripts
{
    public class InventorySlot : MonoBehaviour, IDropHandler
    {
        public bool isWeaponSlot;
        public bool isArmorSlot;
        public bool isEssenceSlot;
        public bool isInventorySlot;
        public bool isDestroySlot;

        public void OnDrop(PointerEventData eventData)
        {
            if (transform.childCount != 0) return;
            var dropped = eventData.pointerDrag;
            var draggableItem = dropped.GetComponent<DraggableItem>();

            if (isInventorySlot)
            {
                draggableItem.parentAfterDrag = transform;
            }
            else if (draggableItem.isArmor && isArmorSlot)
            {
                draggableItem.parentAfterDrag = transform;
            } 
            else if (draggableItem.isWeapon && isWeaponSlot)
            {
                draggableItem.parentAfterDrag = transform;
            } 
            else if (draggableItem.isEssence && isEssenceSlot)
            {
                draggableItem.parentAfterDrag = transform;
            }
            else if (isDestroySlot)
            {
                draggableItem.parentAfterDrag = transform; 
                Destroy(draggableItem.gameObject);
            }
        }
    }
}
