using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APAGARteste : MonoBehaviour
{
    public GameObject other;


	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			other.SetActive(!other.activeInHierarchy);
		}
	}
}
