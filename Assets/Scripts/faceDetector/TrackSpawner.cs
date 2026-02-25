using System.Collections.Generic;
using UnityEngine;

public class TrackSpawner : MonoBehaviour
{
    public GameObject trackPrefab;
    public GameObject obstaclePrefab;
    public GameObject collectiblePrefab;


    public int initialPieces = 5;
    public float pieceLength = 4f;
    public float speed = 2f;
    public float obstacleChance = 0.1f;
    public float collectibleChance = 0.1f;


    private Queue<GameObject> trackQueue = new Queue<GameObject>();
    private float spawnZ = 0f;

    void Start()
    {
        for (int i = 0; i < initialPieces; i++)
        {
            SpawnPiece();
        }
    }

    void Update()
    {
        foreach (GameObject piece in trackQueue)
        {
            piece.transform.Translate(Vector3.back * speed * Time.deltaTime);
        }


        if (trackQueue.Peek().transform.position.z < -pieceLength)
        {
            RemovePiece();
            SpawnPiece();
        }

            
    }

    void SpawnPiece()
    {
        GameObject piece = Instantiate(trackPrefab);
        float zPos = 0f;

        if (trackQueue.Count > 0)
        {
            GameObject lastPiece = null;
            foreach (var p in trackQueue)
                lastPiece = p;
            zPos = lastPiece.transform.position.z + pieceLength;
        }

        piece.transform.position = new Vector3(0, -0.3f, zPos);

        float? obstacleLane = null;

        if (Random.value < obstacleChance)
        {
            obstacleLane = SpawnObstacle(piece.transform);
        }
        if (Random.value < collectibleChance)
        {
            SpawnCollectible(piece.transform, obstacleLane);
        }
        trackQueue.Enqueue(piece);
    }

    float SpawnObstacle(Transform parent)
    {
        float[] lanes = { -0.3f, 0f, 0.3f }; // Posibles posiciones en X
        float x = lanes[Random.Range(0, lanes.Length)];

        GameObject obstacle = Instantiate(obstaclePrefab);
        obstacle.transform.SetParent(parent);
        obstacle.transform.localPosition = new Vector3(x, 4.5f, 0);
        Debug.Log("Spawning obstacle");
        return x;
    }

    void SpawnCollectible(Transform parent, float? occupiedLane)
    {
        List<float> lanes = new List<float> { -0.5f, 0f, 0.5f };

        if (occupiedLane.HasValue)
            lanes.Remove(occupiedLane.Value);

        float x = lanes[Random.Range(0, lanes.Count)];

        GameObject collectible = Instantiate(collectiblePrefab);
        collectible.transform.SetParent(parent);
        collectible.transform.localPosition = new Vector3(x, 1f, 0);
    }

    void RemovePiece()
    {
        GameObject oldPiece = trackQueue.Dequeue();
        Destroy(oldPiece);
    }

}
