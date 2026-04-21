using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HeldItemPhysics : MonoBehaviour
{
    private Rigidbody rb;
    public float moveForce = 250f;
    public float dragAmount = 15f;

    // We only want this script active when the item is being held
    [HideInInspector] public bool isEquipped = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // Only apply physics drag if the item is currently equipped
        if (isEquipped && Inventory.instance.dropPoint != null)
        {
            // Calculate direction and distance to the target equip point
            Vector3 targetPos = Inventory.instance.dropPoint.position;
            Vector3 directionToTarget = targetPos - transform.position;
            float distance = directionToTarget.magnitude;

            // Apply velocity towards the target
            rb.linearVelocity = directionToTarget.normalized * (distance * moveForce * Time.fixedDeltaTime);

            // Match camera rotation smoothly
            Quaternion targetRotation = Inventory.instance.dropPoint.rotation;
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, 15f * Time.fixedDeltaTime));
        }
    }

    public void OnEquip()
    {
        isEquipped = true;
        rb.useGravity = false;

        // --- ADD THESE TWO LINES ---
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.linearDamping = dragAmount;
        rb.angularDamping = dragAmount;

        int heldLayer = LayerMask.NameToLayer("HeldItems");
        SetLayerRecursively(gameObject, heldLayer);
    }

    public void OnUnequip()
    {
        isEquipped = false;
        rb.useGravity = true;

        rb.linearDamping = 0f;
        rb.angularDamping = 0.05f;

        // Get the integer ID of the layer
        int interactableLayer = LayerMask.NameToLayer("Interactable");
        // Apply it to the parent and ALL children
        SetLayerRecursively(gameObject, interactableLayer);
    }

    // --- NEW HELPER METHOD ---
    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        // 1. Change the layer of the current object
        obj.layer = newLayer;

        // 2. Loop through all immediate children and run this function on them too
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}