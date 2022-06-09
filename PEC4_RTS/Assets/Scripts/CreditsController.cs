using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsController : MonoBehaviour
{
    public TextAsset credits;
    public RectTransform startPosition;
    public RectTransform endPosition;
    public float velocity;
    public Animator animator;

    private TextMeshProUGUI creditsText;
    private string creditsMapped;
    private bool pause = false;
    private int waitTimeTransition = 1;

    // Start is called before the first frame update
    void Start()
    {
        Collaborators collaborators = FileManager.LoadCollaborators(credits);
        creditsMapped = FillCredits(collaborators);
        creditsText = GetComponent<TextMeshProUGUI>();
        creditsText.text = creditsMapped;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !pause)
        {
            pause = !pause;
        }
        transform.position = new Vector3(transform.position.x, transform.position.y + (velocity * Time.deltaTime), transform.position.z);
        if(transform.position.y > endPosition.position.y) {
            EndCredits();
        }
    }

    private string FillCredits(Collaborators collaborators)
    {
        string credits = "Credits\n\n\n";

        foreach(Collaborator collaborator in collaborators.collaborators)
        {
            credits += collaborator.aportation + " : " + collaborator.name + "\n\n";
        }
        return credits;
    }

    public void EndCredits()
    {
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        animator.SetTrigger("StartTransition");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("MainMenu");
    }

}
