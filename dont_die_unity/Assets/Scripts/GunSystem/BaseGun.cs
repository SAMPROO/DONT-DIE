using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class BaseGun : MonoBehaviour
{
    public bool testFire;
    public Vector3 holdPosition;

    // Transparent representation of ammunition used in this gun
    [SerializeField] private GunTypeGhostDisplay ghostDisplay;

    // Image to display in icon and other places. Expose publicly
    // as get-only field
    [SerializeField] private Sprite hudIcon;
    public Sprite HudIcon => hudIcon;

    // How long to wait before destroyed when dropped and no ammo.
    private const float fiveSecondRule = 5;
    public bool infiniteAmmo;

    [SerializeField] private int ammo = 0;
    [SerializeField] public int Ammo
    {
        get
        {
            return infiniteAmmo ? int.MaxValue : ammo;
        }

        set
        {
            ammo = value;
        }
    }

    protected bool isCarried;

    private FixedJoint joint;

    /* if you need Update use this in your script: (works for other funtions aswell)
     * public override void Update()
     * {
     *      base.Update();
     *      
     *      // your code...
     * }
     */
    public virtual void Update()
    {
        // Destroy this gameobject if not carried and has no ammo after a time in seconds
        if (!isCarried && !infiniteAmmo && Ammo <= 0)
        {
            Invoke(nameof(Destroy), fiveSecondRule);
        }
        else if (IsInvoking(nameof(Destroy)))
        {
            CancelInvoke(nameof(Destroy));
        }

#if UNITY_EDITOR
        if (testFire)
        {
            Use();
            testFire = false;
        }
#endif
    }


    public abstract void Use();

    public virtual void StartCarrying(Rigidbody connectedBody, float angle)
    {
        if (joint != null) return;

        transform.rotation = connectedBody.rotation * Quaternion.AngleAxis(angle, Vector3.right);
        
        transform.position = connectedBody.transform.TransformPoint(Quaternion.AngleAxis(angle + 180, Vector3.right) * holdPosition);

        joint = gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = connectedBody;

        isCarried = true;

        // Hide ghost when being carried
        ghostDisplay.gameObject.SetActive(false);
    }

    public virtual void StopCarrying()
    {
        SetColliders(true);

        Destroy(joint);
        joint = null;

        isCarried = false;

        // Show this only if we have ammo left
        ghostDisplay.gameObject.SetActive(Ammo > 0); 
    }

    public virtual void Destroy()
    {
        Destroy(gameObject);
    }

    public virtual void SetColliders(bool state) // Used to prevent oscillation when aiming
    {
        GetComponent<MeshCollider>().isTrigger = state;
        Debug.Log("collision thing HAPPENED");
    }

    protected virtual void OnDrawGizmosSelected()
    {
        // Draw a sphere at the hold position
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.TransformPoint(holdPosition), .05f);
    }
}