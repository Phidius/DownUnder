using UnityEngine;
public class LevelEnd : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        GameManager.Instance.FinishLevel(collider.GetComponent<PlayerController>());
    }
}
