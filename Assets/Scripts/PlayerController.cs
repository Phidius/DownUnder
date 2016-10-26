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
    public float healthRecover = .5f;
    public float staminaMax = 20;
    public float staminaRecover = 1;
    public AudioClip gruntSound;
    public Transform cameraTransform;

    public Vector3 _startingPosition;
    public float _currentHealth;
    private Image _healthImage;
    
    private Image _staminaImage;
    private Text _message;
    
    public Transform _itemSlot;
    public Transform _weaponSlot;

    private Animator _animator;
    private AudioSource _audioSource;
    public Weapon _weapon;
    public Usable _item;
    
    private float _stamina;
    public FirstPersonController _firstPersonController;

    private float _jumpSpeed;
    private float _runSpeed;
    private float _walkSpeed;
    public float crouchSpeed = .3f;
    private GameObject _interactableGameObject;
    public VRReticle _reticle;
    private float _maxInteractableUpdate = .1f;
    private float _interactableUpdate = 0f;

    private bool _isCatching = false;
    private bool _isCrouching = false;
    
    // Use this for initialization
    void Start ()
    {
        _itemSlot = GameObject.Find("ItemSlot").transform;
        _weaponSlot = GameObject.Find("WeaponSlot").transform;

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

        var HUDisplay = GameObject.Find("HUDisplay");

        foreach (var component in HUDisplay.GetComponentsInChildren<Text>())
        {
            if (component.name == "Message")
            {
                _message = component;
            }
        }

        foreach (var component in HUDisplay.GetComponentsInChildren<Image>())
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

        if (_weapon == null || _weapon.gameObject.activeInHierarchy == false)
        {
            _weapon = _weaponSlot.gameObject.GetComponentInChildren<Weapon>();
            if (_weapon)
            {
                _weapon.Equipped(true);
            }
            
        }

        if (GameManager.Instance.gameState != GameManager.GameState.Play)
        {
            return;
        }

        GetIteractables();

        if (CrossPlatformInputManager.GetButtonDown("Use") && _item != null)
        {
            _item.Use();
        }

        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
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
        _currentHealth += healthRecover*Time.deltaTime;

        UpdateHUDisplay();

        MovePlayer();

        Interact(CrossPlatformInputManager.GetButtonDown("Interact"));

        
        if (_weapon != null && _weapon.isActiveAndEnabled)
        {
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
        }
    }

    private void UpdateHUDisplay()
    {
        _stamina = Mathf.Clamp(_stamina, 0, staminaMax);
        _currentHealth = Mathf.Clamp(_currentHealth, 0, startingHealth);

        var staminaScale = _staminaImage.transform.localScale;
        staminaScale.x = 1f;
        staminaScale.y = _stamina/staminaMax;

        _staminaImage.transform.localScale = staminaScale;

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

    }

    private void MovePlayer()
    {
        var forwardSpeed = 0f;
        if (_firstPersonController.m_MoveDir.y > 0f)
        {
            _animator.SetBool("OnGround", false);
        }
        if (_firstPersonController.m_MoveDir.y == 0f)
        {
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

        if (_currentHealth > 0f && damage > 0f)
        {
            _audioSource.clip = gruntSound;
            _audioSource.Play();
        }
    }
}
