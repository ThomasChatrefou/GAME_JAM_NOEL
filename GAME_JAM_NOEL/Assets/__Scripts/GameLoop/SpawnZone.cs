using UnityEngine;

[CreateAssetMenu(fileName = "SpawnZone", menuName = "ScriptableObjects/GameLoop/SpawnZone")]
public class SpawnZone : ScriptableObject
{
    [Range(0f, .99f)] public float minMapPercentageX;
    [Range(.1f, 1f)]  public float maxMapPercentageX;
    [Range(0f, .99f)] public float minMapPercentageY;
    [Range(.1f, 1f)]  public float maxMapPercentageY;

    public Vector3 GetRandomPosition(Vector2 topLeft, Vector2 bottomRight)
    {
        Vector2 min = new Vector2(Mathf.Min(topLeft.x, bottomRight.x),
                                  Mathf.Min(topLeft.y, bottomRight.y));
        Vector2 max = new Vector2(Mathf.Max(topLeft.x, bottomRight.x),
                                  Mathf.Max(topLeft.y, bottomRight.y));

        float minX = (max.x - min.x) * minMapPercentageX + min.x;
        float maxX = (max.x - min.x) * maxMapPercentageX + min.x;
        float minY = (max.y - min.y) * minMapPercentageY + min.y;
        float maxY = (max.y - min.y) * maxMapPercentageY + min.y;

        return new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 0f);
    }
}
