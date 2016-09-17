using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(FirstPersonController))]
public class PlayerController : MonoBehaviour, IHitable
{
    public enum PlayerState
    {
        Active,
        Drowned,
        Died,
        Won
    };
    public float startingHealth = 20;
    public float staminaMax = 20;
    public float staminaRecover = 1;
    public float maxThrowDistance = 50f;
    public float throwWindupSpeed = 10f;
    public AudioClip gruntSound;
    public GameObject dot;

    private Vector3 _startingPosition;
    private float _currentHealth;
    private Image _healthImage;
    private Image _throwImage;
    private Image _staminaImage;
    private Text _message;

    private AudioSource _audioSource;
    private BoomerangController _boomerang;
    private float _throwDistance = 0f;
    private float _stamina;
    private FirstPersonController _firstPersonController;
    private float _jumpSpeed;
    private float _runSpeed;
    private GameObject _interactableGameObject;
    private Vector3 _dotScale;
    private float interactableUpdate = 0f;
    private float maxInteractableUpdate = 0.5f;

    private OptionsController options;
    // Use this for initialization
    void Start ()
    {
        options = GameObject.FindObjectOfType<OptionsController>();
	    _audioSource = GetComponent<AudioSource>();
	    _firstPersonController = GetComponent<FirstPersonController>();
	    _jumpSpeed = _firstPersonController.GetJumpSpeed();
	    _runSpeed = _firstPersonController.GetRunSpeed();

        _startingPosition = transform.position;
	    _startingPosition.y += 500f;
	    _stamina = staminaMax;

        _boomerang = GameObject.FindObjectOfType<BoomerangController>();

        _dotScale = dot.transform.localScale;
        foreach (var component in GetComponentsInChildren<Text>())
        {
            if (component.name == "Message")
            {
                _message = component;
            }
        }
        foreach (var component in GetComponentsInChildren<Image>())
	    {
            if (component.name == "Health")
            {
                _healthImage = component;
            }
            if (component.name == "Throw")
            {
                _throwImage = component;
            }
            if (component.name == "Stamina")
            {
                _staminaImage = component;
            }
        }

        _currentHealth = startingHealth;

        var throwScale = _throwImage.transform.localScale;
        throwScale.x = _throwDistance / maxThrowDistance;
        _throwImage.transform.localScale = throwScale;

    }

