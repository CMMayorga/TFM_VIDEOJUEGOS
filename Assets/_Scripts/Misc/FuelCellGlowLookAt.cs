using UnityEngine;
using System.Collections;

public class FuelCellGlowLookAt : MonoBehaviour {


void Update (){
	transform.LookAt(Camera.main.transform);
}

void OnBecameVisible (){
	enabled = true;	
}

void OnBecameInvisible (){
	enabled = false;	
}
}