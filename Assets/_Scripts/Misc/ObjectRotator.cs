using UnityEngine;
using System.Collections;

public class ObjectRotator : MonoBehaviour {


void Update (){
	transform.Rotate (0, 45 * Time.deltaTime, 0);
}

void OnBecameVisible (){
	enabled = true;	
}

void OnBecameInvisible (){
	enabled = false;	
}
}