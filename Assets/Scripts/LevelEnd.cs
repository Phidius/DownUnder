using UnityEngine;
public class LevelEnd : MonoBehaviour
{
    private bool levelEnd = false;
    void OnTriggerEnter(Collider collider)
    {
        if (!levelEnd && collider.tag == "Player")
        {
            levelEnd = true;
            GameManager.Instance.FinishLevel(collider.GetComponent<PlayerController>());
            GetComponent<AudioSource>().Play();
        }
    }
}
