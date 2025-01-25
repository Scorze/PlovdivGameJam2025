using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player"))
        {
            print("Passed checkpoint");
            GameStateManager.Instance.PassCheckpoint(gameObject, other.gameObject);
        }
    }
}
