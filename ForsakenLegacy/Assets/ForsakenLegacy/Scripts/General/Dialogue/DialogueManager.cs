using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Text nameText;
    public Text dialogueText;
    public RectTransform dialoguePanel;
    private Queue<string> sentences;
    private Queue<string> names;
    private bool inDialogue;

    private void Start() {
        sentences = new Queue<string>();
        names = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        names.Clear();
        sentences.Clear();

        foreach (string name in dialogue.names)
        {
            names.Enqueue(name);
        }
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        GameManager.Instance.SetDialogueState();
        inDialogue = true;

        dialoguePanel.DOAnchorPos(Vector2.zero, 0.5f);
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string name = names.Dequeue();
        string sentence = sentences.Dequeue();

        nameText.text = name;
        dialogueText.text = "";
        dialogueText.DOText(sentence, 0.5f);
    }

    public void EndDialogue()
    {
        GameManager.Instance.SetNeutralState();
        inDialogue = false;

        CloseBarkPanel();
    }

    public void CloseBarkPanel()
    {
        // Animate the panel to move off-screen
        dialoguePanel.DOAnchorPos(new Vector2(0, -220), .2f).OnComplete(() =>
        {
            nameText.text = "";
            dialogueText.text = "";   // Clear the text
        });
    }
}
