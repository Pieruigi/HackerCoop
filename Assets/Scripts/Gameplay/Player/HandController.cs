using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR
{
    public class HandController : MonoBehaviour
    {
        Animator animator;

        string paramName = "Up";
               

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void MoveUp()
        {
            if (!animator.GetBool(paramName))
                animator.SetBool(paramName, true);
        }

        public void MoveDown()
        {
            if(animator.GetBool(paramName))
                animator.SetBool(paramName, false);
        }
    }

}
