using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// When looking at an inventory, an item is selected or deselected. Updates the display.
// Index, display string
public class Inventory_SelectEvent : UnityEvent<int?, string>
{
}
