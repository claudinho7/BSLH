using UnityEngine;

namespace OtherScripts
{
    public class DestroyObject : MonoBehaviour
    {
        public void DestroyParent()
        {
            Destroy(gameObject.transform.parent.gameObject);
        }
    }
}
