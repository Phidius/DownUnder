using UnityEngine;
using UnityEngine.Rendering;

public class VRReticle : MonoBehaviour {
    
    private float _lookDistance = 50f;
    private LayerMask _layerMask;
    private GameObject _reticle;
    private GameObject _obstacle;
    private Renderer _reticleRenderer;
    private readonly Vector3 _reticleScale = new Vector3(0.02f, 0.02f, 0.02f);
    private readonly Vector3 _aimPosition = new Vector3(0.5f, 0.4f, 0f);

    // Use this for initialization
    void Start () {
        _layerMask = LayerMask.GetMask(new string[] { "Default", "Ground", "Enemey" });
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
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        var gaze = Camera.main.ViewportPointToRay(_aimPosition);
        RaycastHit hit;
        Vector3 point;
        if (Physics.Raycast(gaze, out hit, _lookDistance, _layerMask))
        {
            _obstacle = hit.collider.gameObject;
            point = hit.point;
        }
        else
        {
            _obstacle = null;
            var throwTarget = _aimPosition;
            throwTarget.z = _lookDistance;
            point = Camera.main.ViewportToWorldPoint(throwTarget);
        }
        _reticle.transform.position = point;
        var cameraPosition = Camera.main.transform.position;
        var distance = Vector3.Distance(point, cameraPosition);
        var distanceMultiplier = _reticleScale * distance;
        _reticle.transform.localScale = distanceMultiplier;
    }

    public GameObject GetObstacle()
    {
        return _obstacle;
    }

    public Vector3 GetAimPoint()
    {
        return _reticle.transform.position;
    }

    public void SetDistance(float distance)
    {
        _lookDistance = distance;
    }

    public float GetDistance()
    {
        return _lookDistance;
    }
}
