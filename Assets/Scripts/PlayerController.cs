using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;
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
    public AudioClip gruntSound;
    public Transform cameraTransform;

    public Vector3 _startingPosition;
    public float _currentHealth;
    private Image _healthImage;
    
    private Image _staminaImage;
    private Text _message;

    private Transform _firstPerspective;
    private Transform _thirdPerspective;

    private Animator _animator;
    private AudioSource _audioSource;
    private Weapon _weapon;
    
    private float _stamina;
    private FirstPersonController _firstPersonController;

    private float _jumpSpeed;
    private float _runSpeed;
    private float _walkSpeed;
    public float crouchSpeed = .3f;
    private GameObject _interactableGameObject;
    private VRReticle _reticle;
    private float _maxInteractableUpdate = .1f;
    private float _interactableUpdate = 0f;

    private bool _isCatching = false;
    private bool _isCrouching = false;
    private bool _isJumping = false;

    private GameObject boomerang;
    private GameObject knife;

    public GameObject[] _inventory;
    
    // Use this for initialization
    void Start ()
    {
        _firstPerspective = transform.FindChild("First");
        _thirdPerspective = transform.FindChild("Third");
        _startingPosition = transform.position;

        _animator = GetComponent<Animator>();
	    _audioSource = GetComponent<AudioSource>();
        _reticle = GetComponent<VRReticle>();
        
        _firstPersonController = GetComponent<FirstPersonController>();

        _jumpSpeed = _firstPersonController.GetJumpSpeed();
        _runSpeed = _firstPersonController.GetRunSpeed();
        _walkSpeed = _firstPersonController.GetWalkSpeed();

        _animator.applyRootMotion = false;
        if (_animator.layerCount >= 2)
        {
            _animator.SetLayerWeight(1, 1);
        }

        _currentHealth = startingHealth;
        _stamina = staminaMax;
        
        if (_reticle)
        {
            _reticle.SetDistance(50f);
        }

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
            if (component.name == "Stamina")
            {
                _staminaImage = component;
            }
        }
        
        boomerang = GameObject.Find("boomerang");
        knife = GameObject.Find("Knife");

        knife.SetActive(false);
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
                
                var interactable = ((Interactable)_interactableGameObject.GetComponent(typeof(Interactable)));
                if (interactable.IsHighlighted() == false)
                {
                    interactable.Highlight(this, true);
                }

            }
            else if (_interactableGameObject)
            {
                // obstacle is not interactable - turn off the last one used
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
            GameManager.Instance.gameState = GameManager.GameState.Dead;
            return;
        }

        if (CrossPlatformInputManager.GetButtonDown("SwitchWeapon"))
        {
            if (boomerang.activeInHierarchy)
            {
                boomerang.SetActive(false);
                knife.SetActive(true);
            }
            else
            {
                boomerang.SetActive(true);
                knife.SetActive(false);
            }
        }
        if (_weapon == null || _weapon.gameObject.activeInHierarchy == false)
        {
            _weapon = GetComponentInChildren<Weapon>();
            _weapon.Equipped(true);
        }
        GetIteractables();
        
        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            _isJumping = true;
            _stamina -= _jumpSpeed;
        }
        
        if (CrossPlatformInputManager.GetButtonDown("Crouch"))
        {
            _isCrouching = !_isCrouching; // Toggle the state
            _animator.SetBool("Crouch", _isCrouching);
        }

        if (_isCrouching)
        {
            _firstPersonController.SetJumpSpeed(0);
            _firstPersonController.SetRunSpeed(crouchSpeed);
            _firstPersonController.SetWalkSpeed(crouchSpeed);
        }
        else if(_stamina <= _jumpSpeed && _stamina <= _runSpeed)
        {
            _firstPersonController.SetJumpSpeed(0);
            _firstPersonController.SetRunSpeed(_walkSpeed);
        }
        else
        {
            _firstPersonController.SetJumpSpeed(_jumpSpeed);
            _firstPersonController.SetRunSpeed(_runSpeed);
            _firstPersonController.SetWalkSpeed(_walkSpeed);
        }

        if (!_isCrouching)
        {
            _stamina += staminaRecover * Time.deltaTime;
        }
        
        MovePerspective();

        MovePlayer();

        Interact(CrossPlatformInputManager.GetButtonDown("Interact"));
        
        var weaponDistance = Vector3.Distance(transform.position, _weapon.transform.position);

        if (weaponDistance < 10f && _weapon.GetState() == Weapon.WeaponState.ThrowReturn && _isCatching == false)
        {
            _animator.SetBool("Catch", true);
            _isCatching = true;
        }

        if (_weapon.GetState() == Weapon.WeaponState.Idle)
        {
            _animator.SetBool("Windup", false);
            _animator.SetBool("Swing", false);
        }

        var fireButtonPressed = CrossPlatformInputManager.GetButton("Fire1");

        if (fireButtonPressed && _weapon.GetState() == Weapon.WeaponState.Idle)
        {
            _weapon.Charge();
            _animator.SetBool("Windup", true);
        }

        if (!fireButtonPressed && _weapon.GetState() == Weapon.WeaponState.Charging)
        {
            // Button released
            _animator.SetBool("Swing", true);
            _weapon.Swing();
        }

        UpdateHUDisplay();
    }

    private void UpdateHUDisplay()
    {
        _stamina = Mathf.Clamp(_stamina, 0, staminaMax);
        var staminaScale = _staminaImage.transform.localScale;
        staminaScale.x = 1f;
        staminaScale.y = _stamina/staminaMax;

        _staminaImage.transform.localScale = staminaScale;

    }

    private void MovePlayer()
    {
        var forwardSpeed = 0f;
        if (_firstPersonController.m_MoveDir.y > 0f)
        {
            _isJumping = true;
            _animator.SetBool("OnGround", false);
        }
        if (_firstPersonController.m_MoveDir.y == 0f)
        {
            _isJumping = false;
            _animator.SetBool("OnGround", true);
        }

        if (_firstPersonController.m_MoveDir.y == -1f)
        {
            if (_firstPersonController.desiredMove.magnitude > 0.0f)
            {
                if (_firstPersonController.IsRunning() &&
                    _firstPersonController.GetRunSpeed() != _firstPersonController.GetWalkSpeed())
                {
                    _stamina -= _runSpeed*Time.deltaTime;
                    forwardSpeed = 1f;
                }
                else
                {
                    forwardSpeed = 0.5f;
                }
            }

            _animator.SetFloat("Forward", forwardSpeed, 0.1f, Time.deltaTime);
        }
        _animator.SetFloat("Jump", _firstPersonController.m_MoveDir.y);
    }

    private void Interact(bool clicked)
    {
        if (clicked)
        {
            if (_interactableGameObject)
            {
                foreach (Interactable interactable in _interactableGameObject.GetComponents(typeof (Interactable)))
                {
                    interactable.Interact(this);
                }
            }
        }
    }

    private void MovePerspective()
    {
        var zoomCamera = Input.GetAxis("ZoomCamera");
        if (zoomCamera != 0)
        {
            var cameraPosition = cameraTransform.localPosition; // Camera.main.transform.localPosition;
            cameraPosition.z += zoomCamera;
            cameraPosition.z = Mathf.Clamp(cameraPosition.z, -10f, 0f);

            //z = 0 is the furthest forward... z = -1 is the limit to avoid seeing the avatar mesh
            if (cameraPosition.z > -1f && cameraTransform.parent.gameObject.name == "Third")
            {
                cameraTransform.parent = _firstPerspective; // Camera.main.transform.parent = _firstPerspective;
                cameraPosition.z = 0.0f;
            }
            else if (cameraPosition.z < 0f && cameraTransform.parent.gameObject.name == "First")
            {
                cameraTransform.parent = _thirdPerspective; //Camera.main.transform.parent = _thirdPerspective;
                cameraPosition.z = -1.1f;
            }
            cameraTransform.localPosition = cameraPosition; //Camera.main.transform.localPosition = cameraPosition;
        }

        //if (cameraTransform.parent == _thirdPerspective)
        //{
        //    var rotateCamera = Input.GetAxis("RotateController");
        //    if (rotateCamera != 0f)
        //    {
        //        _firstPersonController.rotationOffset += (rotateCamera*3);
        //    }
        //}
        var rotateCamera = Input.GetAxis("RotateController");
        if (rotateCamera != 0f)
        {
            _firstPersonController.rotationOffset += (rotateCamera * 3);
        }

    }

    public void ReleaseBoomerang()
    {
        _weapon.Throw(_reticle.GetAimPoint());

        _animator.SetBool("Windup", false);
        _animator.SetBool("Swing", false);
    }

    public void HasCaught()
    {
        _isCatching = false;
        _animator.SetBool("Catch", false);
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
