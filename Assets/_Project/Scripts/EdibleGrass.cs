using UnityEngine;

public class EdibleGrass : MonoBehaviour
{
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player"))
        {
            print("Eaten grass");
            GameStateManager.Instance.EatGrass(gameObject, other.gameObject);
        }
    }
}
