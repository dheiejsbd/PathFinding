using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JPS
{

    public class PathFinder : MonoBehaviour
    {
        enum Shape
        {
            Box,
            Circle
        }
        [SerializeField] Shape shape;
        #region Speed
        [SerializeField] float speed = 3.5f;
        [SerializeField] float angularSpeed = 120;
        [SerializeField] float acceleration = 8;//가속도
        [SerializeField] float stopDistance = 0;
        [SerializeField] bool autoBraking = true;
        [SerializeField] [EnumFlags] GridLayer gridLayer;
        #endregion
        #region Box
        Vector2 boxSize;
        #endregion
        #region Circle
        float circleRadius;
        #endregion

        public Transform target;
        Vector3 targetPos;
        Vector3[] path;
        int pathIndex;
        void Awake()
        {

        }

        private void Update()
        {

            if (pathIndex == path.Length) return;
            Move();
            Rotate();
            pathChack();
        }
        private void FixedUpdate()
        {
            if (target == null || target.position == targetPos) return;
            targetPos = target.position;
            path = JPS.instance.FindPath(gridLayer, transform.position, targetPos);
            pathIndex = 0;
        }

        void pathChack()
        {
            if (Vector3.Distance(transform.position, path[pathIndex]) <= stopDistance)
            {
                pathIndex++;
                if (pathIndex == path.Length)
                {
                    //도착처리
                }
            }

        }
        void Move()
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            //transform.position = 
        }
        void Rotate()
        {
            transform.LookAt(path[pathIndex]);
            //transform.rotation = Quaternion.Lerp(transform.rotation, );
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            for (int i = 1; i < path.Length; i++)
            {
                Gizmos.DrawLine(path[i], path[i - 1]);
            }
        }
    }

}