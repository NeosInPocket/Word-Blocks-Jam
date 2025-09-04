using System.Collections;
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

    private bool _isJumping;
    private bool _shouldJump;

    public void SetTarget(Transform target, bool shouldJump = false)
    {
        agent.updateRotation = false;
        agent.isStopped = false;

        _isJumping = false;
        _shouldJump = shouldJump;

        agent.SetDestination(target.position);
    }

    void Update()
    {
        if (agent.velocity.magnitude == 0)
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
        bool isRunning = speed > 0.1f;
        animator.SetBool("isRunning", isRunning);

        if (_shouldJump)
        {
            float distanceToDestination = Vector3.Distance(transform.position, agent.destination);
            if (distanceToDestination <= jumpTriggerDistance)
            {
                animator.SetBool("isRunning", false);
                StartCoroutine(JumpToTargetRoutine(agent.destination));
                return;
            }
        }

        if (agent.velocity.magnitude == 0)
        {
            Vector3 direction = finalForward.normalized;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            Vector3 velocity = agent.velocity;
            velocity.y = 0; 
            if (velocity.sqrMagnitude > 0.01f) 
            {
                Quaternion targetRotation = Quaternion.LookRotation(-velocity);
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
    }
}
