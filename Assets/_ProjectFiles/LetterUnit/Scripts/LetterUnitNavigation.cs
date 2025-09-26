using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class LetterUnitNavigation : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Animator animator;
    [SerializeField] private Vector3 finalForward;
    [SerializeField] private float jumpTriggerDistance = 2f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float jumpDuration = 0.6f;
    [SerializeField] private bool debug;
    [SerializeField] private float destinationThreshold = 0.1f;

    private ButtonCellView _button;
    private bool _isJumping;
    private bool _shouldJump;
    private Vector3 _finalDestination;
    
    public bool Enabled
    {
        get => agent.enabled;
        set => agent.enabled = value;
    }

    public bool InFinalPlace
    {
        get
        {
            if (_finalDestination == Vector3.zero) return false;
            
            Vector2 flatAgentPosition = new Vector2(agent.transform.position.x, agent.transform.position.z);
            Vector2 flatDestination = new Vector2(_finalDestination.x, _finalDestination.z);
            var distance = Vector2.Distance(flatAgentPosition, flatDestination);

            var result = distance < 0.01f;
            return result;
        }
    }

    private void Start()
    {
        agent.enabled = false;
    }

    public async UniTask SetTarget(Vector3 target, bool shouldJump = false, ButtonCellView cellView = null, List<BoardCellView> keyPositions = null)
    {
        agent.enabled = true;
        agent.updateRotation = false;
        agent.isStopped = false;

        _isJumping = false;
        _shouldJump = shouldJump;
        _button = cellView;
        _finalDestination = target;

        if (keyPositions is { Count: > 0 })
        {
            foreach (var cell in keyPositions)
            {
                if (cell.IsOccupiedByMovingUnit)
                {
                    await WaitForCellFree(cell);
                }

                cell.IsOccupiedByMovingUnit = true;
                if (agent == null) return;
                agent.SetDestination(cell.transform.position);
                await WaitUntilReached(cell.transform.position);
                cell.IsOccupiedByMovingUnit = false;
            }
        }

        agent.SetDestination(target);
        await WaitForDestinationReach();
    }

    private async UniTask WaitForCellFree(BoardCellView cellView)
    {
        while (cellView.IsOccupiedByMovingUnit)
        {
            await Task.Yield();
        }
    }
    
    private async UniTask WaitForDestinationReach()
    {
        while (!InFinalPlace)
        {
            await Task.Yield();
        }
    }
    
    private async UniTask WaitUntilReached(Vector3 destination)
    {
        bool inPlace = false;
        
        while (!inPlace)
        {
            Vector2 flatUnitPosition = new Vector2(transform.position.x, transform.position.z);
            Vector2 flatDestination = new Vector2(destination.x, destination.z);
            float distance = Vector2.Distance(flatUnitPosition, flatDestination);
            if (distance < destinationThreshold && !_isJumping)
            {
                inPlace = true;
            }
            
            await Task.Yield();
        }
    }

    void Update()
    {
        if (!agent.enabled) return;
        
        if (agent.isStopped)
        {
            Vector3 direction = finalForward.normalized;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
        
        if (_isJumping || agent.isStopped) return;
        
        float speed = agent.velocity.magnitude;
        animator.SetBool("isRunning", speed > 0.1f);
        
        if (_shouldJump)
        {
            float distanceToDestination = Vector3.Distance(transform.position, _finalDestination);
            if (distanceToDestination <= jumpTriggerDistance)
            {
                animator.SetBool("isRunning", false);
                StartCoroutine(JumpToTargetRoutine(_finalDestination));
                return;
            }
        }
        
        if (agent.velocity.magnitude > 0.01f)
        {
            Vector3 velocity = agent.velocity;
            velocity.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(-velocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            Vector3 direction = finalForward.normalized;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    private IEnumerator JumpToTargetRoutine(Vector3 targetPos)
    {
        _isJumping = true;
        agent.isStopped = true;
        agent.updatePosition = false;

        animator.SetTrigger("JumpStart");
        yield return new WaitForSeconds(0.15f);

        animator.SetBool("JumpLoop", true);

        Vector3 startPos = transform.position;
        float t = 0f;

        while (t < jumpDuration)
        {
            float normalizedTime = t / jumpDuration;
            Vector3 pos = Vector3.Lerp(startPos, targetPos, normalizedTime);
            pos.y += Mathf.Sin(normalizedTime * Mathf.PI) * jumpHeight;
            transform.position = pos;
            t += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        agent.nextPosition = transform.position;

        animator.SetBool("JumpLoop", false);

        agent.isStopped = true;
        agent.updatePosition = true;
        _isJumping = false;
        
        _button?.OnButtonPressed();
    }
}
