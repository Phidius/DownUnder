using UnityEngine;
using System.Collections;

public interface IInteractable {
    void Enable(bool enable);
    bool IsEnabled();
    void Interact(PlayerController player);
    void Highlight(PlayerController player, bool show);
}
