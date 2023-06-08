using Cysharp.Threading.Tasks;
using Enums;
using Sirenix.OdinInspector;
using Slate;
using UnityEngine;
using UnityEngine.AI;

public class CharacterMovement : MonoBehaviour
{
   NavMeshAgent NavMeshAgent => _navMeshAgent != null ? _navMeshAgent : _navMeshAgent = GetComponent<NavMeshAgent>();
   [InspectorPrivate] bool AgentHasPath => NavMeshAgent.hasPath;
   [InspectorPrivate] NavMeshPathStatus PathStatus => NavMeshAgent.pathStatus;

   [SerializeField] float _speed = 1f;
   [SerializeField] float _smoothInputSpeed = 0.2f;
   [SerializeField] float _rotationSpeed = 0.1f;
   [SerializeField] float _navMeshRotationSpeed = 10f;

   MyPlayerInput _input;
   Rigidbody _rigidbody;
   Animator _animator;
   Camera _mainCamera;
   float _axisCombined;
   Vector2 _smoothMoveVector;
   Vector2 _smoothInputVelocity;
   [InspectorPrivate] bool _canMove = true;

   NavMeshAgent _navMeshAgent;

   [InspectorPrivate] bool _hasNavMeshDestination;
   [SerializeField] Transform _navMeshDestination;

   void Awake()
   {
      _rigidbody = GetComponent<Rigidbody>();
      _animator = GetComponentInChildren<Animator>();
      _input = GetComponent<MyPlayerInput>();

      _navMeshAgent = GetComponent<NavMeshAgent>();
   }

   void Start()
   {
      _mainCamera = Camera.main;
      NavMeshAgent.updateRotation = false;

      Messaging.AddListener(MessageType.CharacterPreInteracting, OnCharacterPreInteracting);
      Messaging.AddListener(MessageType.CharacterInteractionStarted, OnCharacterInteractionStarted);
      Messaging.AddListener(MessageType.CharacterInteractionFinished, OnCharacterInteractionFinished);
      Messaging.AddListener<(Floor, Floor)>(MessageType.FloorTransition, OnFloorTransition);
      Messaging.AddListener(MessageType.FloorTransitionComplete, OnFloorTransitionComplete);

      Cutscene.OnCutsceneStarted += OnCutsceneStarted;
      Cutscene.OnCutsceneStopped += OnCutsceneStopped;
   }

   void OnDestroy()
   {
      Messaging.RemoveListener(MessageType.CharacterPreInteracting, OnCharacterPreInteracting);
      Messaging.RemoveListener(MessageType.CharacterInteractionStarted, OnCharacterInteractionStarted);
      Messaging.RemoveListener(MessageType.CharacterInteractionFinished, OnCharacterInteractionFinished);
   }

   void Update()
   {
      if (_hasNavMeshDestination == false)
      {
         ControllerUpdate();
      }
      else
      {
         NavMeshUpdate();
      }
   }

   public async UniTask WalkToPosition(Vector3 destination)
   {
      NavMeshAgent.enabled = true;
      NavMeshAgent.ResetPath();
      NavMeshAgent.SetDestination(destination);
      _hasNavMeshDestination = true;

      while (NavMeshAgent.ReachedDestinationOrGaveUp() == false)
      {
         await UniTask.Yield();
      }

      NavMeshAgent.ResetPath();
      // At this point, the agent has reached the destination
      _hasNavMeshDestination = false;
      NavMeshAgent.enabled = false;
      Debug.Log("#CharacterMovement# Agent has reached the destination.");
   }

   [Button]
   public void GoToDestination()
   {
      NavMeshAgent.destination = _navMeshDestination.position;
      _hasNavMeshDestination = true;
      NavMeshAgent.enabled = true;
      NavMeshAgent.isStopped = false;
   }

   [Button]
   public void ResetPath()
   {
      NavMeshAgent.ResetPath();
   }

