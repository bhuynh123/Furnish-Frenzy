using UnityEngine;

public class Button : Interactable
{
    [SerializeField]
    private PrefabSpawner spawnLocation;
    protected override void Interact()
    {
        Debug.Log("Calling RespawnPrefab");
        spawnLocation.RespawnPrefab();
    }
}
