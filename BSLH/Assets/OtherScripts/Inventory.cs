using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OtherScripts
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private List<InventorySlot> inventorySlots;
        [SerializeField] private int bones;
        [SerializeField] private int leathers;
        public TextMeshProUGUI[] leatherCounter;
        public TextMeshProUGUI[] boneCounter;

        public Image sword;
        public Image spear;
        public Image hammer;
        public Image xbow;
        public Image lightArmor;
        public Image mediumArmor;
        public Image heavyArmor;

        private void AddItem(Image item)
        {
            //iterate through all slots and find the first available one
            foreach (var slot in inventorySlots.Where(slot => slot.transform.childCount == 0))
            {
                // Attach the item to the slot and exit the loop
                Instantiate(item, slot.transform);
                return;
            }

            // If all slots are occupied, you can handle this case as needed
            Debug.LogWarning("No available slots to add item!");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Item")) return;
            if (other.gameObject.GetComponentInChildren<DraggableItem>().isLeather)
            {
                leathers += 3;
                leatherCounter[0].text = leathers.ToString();
                leatherCounter[1].text = leathers.ToString();
                Destroy(other.gameObject);
            }
            else if (other.gameObject.GetComponentInChildren<DraggableItem>().isBone)
            {
                bones += 3;
                boneCounter[0].text = bones.ToString();
                boneCounter[1].text = bones.ToString();
                Destroy(other.gameObject);
            }
            else
            {
                AddItem(other.GetComponentInChildren<Image>());
                Destroy(other.gameObject);
            }
        }

        public void CraftSword()
        {
            if (leathers < 2 || bones < 5) return;
            AddItem(sword);
            leathers -= 2;
            bones -= 5;
            boneCounter[0].text = bones.ToString();
            leatherCounter[0].text = leathers.ToString();
            boneCounter[1].text = bones.ToString();
            leatherCounter[1].text = leathers.ToString();
        }

        public void CraftSpear()
        {
            if (leathers < 3 || bones < 4) return;
            AddItem(spear);
            leathers -= 3;
            bones -= 4;
            boneCounter[0].text = bones.ToString();
            leatherCounter[0].text = leathers.ToString();
            boneCounter[1].text = bones.ToString();
            leatherCounter[1].text = leathers.ToString();
        }

        public void CraftHammer()
        {
            if (leathers < 2 || bones < 5) return;
            AddItem(hammer);
            leathers -= 2;
            bones -= 5;
            boneCounter[0].text = bones.ToString();
            leatherCounter[0].text = leathers.ToString();
            boneCounter[1].text = bones.ToString();
            leatherCounter[1].text = leathers.ToString();
        }

        public void CraftXBow()
        {
            if (leathers < 3 || bones < 3) return;
            AddItem(xbow);
            leathers -= 3;
            bones -= 3;
            boneCounter[0].text = bones.ToString();
            leatherCounter[0].text = leathers.ToString();
            boneCounter[1].text = bones.ToString();
            leatherCounter[1].text = leathers.ToString();
        }
        
        public void CraftLightArmor()
        {
            if (leathers < 1 || bones < 0) return;
            AddItem(lightArmor);
            leathers -= 1;
            boneCounter[0].text = bones.ToString();
            leatherCounter[0].text = leathers.ToString();
            boneCounter[1].text = bones.ToString();
            leatherCounter[1].text = leathers.ToString();

        }
        public void CraftMediumArmor()
        {
            if (leathers < 3 || bones < 1) return;
            AddItem(mediumArmor);
            leathers -= 3;
            bones -= 1;
            boneCounter[0].text = bones.ToString();
            leatherCounter[0].text = leathers.ToString();
            boneCounter[1].text = bones.ToString();
            leatherCounter[1].text = leathers.ToString();
        }
        
        public void CraftHeavyArmor()
        {
            if (leathers < 5 || bones < 3) return;
            AddItem(heavyArmor);
            leathers -= 5;
            bones -= 3;
            boneCounter[0].text = bones.ToString();
            leatherCounter[0].text = leathers.ToString();
            boneCounter[1].text = bones.ToString();
            leatherCounter[1].text = leathers.ToString();
        }
    }
}
