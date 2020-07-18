using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Shop: The user has pressed "Buy". Add the items from the cart to the party inventory and deduct gold
public class Shop_MakePurchaseEvent : UnityEvent<Dictionary<InvNames,int>>
{
}
