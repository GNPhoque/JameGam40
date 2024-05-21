using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMonoBehaviour : MonoBehaviour
{
	public static int integer;
	public static MyClass myClass;

	// Start is called before the first frame update
	void Start()
	{
		print($"{integer}");
		print($"{myClass == null}");
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Return))
		{
			integer++;
			myClass = new MyClass();
			print($"{integer}");
			print($"{myClass == null}");
		}
	}
}
public class MyClass { }