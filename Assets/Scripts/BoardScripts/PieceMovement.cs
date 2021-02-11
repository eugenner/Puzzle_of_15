using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PieceMovement : MonoBehaviour
{
    public InitBoard board;
    // Start is called before the first frame update

    void OnCollisionEnter(Collision collision)
    {
        board.TileMove(gameObject);
    }

    public void OnSelect(XRBaseInteractor interactor)
    {
        board.TileMove(interactor.selectTarget.gameObject);
    }
}
