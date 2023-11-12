using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Proxy : Subject
{
    private string objectName;
	private GameObject desiredObject;

    /// <summary>
    /// Constructor with parameters
    /// </summary>
    /// <param name="objectName">Name of the proxified object</param>
	public Proxy(string objectName)
    {
        this.objectName = objectName;
        this.desiredObject = null;
    }

	/// <summary>
	/// Gets a GameObject
	/// </summary>
	/// <returns>A GameObject</returns>
	public GameObject GetObject()
    {
		try
		{
			desiredObject = GameObject.Find(objectName);

			if (desiredObject == null)
			{
				throw new Exception("Object not found with name: " + objectName);
			}

			return desiredObject;
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception while finding object: " + ex.Message);
			return null;
		}
	}
}

