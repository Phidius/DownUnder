using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(AudioSource))]
//[RequireComponent(typeof(FirstPersonController))]
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
    //public GameObject dot;

    private Vector3 _startingPosition;
    private float _currentHealth;
    private Image _healthImage;
    private Image _throwImage;
    private Image _staminaImage;
    private Text _message;

    private AudioSource _audioSource;
    public Weapon _weapon;
    private float _throwDistance = 0f;
    private float _stamina;
    //private FirstPersonController _firstPersonController;
    //private float _jumpSpeed;
    //private float _runSpeed;
    private GameObject _interactableGameObject;
    //private Vector3 _dotScale;
    //private float interactableUpdate = 0f;
    //private float maxInteractableUpdate = 0.5f;
    //private Vector3 _aimPosition = new Vector3(0.5f, 0.4f, 0f);
    private VRReticle _reticle;

    private OptionsController options;

    // Use this for initialization
    void Start ()
    {
        options = GameObject.FindObjectOfType<OptionsController>();
	    _audioSource = GetComponent<AudioSource>();
        _reticle = GetComponent<VRReticle>();
	    //_firstPersonController = GetComponent<FirstPersonController>();
	    //_jumpSpeed = _firstPersonController.GetJumpSpeed();
	    //_runSpeed = _firstPersonController.GetRunSpeed();

        _startingPosition = transform.position;
	    _startingPosition.y += 500f;
	    _stamina = staminaMax;
        
        _weapon = GetComponentInChildren<Weapon>();
        if (_weapon != null)
        {
            _weapon.Equipped(true);
            if (_reticle)
            {
                 _reticle.SetDistance(maxThrowDistance);
            }
           
        }
        
        //_dotScale = dot.transform.localScale;
        foreach (var component in Camera.main.GetComponentsInChildren<Text>())
        {
            if (component.name == "Message")
            {
                _message = component;
            }
        }
        foreach (var component in Camera.main.GetComponentsInChildren<Image>())
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
        //interactableUpdate += Time.deltaTime;
        //if (interactableUpdate < maxInteractableUpdate)
        //{
        //    return;
        //}

        //var gaze = Camera.main.ViewportPointToRay(_aimPosition);
        //RaycastHit hit;
        //if (Physics.Raycast(gaze, out hit))
        var obstacle = _reticle.GetObstacle();
        if (obstacle)
        {
            if ((Interactable)obstacle.GetComponent(typeof(Interactable)) != null)
            {
                // This is interactable
                if (_interactableGameObject && obstacle.GetInstanceID() != _interactableGameObject.GetInstanceID())
                {
                    // This is a different object - turn off the last one used
                    ((Interactable)_interactableGameObject.GetComponent(typeof(Interactable))).Highlight(this, false);
                }
                // Turn on this object
                _interactableGameObject = obstacle.gameObject;
                ((Interactable)_interactableGameObject.GetComponent(typeof(Interactable))).Highlight(this, true);

            }
            else if (_interactableGameObject)
            {
                // obstacle is not interactable is not - turn off the last one used
                ((Interactable)_interactableGameObject.GetComponent(typeof(Interactable))).Highlight(this, false);
                _interactableGameObject = null;
            }
        }
        else
        {
            //dot.transform.position = Vector3.zero;
            if (_interactableGameObject)
            {
                ((Interactable)_interactableGameObject.GetComponent(typeof(Interactable))).Highlight(this, false);
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
        //   if (CrossPlatformInputManager.GetButtonDown("Jump"))
        //{
        //    _stamina -= _jumpSpeed;
        //}

        //if (_firstPersonController.IsRunning())
        //{
        //    _stamina -= _runSpeed * Time.deltaTime;
        //}

        //if (_stamina <= _jumpSpeed && _stamina <= _runSpeed)
        //{
        //    _firstPersonController.SetJumpSpeed(0);
        //    _firstPersonController.SetRunSpeed(_firstPersonController.GetWalkSpeed());
        //}
        //else
        //{
        //       _firstPersonController.SetJumpSpeed(_jumpSpeed);
        //       _firstPersonController.SetRunSpeed(_runSpeed);
        //   }

        _stamina += staminaRecover * Time.deltaTime;
        _stamina = Mathf.Clamp(_stamina, 0, staminaMax);

        var staminaScale = _staminaImage.transform.localScale;
        staminaScale.x = 1f;
        staminaScale.y = _stamina / staminaMax;

        _staminaImage.transform.localScale = staminaScale;

        if (_weapon.GetState() != BoomerangController.WeaponState.Idle)
	    {
	        return;// The following code requires the state Rest
	    }

        if (!options.showOptions)
        {
            if (CrossPlatformInputManager.GetButton("Fire1"))
            {
                _throwDistance += (throwWindupSpeed * Time.deltaTime);
                _throwDistance = Mathf.Clamp(_throwDistance, 0f, 50f);
                
            }
            //if (CrossPlatformInputManager.GetButton("Fire2"))
            //{
            //    _message.text = "Fire2 pressed";
            //}
            //if (CrossPlatformInputManager.GetButton("Fire3"))
            //{
            //    _message.text = "Fire3 pressed";
            //}
            //if (CrossPlatformInputManager.GetButton("Fire4"))
            //{
            //    _message.text = "Fire4 pressed";
            //}
            //if (CrossPlatformInputManager.GetButton("Fire5"))
            //{
            //    _message.text = "Fire5 pressed";
            //}
            //if (CrossPlatformInputManager.GetButton("Fire6"))
            //{
            //    _message.text = "Fire6 pressed";
            //}
            //if (CrossPlatformInputManager.GetButton("Fire7"))
            //{
            //    _message.text = "Fire7 pressed";
            //}
            //if (CrossPlatformInputManager.GetButton("Fire8"))
            //{
            //    _message.text = "Fire8 pressed";
            //}
            //if (CrossPlatformInputManager.GetButton("Fire9"))
            //{
            //    _message.text = "Fire9 pressed";
            //}
            //if (CrossPlatformInputManager.GetButton("Fire10"))
            //{
            //    _message.text = "Fire10 pressed";
            //}
            //if (CrossPlatformInputManager.GetButton("Fire11"))
            //{
            //    _message.text = "Fire11 pressed";
            //}

            //dot.transform.position = point;
            if (CrossPlatformInputManager.GetButtonUp("Fire1"))
            {
                if (_throwDistance > maxThrowDistance*.1f) // TODO: base this on the player's collider, perhaps?
                {
                    var targetDistance = Vector3.Distance(_weapon.transform.position, _reticle.GetAimPoint());
                    var fractionThrow = (_throwDistance < targetDistance) ? _throwDistance/targetDistance : 1f;
                    _weapon.Throw(Vector3.Lerp(_weapon.transform.position, _reticle.GetAimPoint(), fractionThrow));
                }
                else
                {
                    _weapon.Swing();
                }
                _throwDistance = 0f;
            }

            if (CrossPlatformInputManager.GetButtonDown("Interact"))
            {
                if (_interactableGameObject)
                {
                    foreach (Interactable interactable in _interactableGameObject.GetComponents(typeof (Interactable)))
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
