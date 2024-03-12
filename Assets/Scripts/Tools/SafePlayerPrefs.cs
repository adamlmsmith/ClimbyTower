using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SafePlayerPrefs
{
    private string key;
    private List<string> properties = new List<string>();
    
    public SafePlayerPrefs (string key, params string [] properties)
    {
        this.key = key;
        foreach (string property in properties)
            this.properties.Add(property);
        //Save();
    }
    
    // Generates the checksum
    private string GenerateChecksum ()
    {
        string hash = "";
        foreach (string property in properties)
        {
            hash += property + ":";
            if (PlayerPrefs.HasKey(property))
                hash += PlayerPrefs.GetString(property);
        }

        return Md5Sum(hash + key);
    }
    
    // Saves the checksum
    public void Save()
    {
        string checksum = GenerateChecksum();
        PlayerPrefs.SetString("volume", checksum);
        PlayerPrefs.Save();
    }
    
    // Checks if there has been an edit
    public bool HasNotBeenEdited ()
    {
        if (! PlayerPrefs.HasKey("volume"))
        {
            return false;
        }

        string checksumSaved = PlayerPrefs.GetString("volume");
        string checksumReal = GenerateChecksum();

        return checksumSaved.Equals(checksumReal);
    }

    public string Md5Sum(string strToEncrypt)
    {
        System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);
        
        // encrypt bytes
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);
        
        // Convert the encrypted bytes back to a string (base 16)
        string hashString = "";
        
        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }
        
        return hashString.PadLeft(32, '0');
    }
}