using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager instance;
    public Transform currentRespawnPoint;

    public void SetRespawnPoint(Transform newRespawnPoint)
    {
        currentRespawnPoint = newRespawnPoint;
    }

    public void RespawnPlayer(GameObject player)
    {
        if (currentRespawnPoint != null && player != null)
        {
            player.transform.position = currentRespawnPoint.position;
        }
    }
}