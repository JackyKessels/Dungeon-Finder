using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InventoryAddResult
{
    public int requested;
    public int added;

    public int remaining => requested - added;

    public bool Complete => remaining == 0;
}
