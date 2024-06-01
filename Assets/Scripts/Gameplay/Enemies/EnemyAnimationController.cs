using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace HKR
{
    public class EnemyAnimationController : MonoBehaviour
    {
        enum AnimationState { Idle, Move, Hit }

        [SerializeField]
        NavMeshAgent agent;

        [SerializeField]
        Puncher puncher;

        [SerializeField]
        NetworkMecanimAnimator netAnimator;

        string motionParam = "Move";
        string hitParam = "Hit";

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!netAnimator.HasStateAuthority)
                return;


            // Check movement
            if(agent.velocity.magnitude > 0)
            {
                if (!netAnimator.Animator.GetBool(motionParam))
                    netAnimator.Animator.SetBool(motionParam, true);
            }
            else
            {
                if (netAnimator.Animator.GetBool(motionParam))
                    netAnimator.Animator.SetBool(motionParam, false);
            }

            
        }

        private void OnEnable()
        {
            puncher.OnHit += HandleOnHit;
        }

        private void OnDisable()
        {
            puncher.OnHit -= HandleOnHit;
        }

        void HandleOnHit()
        {
            if (!netAnimator.HasStateAuthority)
                return;

            netAnimator.Animator.SetTrigger(hitParam);
        }

        bool IsMoveState()
        {
            return netAnimator.Animator.GetCurrentAnimatorStateInfo(0).IsName("Move");
        }

        bool IsIdleState()
        {
            return netAnimator.Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle");
        }

        bool IsHitState()
        {
            return netAnimator.Animator.GetCurrentAnimatorStateInfo(0).IsName("Hit");
        }
    }

}
