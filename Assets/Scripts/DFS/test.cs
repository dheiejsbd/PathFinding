using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField] Transform[] trs;
    void Start()
    {
        
    }
    //https://gaussian37.github.io/math-algorithm-intersection_point/
    //https://www.itcodet.com/cpp/cpp-segment2-function-examples.html
    //http://www.findmean.com/%EC%88%98%ED%95%99/%EB%B2%A1%ED%84%B0/%EB%B2%A1%ED%84%B0%EC%9D%98-%EC%99%B8%EC%A0%81/
    // Update is called once per frame
    void Update()
    {
    }
    private bool LineCollision(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        

        if (ccw(p1, p3, p4) * ccw(p2, p3, p4) >= 0) return false;
        if (ccw(p3, p1, p2) * ccw(p4, p1, p2) >= 0) return false;

        
        return true;
    }

    /// <summary>
    /// output : 1 (counter clockwise), 0 (collinear), -1 (clockwise)
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <param name="v3"></param>
    /// <returns></returns>
    int ccw(Vector2 v1, Vector2 v2, Vector2 v3)
    {
        float cross_product = (v2.x - v1.x) * (v3.y - v1.y) - (v3.x - v1.x) * (v2.y - v1.y);

        if (cross_product > 0)
        {
            return 1;
        }
        else if (cross_product < 0)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(trs[0].position, trs[1].position);

        Vector2[] p = new Vector2[4];
        for (int i = 0; i < 4; i++)
        {
            p[i] = new Vector2(trs[i].position.x, trs[i].position.z);
        }

        Gizmos.color = LineCollision(p[0], p[1], p[2], p[3]) ? Color.red : Color.green;
        Gizmos.DrawLine(trs[2].position, trs[3].position);
    }
}
