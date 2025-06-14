using System.Collections;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{
    //public float activeTime = 2f;
    private MechaPlayer mechaPlayer;

    [Header("MeshRelated")]
    public Material mat;
    public float meshRefreshRate; //0.1f;
    public float meshDestroyDelay; //1f;
    public Transform positionToSpawn;

    private SkinnedMeshRenderer[] skinnedMeshRenderers;

    // Update is called once per frame

    private void Start()
    {
        mechaPlayer = GetComponentInParent<MechaPlayer>();
    }

    void Update()
    {
        positionToSpawn = mechaPlayer.transform;
        if (mechaPlayer.isBoosting)
        {
            StartCoroutine(ActiveTrail());
        }
        else
        {
            StopCoroutine(ActiveTrail());
        }
    }

    IEnumerator ActiveTrail()
    {
        while (mechaPlayer.isBoosting)
        {
            skinnedMeshRenderers ??= GetComponentsInChildren<SkinnedMeshRenderer>();

            for(int i = 0; i < skinnedMeshRenderers.Length; i++)
            {
                GameObject gObj = new()
                {
                    layer = 8
                };
                gObj.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);

                MeshRenderer mr = gObj.AddComponent<MeshRenderer>();
                MeshFilter mf = gObj.AddComponent<MeshFilter>();

                Mesh mesh = new();
                skinnedMeshRenderers[i].BakeMesh(mesh);

                mf.mesh = mesh;
                mr.material = mat;

                Destroy(gObj, meshDestroyDelay);
                yield return new WaitForSeconds(meshRefreshRate);
            }
        }
    }
}
