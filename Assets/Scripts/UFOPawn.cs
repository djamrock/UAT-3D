using UnityEngine;



public class UFOPawn : Pawn
{
    public float moveSpeed = 5f;



    public override void Start()
    {

    }

    public override void Update()
    {

    }



    public override void MoveForward()
    {
        if (GameManager.instance == null)
        {
            return;
        }

        PlayerController playerController = GameManager.instance.playerController;

        if (playerController == null || playerController.pawn == null)
        {
            return;
        }

        Transform player = playerController.pawn.transform;

        Vector3 direction = (player.position - transform.position).normalized;      // points UFO towards player

        transform.position += direction * moveSpeed * Time.deltaTime;

        transform.rotation = Quaternion.LookRotation(direction);        
    }



    public override void MoveBackward()
    {
    }

    public override void YawLeft()
    {
    }

    public override void YawRight()
    {
    }

    public override void RollLeft()
    {
    }

    public override void RollRight()
    {
    }

    public override void PitchUp()
    {
    }

    public override void PitchDown()
    {
    }

    public override void Shoot()
    {
    }
}
