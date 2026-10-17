
using UnityEngine;



public class UFOAIController : Controller
{
    public Transform target;
    public float detectionDistance = 100f;





    public override void Start()
    {

    }


    public override void Update()
    {
        MakeDecisions();
    }


    public override void MakeDecisions()
    {
        if (pawn == null) return;

        // Find the player automatically if no target is assigned.
        if (target == null && GameManager.instance != null)
        {
            if (GameManager.instance.playerController != null && GameManager.instance.playerController.pawn != null)
            {
                target = GameManager.instance.playerController.pawn.transform;
            }
        }

        if (target == null) return;

        float distance = Vector3.Distance(pawn.transform.position, target.position);

        if (distance <= detectionDistance)
        {
            pawn.MoveForward();
        }
    }
}
