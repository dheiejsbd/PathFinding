using UnityEditor;
using UnityEngine;

namespace JPS
{
    public class JPSObstacle : MonoBehaviour
    {
        bool staticObj = true;
        [SerializeField] Vector3 size = Vector3.one;
        public void Start()
        {
            Vector2 center = transform.position.ToVector2();
            Vector2 p1 = center - size.ToVector2() / 2;
            Vector2 p2 = center + size.ToVector2() / 2;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, size);
        }
    }
}