   void ControllerUpdate()
   {
      if (_canMove == false) return;

      var inputVectorRaw = _input.MoveVectorRaw;

      _smoothMoveVector = Vector2.SmoothDamp(_smoothMoveVector, inputVectorRaw, ref _smoothInputVelocity, _smoothInputSpeed);

      var finalMoveVector = new Vector3(_smoothMoveVector.x, 0, _smoothMoveVector.y);
      var moveDirection = GetMoveDirection(finalMoveVector);
      MoveTowardTarget(finalMoveVector);

      RotateTowardMovementVector(moveDirection);
      _animator.SetFloat("Speed", moveDirection.magnitude);
   }

   void NavMeshUpdate()
   {
      if (NavMeshAgent.ReachedDestinationOrGaveUp())
      {
         _animator.SetFloat("Speed", 0f);
         _hasNavMeshDestination = false;
         NavMeshAgent.enabled = false;
      }
      else
      {
         var targetDirection = (NavMeshAgent.steeringTarget - transform.position).normalized;
         var fixedYTargetDirection = new Vector3(targetDirection.x, transform.position.y, targetDirection.z);
         var lookRotation = Quaternion.LookRotation(fixedYTargetDirection);

         DebugExtension.DebugArrow(transform.position, targetDirection, Color.white);
         DebugExtension.DebugArrow(transform.position, fixedYTargetDirection, Color.yellow);

         transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * _navMeshRotationSpeed);
         _animator.SetFloat("Speed", NavMeshAgent.velocity.magnitude);

      }
   }

   Vector3 GetMoveDirection(Vector3 moveVector)
   {
      return Quaternion.Euler(0, _mainCamera.transform.rotation.eulerAngles.y, 0) * moveVector;
   }

   void MoveTowardTarget(Vector3 targetVector)
   {
      _rigidbody.velocity = targetVector * _speed;
   }

   void RotateTowardMovementVector(Vector3 movementDirection)
   {
      if (movementDirection.magnitude < 0.01f)
      {
         return;
      }

      var rotation = Quaternion.LookRotation(movementDirection);
      transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, _rotationSpeed * Time.deltaTime);
   }

   void OnCharacterPreInteracting()
   {
      Debug.Log($"#CharacterMovement# OnCharacterPreInteracting");
      _canMove = false;
   }

   void OnCharacterInteractionStarted()
   {
      Debug.Log($"#CharacterMovement# OnCharacterInteractionStarted");
      _rigidbody.velocity = Vector3.zero;
      _smoothMoveVector = Vector3.zero;
      _smoothInputVelocity = Vector3.zero;
      _animator.SetFloat("Speed", 0f);
      NavMeshAgent.enabled = false;
   }

   void OnCharacterInteractionFinished()
   {
      Debug.Log($"#CharacterMovement# OnCharacterInteractionFinished");
      _canMove = true;
   }

   void OnCutsceneStopped(Cutscene cutscene)
   {
      if (cutscene is HouseCutscene houseCutscene)
      {
         if (houseCutscene.DisableCharacterMovement)
         {
            _canMove = true;
         }
      }
   }

   void OnCutsceneStarted(Cutscene cutscene)
   {
      if (cutscene is HouseCutscene houseCutscene)
      {
         if (houseCutscene.DisableCharacterMovement)
         {
            _canMove = false;
            _rigidbody.velocity = Vector3.zero;
            _animator.SetFloat("Speed", 0f);
         }
      }
   }

   void OnFloorTransitionComplete()
   {
      _canMove = true;
      _rigidbody.velocity = Vector3.zero;
   }

   async void OnFloorTransition((Floor, Floor) fromToFloor)
   {
      _canMove = false;
      _rigidbody.velocity = Vector3.zero;
      await FadeToBlackController.Instance.HasFadedToBlack();


      var entryPoint = FloorManager.Instance.GetEntryPoint();

      transform.position = entryPoint.position;
      transform.rotation = entryPoint.rotation;
   }

}