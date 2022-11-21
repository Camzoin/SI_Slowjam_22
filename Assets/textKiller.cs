using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class textKiller : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameManager gm = GameObject.FindObjectOfType<GameManager>();

        gm.uiGameObject.SetActive(false);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameManager gm = GameObject.FindObjectOfType<GameManager>();

        gm.curIdeaToDestroy.dp.gameObject.SetActive(false);

        gm.curIdeaToDestroy.ShrinkMe();

        gm.curIdeaToDestroy = null;

        gm.inputManager.playerCanAct = true;

        if (gm.curLevel != null)
        {
            int randy = Random.Range(0, gm.curLevel.fonts.Count);

            gm.firstFrac.font = gm.curLevel.fonts[randy];

            gm.secondFrac.font = gm.curLevel.fonts[randy];

            gm.thirdFrac.font = gm.curLevel.fonts[randy];
        }

        if (gm.curLevel.curFoundIdeas >= gm.curLevel.spawnedIdeaCount)
        {         
            gm.GoToNextLevel();

            gm.curLevel.FadeMusicOutFunc();
        }

        gm.collectionAudioLoopVolGoingDown = true;

        gm.SetImageFills();

        gm.uiGameObject.SetActive(true);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
