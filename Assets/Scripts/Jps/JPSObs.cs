using UnityEditor;
using UnityEngine;

namespace JPS
{
    public class JPSObs : MonoBehaviour
    {
        Shape shape = Shape.Box;
        [SerializeField]Grid grid;
        [SerializeField]Vector3 size;
        public void Start()
        {
            Vector2 center = transform.position.ToVector2();
            Vector2 p1 = center - size.ToVector2() / 2;
            Vector2 p2 = center + size.ToVector2() / 2;
            grid.Obs(p1, p2);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawCube(transform.position, size);
        }
    }
}