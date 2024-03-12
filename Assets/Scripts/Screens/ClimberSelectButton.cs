using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class ClimberSelectButton : MonoBehaviour
{
    public Color m_SelectedColor;
    public Color m_UnselectedColor;
    public Color m_LockedColor;

    public Material m_LockedMaterial;
    public Material m_UnlockedMaterial;

    private Climber Climber { get; set; }
    private bool Highlighted { get; set; }

    void Start()
    {
        transform.localScale = Vector3.one;
    }

    public void AddClimber(int i)
    {
        // Instantiate a climber and child it to the button.
        Climber = Instantiate(GameVariables.instance.ClimberManager.GetClimber(i)) as Climber;

        Climber.transform.SetParent(transform.Find("ClimberHolder").transform);
        Climber.transform.localPosition = new Vector3(0.0f, 0.0f, -0.1f);

        if(Climber.transform.Find("ClimberTorso") != null)
            Climber.transform.Find("ClimberTorso").transform.localPosition = Vector3.zero;

        Climber.transform.localScale = new Vector3(77.0f, 77.0f, 77.0f);

        Climber.GetComponent<Animator>().applyRootMotion = false;
        Climber.GetComponent<Animator>().Play("DemoClimbLeft");


        foreach (Transform child in Climber.GetComponentsInChildren<Transform>(true))
        {
            child.gameObject.layer = LayerMask.NameToLayer("UI");
        }

        Climber.name = GameVariables.instance.ClimberManager.GetClimber(i).name;
        Climber.GetComponent<Climber>().enabled = false;

        transform.Find("ClimberName").GetComponent<Text>().text = Climber.ClimberDisplayName;
    }

    public void Initialize(bool animatorEnabled)
    {
        Climber.GetComponent<Animator>().Play("DemoClimbLeft");
        Climber.GetComponent<Animator>().enabled = animatorEnabled;
    }

    public void SetHighlighted(bool highlighted)
    {
        Highlighted = highlighted;

        if (highlighted == false)
        {
            Climber.transform.parent.localScale = new Vector3(1.0f, 1.0f, 1.0f);

            if (GameVariables.instance.ClimberManager.IsClimberUnlocked(Climber.ClimberName))
                SetClimberColor(m_UnlockedMaterial, m_UnselectedColor);
            else
                SetClimberColor(m_LockedMaterial, m_LockedColor);

            Climber.GetComponent<Animator>().speed = 0.0f;
        }
        else
        {
            // Highlight and Scale up the selected climber
            Climber.transform.parent.localScale *= 2.0f;

            if (GameVariables.instance.ClimberManager.IsClimberUnlocked(Climber.ClimberName))
            {
                // only change the color and animate if the climber is unlocked
                SetClimberColor(m_UnlockedMaterial, m_SelectedColor);
                Climber.GetComponent<Animator>().speed = 1.0f;
            }
        }
    }

    void SetClimberColor(Material material, Color color)
    {
        foreach (SpriteRenderer renderer in Climber.GetComponentsInChildren<SpriteRenderer>())
        {
            renderer.color = color;
            renderer.material = material;
        }
    }

    public bool IsClimberUnlocked()
    {
        return(GameVariables.instance.ClimberManager.IsClimberUnlocked(Climber.ClimberName));
    }

    public void OnButtonClick()
    {
        print(Climber.ClimberName + " Unlocked = " + Climber.Unlocked + " Highlighted = " + Highlighted);

        if (IsClimberUnlocked() && Highlighted)
        {
            NotificationCenter.DefaultCenter().PostNotification(gameObject, k.MessageNames.CLIMBER_COSTUME_CLICKED);
        }
    }
}
