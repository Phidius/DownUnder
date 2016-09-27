using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;
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
    public Transform cameraTransform;

    public Vector3 _startingPosition;
    public float _currentHealth;
    private Image _healthImage;
    private Image _throwImage;
    private Image _staminaImage;
    private Text _message;

    private Transform _firstPerspective;
    private Transform _thirdPerspective;

    private Animator _animator;
    private AudioSource _audioSource;
    private Weapon _weapon;
    private float _throwDistance = 0f;
    private float _stamina;
    private FirstPersonController _firstPersonController;

    private float _jumpSpeed;
    private float _runSpeed;
    private GameObject _interactableGameObject;
    private VRReticle _reticle;
    private float _maxInteractableUpdate = .1f;
    private float _interactableUpdate = 0f;


    // Use this for initialization
    void Start ()
    {
        _firstPerspective = transform.FindChild("First");
        _thirdPerspective = transform.FindChild("Third");

        var avatar = transform.FindChild("avatar");
        _animator = avatar.GetComponentInChildren<Animator>();
	    _audioSource = GetComponent<AudioSource>();
        _reticle = GetComponent<VRReticle>();
        
        _firstPersonController = GetComponent<FirstPersonController>();
        if (_firstPersonController)
        {
            _jumpSpeed = _firstPersonController.GetJumpSpeed();
            _runSpeed = _firstPersonController.GetRunSpeed();
        }

        _animator.applyRootMotion = false;
        if (_animator.layerCount >= 2)
        {
            _animator.SetLayerWeight(1, 1);
        }

        _startingPosition = transform.position;
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
        _interactableUpdate += Time.deltaTime;
        if (_interactableUpdate < _maxInteractableUpdate)
        {
            return;
        }
        _interactableUpdate = 0f;

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

        if (_currentHealth <= 0)
        {
            GameManager.Instance.showHasDied = true;
            return;
        }

        GetIteractables();
        //Change back to prvious color.
        //Set / recharge the stamina
        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            _stamina -= _jumpSpeed;
        }

        var scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            var cameraPosition = cameraTransform.localPosition;// Camera.main.transform.localPosition;
            cameraPosition.z += scroll;
            cameraPosition.z = Mathf.Clamp(cameraPosition.z, -10f, 0f);

            //z = 0 is the furthest forward... z = -1 is the limit to avoid seeing the avatar mesh
            if (cameraPosition.z > -1f && cameraTransform.parent.gameObject.name == "Third") // Camera.main.transform.parent.gameObject.name == "Third")
            {
                cameraTransform.parent = _firstPerspective;// Camera.main.transform.parent = _firstPerspective;
                cameraPosition.z = 0.0f;
            }
            else
            if (cameraPosition.z < 0f && cameraTransform.parent.gameObject.name == "First") //Camera.main.transform.parent.gameObject.name == "First")
            {
                cameraTransform.parent = _thirdPerspective; //Camera.main.transform.parent = _thirdPerspective;
                cameraPosition.z = -1.1f;
            }
            cameraTransform.localPosition = cameraPosition; //Camera.main.transform.localPosition = cameraPosition;
        }
        //if (_firstPersonController)
        //{
        var forwardSpeed = 0f;
        if (_firstPersonController.m_MoveDir.y == -1f)
        {
            _animator.SetBool("OnGround", true);
            if (_firstPersonController.desiredMove.magnitude > 0.0f)
            {
                if (_firstPersonController.IsRunning() && _firstPersonController.GetRunSpeed() != _firstPersonController.GetWalkSpeed())
                {
                    _stamina -= _runSpeed * Time.deltaTime;
                    forwardSpeed = 1f;
                }
                else
                {
                    forwardSpeed = 0.5f;
                }
            }

            _animator.SetFloat("Forward", forwardSpeed, 0.1f, Time.deltaTime);
        }
        else
        {
            _animator.SetBool("OnGround", false);
            _animator.SetFloat("Jump", _firstPersonController.m_MoveDir.y);
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


        _stamina += staminaRecover * Time.deltaTime;
        _stamina = Mathf.Clamp(_stamina, 0, staminaMax);

        var staminaScale = _staminaImage.transform.localScale;
        staminaScale.x = 1f;
        staminaScale.y = _stamina / staminaMax;

        _staminaImage.transform.localScale = staminaScale;

        if (_weapon.GetState() != Weapon.WeaponState.Idle)
	    {
	        return;// The following code requires the state Rest
	    }

        if (CrossPlatformInputManager.GetButton("Fire1"))
        {
            _animator.SetBool("Windup", true);
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
            
        if (CrossPlatformInputManager.GetButtonUp("Fire1"))
        {
            _animator.SetBool("Windup", false);
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

        if (_currentHealth > 0f && damage > 0f)
        {
            _audioSource.clip = gruntSound;
            _audioSource.Play();
        }
    }
}
