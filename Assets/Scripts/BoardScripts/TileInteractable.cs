using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TileInteractable : XRBaseInteractable
{
    public InitBoard board;
    public void OnMyActivate(XRBaseInteractor interactor)
    {
        Debug.Log("Activated");
        board.TileMove(interactor.gameObject);
    }
}
