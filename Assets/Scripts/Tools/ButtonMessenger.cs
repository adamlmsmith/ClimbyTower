using UnityEngine;
using System.Collections;

public class ButtonMessenger : MonoBehaviour
{
	public void PostNotification(string message)
	{
		NotificationCenter.DefaultCenter().PostNotification(gameObject, message);
	}
}
