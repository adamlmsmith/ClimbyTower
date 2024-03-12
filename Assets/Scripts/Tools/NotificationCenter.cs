using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//    NotificationCenter is used for handling messages between GameObjects.   
//    GameObjects can register to receive specific notifications.  When another objects sends a notification of that type, all GameObjects that registered for it and implement the appropriate message will receive that notification.
//    Observing GameObjetcs must register to receive notifications with the AddObserver function, and pass their selves, and the name of the notification.  Observing GameObjects can also unregister themselves with the RemoveObserver function.  GameObjects must request to receive and remove notification types on a type by type basis.   
//    Posting notifications is done by creating a Notification object and passing it to PostNotification.  All receiving GameObjects will accept that Notification object.  The Notification object contains the sender, the notification type name, and an option hashtable containing data.
//    To use NotificationCenter, either create and manage a unique instance of it somewhere, or use the static NotificationCenter.
public class NotificationCenter
{
    static NotificationCenter defaultCenter;

    // We need a static method for objects to be able to obtain the default notification center.
    // This default center is what all objects will use for most notifications.  We can of course create our own separate instances (i.e. NotificationCenter_SuperBoxxi) but this is the static one used by all.
    public static NotificationCenter DefaultCenter()
    {
        // If the defaultCenter doesn't already exist, we need to create it
        if (defaultCenter == null)
        {
            defaultCenter = new NotificationCenter();
        }

        return defaultCenter;
    }

    /// <summary>
    /// clears default center out
    /// </summary>
    public static void Shutdown()
    {
        defaultCenter = null;
    }

    // Our hashtable containing all the notifications.  Each notification in the hash table is an ArrayList that contains all the observers for that notification.
    Dictionary<string, List<GameObject>> notifications = new Dictionary<string, List<GameObject>>();

    // AddObserver includes a version where the observer can request to only receive notifications from a specific object.  We haven't implemented that yet, so the sender value is ignored for now.
    public void AddObserver(GameObject observer, string name)
    {
        AddObserver(observer, name, null);
    }

    public void AddObserver(GameObject observer, string name, GameObject sender)
    {
        // If the name isn't good, then throw an error and return.
        if (name == null || name == "")
        {
            Debug.LogError("Null name specified for notification in AddObserver."); 
            return;
        }

        if (observer == null)
        {
            Debug.LogError("Game object is gone");
            return;
        }

        // If this specific notification doens't exist yet, then create it.
        if (!notifications.ContainsKey(name) || notifications[name] == null)
        {
            notifications[name] = new List<GameObject>();
        }

        var notifyList = notifications[name];

        // If the list of observers doesn't already contains the one that's registering, then add it.
        if (!notifyList.Contains(observer))
        {
            notifyList.Add(observer);
        }
        for (int i = notifyList.Count - 1; i >= 0; --i)
        {
            if (notifyList[i] == null)
                notifyList.RemoveAt(i);
        }
        notifyList.Sort((a, b) => a.name.CompareTo(b.name));
    }

    // RemoveObserver removes the observer from the notification list for the specified notification type
    public void RemoveObserver(GameObject observer, string name)
    {
        if (!notifications.ContainsKey(name))
        {
            Debug.Log("Can not find [" + name + "] in notification");
            return;
        }

        var notifyList = notifications[name];

        // Assuming that this is a valid notification type, remove the observer from the list.
        // If the list of observers is now empty, then remove that notification type from the notifications hash.  This is for housekeeping purposes.
        if (notifyList != null)
        {
            if (notifyList.Contains(observer))
            {
                notifyList.Remove(observer);
            }
            if (notifyList.Count == 0)
            {
                notifications.Remove(name);
            }
        }
    }

    // PostNotification sends a notification object to all objects that have requested to receive this type of notification.
    // A notification can either be posted with a notification object or by just sending the individual components.
    public void PostNotification(GameObject sender, string name)
    {
        PostNotification(sender, name, null);
    }

    public void PostNotification(GameObject sender, string name, object data)
    {
        PostNotification(new Notification(sender, name, data));
    }

    public void PostNotification(Notification notification)
    {
        // First make sure that the name of the notification is valid.
        if (notification.Name == null || notification.Name == "")
        {
            Debug.Log("Null name sent to PostNotification.");
            return;
        }

        // Obtain the notification list, and make sure that it is valid as well
        if (!notifications.ContainsKey(notification.Name))
        {
            Debug.Log("Can not find [" + notification.Name + "] in notification");
            return;
        }
        var notifyList = notifications[notification.Name];
        if (notifyList == null)
        {
            Debug.Log("Notify list not found in PostNotification. Name: " + notification.Name);
            return;
        }

        // Create an array to keep track of invalid observers that we need to remove
        var observersToRemove = new List<GameObject>();

        // Itterate through all the objects that have signed up to be notified by this type of notification.
        for (int i = 0; i < notifyList.Count; ++i)//GameObject observer in notifyList) 
        {
            var observer = notifyList[i];

            // If the observer isn't valid, then keep track of it so we can remove it later.
            // We can't remove it right now, or it will mess the for loop up.
            if (ReferenceEquals(observer, null))
            {
                observersToRemove.Add(observer);
            }
            else
            {
                // If the observer is valid, then send it the notification.  The message that's sent is the name of the notification.
                var gameObject = (GameObject)observer;
                if (gameObject != null)
                    gameObject.SendMessage(notification.Name, notification, SendMessageOptions.DontRequireReceiver);
            }
        }

        // Remove all the invalid observers
        for (int i = 0; i < observersToRemove.Count; ++i)//observer in observersToRemove) 
        {
            notifyList.Remove(observersToRemove[i]);
        }
    }
}

// The Notification class is the object that is send to receiving objects of a notification type.
// This class contains the sending GameObject, the name of the notification, and optionally a hashtable containing data.
public class Notification
{
    public GameObject Sender { get; set; }
    public string Name { get; set; }
    public object Data { get; set; }

    public Notification(GameObject sender, string name)
    {
        Sender = sender;
        Name = name;
        Data = null;
    }

    public Notification(GameObject sender, string name, object data)
    {
        Sender = sender;
        Name = name;
        Data = data;
    }
}