    void GetIteractables()
    {
        interactableUpdate += Time.deltaTime;
        var gaze = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.4f, 0f));
        RaycastHit hit;
        if (Physics.Raycast(gaze, out hit))
        {
            dot.transform.position = hit.point;
            var cameraPosition = Camera.main.transform.position;
            var distance = Vector3.Distance(hit.point, cameraPosition);
            var distanceMultiplier = _dotScale * distance;
            dot.transform.localScale = distanceMultiplier;
            if (interactableUpdate < maxInteractableUpdate)
            {
                return;
            }
            var gazeObject = (IInteractable)hit.transform.gameObject.GetComponent(typeof(IInteractable));
            if (gazeObject != null)
            {
                // This is interactable
                if (_interactableGameObject && hit.transform.gameObject.GetInstanceID() != _interactableGameObject.GetInstanceID())
                {
                    // This is a different object - turn off the last one used
                    ((IInteractable)_interactableGameObject.GetComponent(typeof(IInteractable))).Highlight(this, false);
                }
                // Turn on this object
                _interactableGameObject = hit.transform.gameObject;
                ((IInteractable)_interactableGameObject.GetComponent(typeof(IInteractable))).Highlight(this, true);

            }
            else if (_interactableGameObject)
            {
                // This is not - turn off the last one used
                ((IInteractable)_interactableGameObject.GetComponent(typeof(IInteractable))).Highlight(this, false);
                _interactableGameObject = null;
            }
        }
        else
        {
            dot.transform.position = Vector3.zero;
            if (interactableUpdate < maxInteractableUpdate)
            {
                return;
            }
            if (_interactableGameObject)
            {
                ((IInteractable)_interactableGameObject.GetComponent(typeof(IInteractable))).Highlight(this, false);
                _interactableGameObject = null;
            }
        }
    }
    // Update is called once per frame
    void Update () {
	    //if (GvrViewer.Instance.VRModeEnabled && GvrViewer.Instance.Triggered)
	    //{
     //       GvrViewer.Instance.Recenter();
	    //}

        if (_currentHealth <= 0)
        {
            Reset(PlayerState.Died);
            return;
        }

        GetIteractables();
        // Change back to prvious color.
        // Set/recharge the stamina
        if (CrossPlatformInputManager.GetButtonDown("Jump"))
	    {
	        _stamina -= _jumpSpeed;
	    }

	    if (_firstPersonController.IsRunning())
	    {
	        _stamina -= _runSpeed * Time.deltaTime;
	    }

	    if (_stamina <= _jumpSpeed && _stamina <= _runSpeed)
	    {
	        _firstPersonController.SetJumpSpeed(0);
	        _firstPersonController.SetRunSpeed(_firstPersonController.GetWalkSpeed());
	    }
	    else
	    {
            _firstPersonController.SetJumpSpeed(_jumpSpeed);
            _firstPersonController.SetRunSpeed(_runSpeed);
        }

	    _stamina += staminaRecover*Time.deltaTime;
	    _stamina = Mathf.Clamp(_stamina, 0, staminaMax);

        var staminaScale = _staminaImage.transform.localScale;
        staminaScale.x = 1f;
        staminaScale.y = _stamina / staminaMax;

        _staminaImage.transform.localScale = staminaScale;

        if (_boomerang.GetState() != BoomerangController.BoomerangState.Rest)
	    {
	        return;// The following code requires the state Rest
	    }

        if (!options.showOptions)
        {
            if (CrossPlatformInputManager.GetButton("Fire1"))
            {
                _throwDistance += (throwWindupSpeed*Time.deltaTime);
                _throwDistance = Mathf.Clamp(_throwDistance, 0f, 50f);

            }

            if (CrossPlatformInputManager.GetButtonUp("Fire1"))
            {
                if (_throwDistance > maxThrowDistance*.1f)
                {
                    var point = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.40f, _throwDistance));
                    _boomerang.Throw(point);
                }
                else
                {
                    _boomerang.Swing();
                }
                _throwDistance = 0f;
            }

            if (CrossPlatformInputManager.GetButtonDown("Fire2"))
            {
                if (_interactableGameObject)
                {
                    foreach (IInteractable interactable in _interactableGameObject.GetComponents(typeof (IInteractable)))
                    {
                        interactable.Interact(this);
                    }
                    //_interactableObject.GetComponent<InteractionSpider>().Interact(this);

                    //var healthLoot = _interactableObject.GetComponent<BottleHealthController>();
                    //if (healthLoot)
                    //{
                    //    healthLoot.Interact(this);
                    //}
                    //var staminaLoot = _interactableObject.GetComponent<BottleStaminaController>();
                    //if (staminaLoot)
                    //{
                    //    staminaLoot.Interact(this);
                    //}
                }
            }
        }

        var throwScale = _throwImage.transform.localScale;
	    throwScale.x = 1f;
        throwScale.y = _throwDistance / maxThrowDistance;
	    
        _throwImage.transform.localScale = throwScale;

	    if (_throwDistance > maxThrowDistance*.1f)
	    {
	        _throwImage.color = Color.green;
	    }
	    else
	    {
            _throwImage.color = Color.grey;
        }
    }

    public void Reset(PlayerState state)
    {
        SceneManager.LoadScene("GameOver");
        //_message.text = "You have " + state;
        //Invoke("ClearMessage", 2f);
        //_currentHealth = startingHealth;
        //Hit(0);
        //_currentHealth = startingHealth;
        //transform.position = _startingPosition;
        //transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }

    public void FinishLevel()
    {
        _message.text = "You have won the game!";
    }

    private void ClearMessage()
    {
        _message.text = string.Empty;
    }

    public void ApplyStamina(float stamina)
    {
        _stamina += stamina;
    }

    public void Hit(float damage)
    {
        _currentHealth -= damage;
        if (_currentHealth > startingHealth * .75)
        {
            _healthImage.color = Color.green;
        }
        else if (_currentHealth > startingHealth * .5)
        {
            _healthImage.color = Color.yellow;
        }
        else if (_currentHealth > startingHealth * .25)
        {
            _healthImage.color = Color.red;
        }

        var healthScale = _healthImage.transform.localScale;
        healthScale.x = _currentHealth / startingHealth;
        _healthImage.transform.localScale = healthScale;

        if (_currentHealth > 0)
        {
            _audioSource.clip = gruntSound;
            _audioSource.Play();
        }
    }
}
