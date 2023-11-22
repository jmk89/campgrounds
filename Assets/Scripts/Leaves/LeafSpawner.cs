
using UnityEngine;

public class LeafSpawner : MonoBehaviour
{

    [SerializeField] float spawnTime = 1f;
    [SerializeField] Vector3 spawnPositionVariance = new Vector3(0f, 0f, 0f);
    [SerializeField] float spawnTimingVariance = 0f;
    [SerializeField] GameObject leaf = null;

    float nextSpawnTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        float timeModifier = Random.Range(-spawnTimingVariance, spawnTimingVariance);
        nextSpawnTime += Time.time + spawnTime + timeModifier;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextSpawnTime) {
            spawnLeaf();
        }
    }

    void spawnLeaf() {
        float timeModifier = Random.Range(-spawnTimingVariance, spawnTimingVariance);
        nextSpawnTime += spawnTime + timeModifier;
        Vector3 positionVariance = getRandomVector();
        Vector3 spawnPosition = new Vector3(transform.position.x + positionVariance.x, transform.position.y + positionVariance.y, + 0);
        Instantiate(leaf, spawnPosition, transform.rotation);
    }

    Vector3 getRandomVector() {
        float xVariance = Random.Range(-spawnPositionVariance.x, spawnPositionVariance.x);
        float yVariance = Random.Range(-spawnPositionVariance.y, spawnPositionVariance.y);
        return new Vector3(xVariance, yVariance, 0);
    }
}
