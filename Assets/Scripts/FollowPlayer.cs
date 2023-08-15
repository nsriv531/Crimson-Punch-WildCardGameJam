using UnityEngine;
public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    private Vector3 offset;

    public float yScale = 1.0f;
    public bool followYAxis = true;
    public bool keepOffset = false;

    void Start()
    {
        if (player) Init();
    }

    public void Init()
    {
        offset = player.position - transform.position;
    }
    
    void Update()
    {
        if (!player) return;
        
        var p = player.transform.position * yScale;

        var selfPos = transform.position;
        selfPos.x = p.x;
        selfPos.z = p.z;

        if (followYAxis)
        {
            selfPos.y = p.y;
            if(keepOffset)
            {
                selfPos.y -= offset.y;
            }
            selfPos.y *= yScale;
        }

        if(keepOffset)
        {
            selfPos.x += offset.x;
            selfPos.z += offset.z;
        }

        transform.position = selfPos;
    }
}