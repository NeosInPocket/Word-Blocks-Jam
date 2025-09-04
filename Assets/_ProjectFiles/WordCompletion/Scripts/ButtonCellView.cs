using System;
using UnityEngine;

public class ButtonCellView : MonoBehaviour
{
   private void OnTriggerEnter(Collider other)
   {
      Debug.Log("button");
   }
}