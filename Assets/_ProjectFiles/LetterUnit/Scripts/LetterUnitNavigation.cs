using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class LetterUnitNavigation : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Animator animator;
    [SerializeField] private Vector3 finalForward;
    private Transform _target;
    
    public void SetTarget(Transform target)
    {
        agent.updateRotation = false;
        agent.SetDestination(target.position);
    }

    void Update()
    {
        float speed = agent.velocity.magnitude;
        bool isRunning = speed > 0.1f;

        animator.SetBool("isRunning", isRunning);

        if (agent.isOnOffMeshLink)
        {
            StartCoroutine(JumpRoutine());
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

    private IEnumerator JumpRoutine()
    {
        animator.SetBool("isJumping", true);

        Vector3 startPos = transform.position;
        Vector3 endPos = agent.currentOffMeshLinkData.endPos;

        float jumpHeight = 2f;
        float duration = 0.6f;
        float t = 0f;

        agent.updatePosition = false;

        while (t < duration)
        {
            float normalizedTime = t / duration;
            Vector3 pos = Vector3.Lerp(startPos, endPos, normalizedTime);
            pos.y += Mathf.Sin(normalizedTime * Mathf.PI) * jumpHeight;
            transform.position = pos;
            t += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        agent.CompleteOffMeshLink();
        agent.updatePosition = true;

        animator.SetBool("isJumping", false);
    }
}
