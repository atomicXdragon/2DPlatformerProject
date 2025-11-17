using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed;
    private float currentPositionY;
    private Vector3 velocity = Vector3.zero;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentPositionY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(transform.position.x, currentPositionY, transform.position.z), ref velocity, speed);
    }

    public void MoveToNewRoom(Transform newRoom)
    {
        currentPositionY = newRoom.position.y;
    }
}
