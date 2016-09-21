using UnityEngine;
using UnityEngine.Rendering;

public class VRCursor : MonoBehaviour
{
    [SerializeField] public Transform target;
    [SerializeField] private bool autoTargetPlayer = true;
    [SerializeField] private float moveSpeed = 5f;
	[SerializeField] private float turnSpeed = 1.5f;
	[SerializeField] private float turnsmoothing = .1f;
	[SerializeField] private float tiltMax = 75f;
	[SerializeField] private float tiltMin = 45f;
    [SerializeField]  private bool lockCursor = false;
    [SerializeField] private bool showReticle = false;
    public LayerMask layerMask;

    private float lookAngle;
	private float tiltAngle;
	private const float _lookDistance = 100f;
	private float smoothX = 0;
	private float smoothY = 0;
	private float smoothXvelocity = 0;
	private float smoothYvelocity = 0;
    private Transform cam;
    private Transform pivot;
    //private Vector3 _lastTargetPosition;
    private float offsetX;
    private float offsetY;

    // Reticle
    private GameObject _reticle;
    private Renderer _reticleRenderer;
    private Vector3 _reticleScale = new Vector3(0.02f, 0.02f, 0.02f);
    private Vector3 _aimPosition = new Vector3(0.5f, 0.4f, 0f);

    //add the singleton
    //private static VRCursor instance;
    //public static VRCursor GetInstance()
    //{
    //    return _instance;
    //}

    // Getters
    public Transform Target { get { return this.target; } }

    // Setters
    public void SetTarget(Transform newTransform)
    {
        target = newTransform;
    }

    void Awake()
	{
        //_instance = this;
		cam = GetComponentInChildren<Camera>().transform;
        pivot = cam.parent;
	}

    void Start()
    {
        // Finding shader in Awake()
        var transparent = Shader.Find("Transparent/Diffuse");

        var alphaColor = Color.red;
        alphaColor.a = 0.5f;

        _reticle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _reticleRenderer = _reticle.GetComponent<Renderer>();
        _reticleRenderer.shadowCastingMode = ShadowCastingMode.Off;
        var collider = _reticle.GetComponent<Collider>();
        if (collider)
        {
            Destroy(collider);
        }

        // Changing  shader
        _reticleRenderer.material.shader = transparent;

        _reticleRenderer.material.color = alphaColor;
        if (autoTargetPlayer)
        {
            FindTargetPlayer();
        }

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        
    }
    void FixedUpdate()
    {
        //if (autoTargetPlayer && (target == null || !target.gameObject.activeSelf))
        //{
        //    // If the target is missing or has been inactivated, and the property "autoTargetPlayer" is true, then find the active game object with the tag "Player"
        //    FindTargetPlayer();
        //}
        if (target != null && (target.GetComponent<Rigidbody>() != null && !target.GetComponent<Rigidbody>().isKinematic))
        {
            Follow(Time.deltaTime);
        }

        var gaze = Camera.main.ViewportPointToRay(_aimPosition);
        RaycastHit hit;
        Vector3 point;
        if (Physics.Raycast(gaze, out hit, _lookDistance, layerMask))
        {
            point = hit.point;
        }
        else
        {
            var throwTarget = _aimPosition;
            throwTarget.z = _lookDistance;
            point = Camera.main.ViewportToWorldPoint(throwTarget);
        }

        _reticle.transform.position = point;
        var cameraPosition = Camera.main.transform.position;
        var distance = Vector3.Distance(hit.point, cameraPosition);
        var distanceMultiplier = _reticleScale * distance;
        _reticle.transform.localScale = distanceMultiplier;
    }
    
    // Update is called once per frame
    void Update ()
	{
        HandleRotationMovement();

	}
    public void FindTargetPlayer()
    {
        if (target == null)
        {
            GameObject targetObj = GameObject.FindGameObjectWithTag("Player");
            if (targetObj)
            {
                SetTarget(targetObj.transform);
            }
        }
    }

    
    // Update is called once per frame

    void Follow (float deltaTime)
	{
		transform.position = Vector3.Lerp(transform.position, target.position, deltaTime * moveSpeed);

	}

	void HandleRotationMovement()
	{
        if (offsetX != 0)
        {
            offsetX = Mathf.MoveTowards(offsetX, 0, Time.deltaTime);
        }

        if (offsetY != 0)
        {
            offsetY = Mathf.MoveTowards(offsetY, 0, Time.deltaTime);
        }

        var x = Input.GetAxis("Mouse X") + offsetX;
		var y = Input.GetAxis("Mouse Y") + offsetY;

        if (turnsmoothing > 0)
        {
            smoothX = Mathf.SmoothDamp(smoothX, x, ref smoothXvelocity, turnsmoothing);
            smoothY = Mathf.SmoothDamp(smoothY, y, ref smoothYvelocity, turnsmoothing);
        }
        else
        {
            smoothX = x;
            smoothY = y;
        }

		lookAngle += smoothX * turnSpeed;

		transform.rotation = Quaternion.Euler(0f, lookAngle, 0);

		tiltAngle -= smoothY * turnSpeed;
		tiltAngle = Mathf.Clamp (tiltAngle, -tiltMin, tiltMax);

		pivot.localRotation = Quaternion.Euler(tiltAngle,0,0);

	}

}
