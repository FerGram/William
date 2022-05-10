using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace QuantumTek.QuantumDialogue.Demo
{
    public class QD_DialogueDemo : MonoBehaviour
    {
        public QD_DialogueHandler handler;
        public Transform choices;
        public TextMeshProUGUI choiceTemplate;
        public GameObject messagePrefabRight;
        public GameObject messagePrefabLeft;
        public GameObject messagePrefabLeftWriting;
        public GameObject myMessages;
        public GameObject otherMessages;
        public AnswerPopUp answerWindow;
        public TextMeshProUGUI fadeIntText;
        public AudioClip myMessageSE;
        public AudioClip otherMessageSE;
        
        private List<TextMeshProUGUI> activeChoices = new List<TextMeshProUGUI>();
        private List<TextMeshProUGUI> inactiveChoices = new List<TextMeshProUGUI>();

        private AudioSource audioSource;
        private string messageText;
        private string speakerName;
        private bool ended;
        private bool lastMessageSent = false;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            handler.SetConversation("Main Conv");
            SetText();
        }

        private void Update()
        {
            // Don't do anything if the conversation is over
            if (!ended)
            {
                // Check if the space key is pressed and the current message is not a choice
                if (handler.currentMessageInfo.Type == QD_NodeType.Message && Input.GetKeyUp(KeyCode.Space) && lastMessageSent)
                {
                    lastMessageSent = false;
                    Next();
                }
            }
        }

        private void ClearChoices()
        {
            for (int i = activeChoices.Count - 1; i >= 0; --i)
            {
                // Use object pooling with the choices to prevent unecessary garbage collection
                activeChoices[i].gameObject.SetActive(false);
                activeChoices[i].text = "";
                inactiveChoices.Add(activeChoices[i]);
                activeChoices.RemoveAt(i);
            }
        }

        private void GenerateChoices()
        {
            // Exit if not a choice
            if (handler.currentMessageInfo.Type != QD_NodeType.Choice)
                return;
            // Clear the old choices
            ClearChoices();
            // Generate new choices
            QD_Choice choice = handler.GetChoice();
            int added = 0;
            // Use inactive choices instead of making new ones, if possible
            while (inactiveChoices.Count > 0 && added < choice.Choices.Count)
            {
                int i = inactiveChoices.Count - 1;
                TextMeshProUGUI cText = inactiveChoices[i];
                cText.text = choice.Choices[added];
                QD_ChoiceButton button = cText.GetComponent<QD_ChoiceButton>();
                button.number = added;
                cText.gameObject.SetActive(true);
                activeChoices.Add(cText);
                inactiveChoices.RemoveAt(i);
                added++;
            }
            // Make new choices if any left to make
            while (added < choice.Choices.Count)
            {
                TextMeshProUGUI newChoice = Instantiate(choiceTemplate, choices);
                newChoice.text = choice.Choices[added];
                QD_ChoiceButton button = newChoice.GetComponent<QD_ChoiceButton>();
                button.number = added;
                newChoice.gameObject.SetActive(true);
                activeChoices.Add(newChoice);
                added++;
            }
        }

        private void SetText()
        {
            // Clear everything
            speakerName = "";
            messageText = "";
            ClearChoices();

            // If at the end, don't do anything
            if (ended)
                return;

            StartCoroutine(SetTextCoroutine());
        }

        IEnumerator SetTextCoroutine()
        {
            // Generate choices if a choice, otherwise display the message
            if (handler.currentMessageInfo.Type == QD_NodeType.Message)
            {
                QD_Message message = handler.GetMessage();
                speakerName = message.SpeakerName;
                messageText = message.MessageText;

                //Wait till timing
                float timing = handler.dialogue.GetMessage(handler.currentMessageInfo.ID).Timing;
                if (timing == 0f) { timing = Random.Range(1f, 2.5f); }

                if (speakerName != "Player")
                {
                    GameObject writingMessage = Instantiate(messagePrefabLeftWriting, otherMessages.transform);
                    GameObject temp = Instantiate(messagePrefabLeftWriting, myMessages.transform);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(otherMessages.GetComponent<RectTransform>());
                    yield return null;
                    temp.GetComponentInChildren<Image>().color = new Color(0, 0, 0, 0);
                    Image[] tempGO = temp.GetComponentsInChildren<Image>();
                    foreach (Image item in tempGO)
                    {
                        Destroy(item.gameObject);
                    }
                    Destroy(writingMessage, timing);
                    Destroy(temp, timing);
                }
                yield return new WaitForSeconds(timing);

                Debug.Log(speakerName + " is speaking");
                if (speakerName == "TimeStamp")
                {
                    FadeInScreen(handler.dialogue.GetMessage(handler.currentMessageInfo.ID).MessageText);
                    Invoke("FadeOutScreen", 2f);
                }
                else if (speakerName == "Ending 1" || speakerName == "Ending 2" || 
                         speakerName == "Ending 3" || speakerName == "Ending 4" || 
                         speakerName == "Ending 5" || speakerName == "Ending 6") 
                        { FadeInScreen(handler.dialogue.GetMessage(handler.currentMessageInfo.ID).MessageText); }
                else
                {
                    GameObject newMessage;
                    GameObject hidden;
                    if (speakerName == "Player")
                    {
                        newMessage = Instantiate(messagePrefabRight, myMessages.transform);
                        hidden = Instantiate(newMessage, otherMessages.transform);
                        audioSource.PlayOneShot(myMessageSE);
                    }
                    else
                    {
                        newMessage = Instantiate(messagePrefabLeft, otherMessages.transform);
                        hidden = Instantiate(newMessage, myMessages.transform);
                        audioSource.PlayOneShot(otherMessageSE);
                    }
                    newMessage.GetComponentInChildren<TextMeshProUGUI>().text = messageText;
                    //Debug.Log(newMessage.GetComponentInChildren<TextMeshProUGUI>().text);
                    hidden.GetComponentInChildren<TextMeshProUGUI>().text = messageText;
                    LayoutRebuilder.ForceRebuildLayoutImmediate(myMessages.GetComponent<RectTransform>());
                    LayoutRebuilder.ForceRebuildLayoutImmediate(otherMessages.GetComponent<RectTransform>());
                    //yield return null;
                    hidden.GetComponentInChildren<TextMeshProUGUI>().color = new Color(0, 0, 0, 0);
                    hidden.GetComponentInChildren<Image>().color = new Color(0, 0, 0, 0);
                }
            }
            else if (handler.currentMessageInfo.Type == QD_NodeType.Choice)
            {
                answerWindow.UpdateAnswersDisplay();
                speakerName = "Player";
                GenerateChoices();
            }

            lastMessageSent = true;
            yield return null;
        }

        public void Next(int choice = -1)
        {
            if (ended)
                return;
            
            // Go to the next message
            handler.NextMessage(choice);
            // Set the new text
            SetText();
            // End if there is no next message
            if (handler.currentMessageInfo.ID < 0)
                ended = true;
        }

        public void Choose(int choice)
        {
            if (ended)
                return;

            Next(choice);
        }

        public void EnterSelectedChoice(string text)
        {
            StartCoroutine(EnterSelectedChoiceRoutine(text));
        }

        IEnumerator EnterSelectedChoiceRoutine(string text)
        {
            GameObject newMessage = Instantiate(messagePrefabRight, myMessages.transform);
            GameObject hidden = Instantiate(newMessage, otherMessages.transform);
            newMessage.GetComponentInChildren<TextMeshProUGUI>().text = text;
            hidden.GetComponentInChildren<TextMeshProUGUI>().text = text;
            LayoutRebuilder.ForceRebuildLayoutImmediate(myMessages.GetComponent<RectTransform>());
            //yield return null;
            hidden.GetComponentInChildren<Image>().color = new Color(0, 0, 0, 0);
            hidden.GetComponentInChildren<TextMeshProUGUI>().color = new Color(0, 0, 0, 0);
            audioSource.Play();
            lastMessageSent = false;
            yield return null;
        }

        public void FadeInScreen(string text)
        {
            fadeIntText.GetComponentInParent<Animator>().SetTrigger("FadeOUT");
            fadeIntText.text = text;
            Debug.Log("Fading in...");
        }

        public void FadeOutScreen()
        {
            fadeIntText.GetComponentInParent<Animator>().SetTrigger("FadeOUT");
            Debug.Log("Fading out...");
        }
    }
}