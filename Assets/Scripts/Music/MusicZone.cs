using UnityEngine;

public class MusicZone : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;

    void OnTriggerEnter(Collider other) {
        if (!audioSource.isPlaying && other.tag == "Player") {
            audioSource.Play();
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.tag == "Player") {
            audioSource.Stop();
        }
    }
}
