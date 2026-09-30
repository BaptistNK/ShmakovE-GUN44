using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameplayCommand
{
    void Interact(Cell cell);
}
