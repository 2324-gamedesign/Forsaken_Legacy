using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [System.Serializable]
    public class CharacterProfile
    {
        public string name;
        public Sprite profileImage;
        public Color nameColor;
    }

    public List<CharacterProfile> characterProfiles;
    private Dictionary<string, CharacterProfile> characterMap;

    public TMP_Text tutorial;

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

        // Initialize the character map
        characterMap = new Dictionary<string, CharacterProfile>();
        foreach (var profile in characterProfiles)
        {
            characterMap[profile.name] = profile;
        }
    }

    public TMP_Text nameText;
    public Text dialogueText;
    public RectTransform dialoguePanel;
    private Queue<string> sentences;
    private Queue<string> names;

    public bool isInDialogue;

    public Image profileImage;

    private void Start()
    {
        sentences = new Queue<string>();
        names = new Queue<string>();
    }

    public void StartDialogue(int nextDialogue, Dialogue[] dialogues)
    {
        names.Clear();
        sentences.Clear();

        foreach (Dialogue dialogue in dialogues)
        {
            if(dialogue.dialogueNumber == nextDialogue)
            {
                foreach (DialogueLine line in dialogue.dialogueLines)
                {
                    names.Enqueue(line.name);
                    sentences.Enqueue(line.sentence);
                }
            }
        }

        GameManager.Instance.SetDialogueState();
        isInDialogue = true;

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

        HandleSpeaker(name);
        dialogueText.text = "";
        dialogueText.DOText(sentence, 0.5f);
    }

    public void HandleSpeaker(string name)
    {
        if (characterMap.ContainsKey(name))
        {
            CharacterProfile profile = characterMap[name];
            nameText.text = profile.name;
            nameText.color = profile.nameColor;
            profileImage.sprite = profile.profileImage;
        }
    }

    public void EndDialogue()
    {
        GameManager.Instance.SetNeutralState();
        isInDialogue = false;

        CloseBarkPanel();
    }

    public void CloseBarkPanel()
    {
        // Animate the panel to move off-screen
        dialoguePanel.DOAnchorPos(new Vector2(100, -600), .2f).OnComplete(() =>
        {
            nameText.text = "";
            dialogueText.text = "";   // Clear the text
            profileImage.enabled = true;
        });
    }

    public void EditTutorialText(string text)
    {
        tutorial.text = text;
    }
}