using UnityEngine;


public class PathFinder : MonoBehaviour
{
    public static PathFinder Instance;

    public void SetClosenessToExit(Goo originPoint, int offset)
    {
        originPoint.m_exitCloseness = offset;
        foreach (Goo con in originPoint.m_connections)
        {
            if (con.m_exitCloseness == -1)
            {
                con.m_exitCloseness = ++offset;
                SetClosenessToExit(con, offset);
            }

        }
    }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}
