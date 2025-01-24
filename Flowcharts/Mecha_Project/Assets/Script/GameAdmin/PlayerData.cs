using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PlayerData
{
    public int health;
    public string sceneName;
    public float[] position;
    //public Transform PlayerPost;

    public PlayerData (MechaPlayer player)
    {
        health = player.Health;
        sceneName = SceneManager.GetActiveScene().name;
        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;
    }
}
