using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace QuantumTek.QuantumDialogue.Demo
{
    public class QD_ChoiceButton : MonoBehaviour
    {
        public int number;
        public QD_DialogueDemo demo;
        [SerializeField] TextMeshProUGUI text;

        private void OnEnable()
        {
            TextMeshProUGUI buttonText = GetComponent<TextMeshProUGUI>();
            text.text = buttonText.text;
        }

        public void Select()
        {
            //this.GetComponent<Button>().interactable = false;
            demo.EnterSelectedChoice(this.GetComponent<TextMeshProUGUI>().text);
            //Invoke("ChooseNext", 1f);
            ChooseNext();
        }

        private void ChooseNext()
        {
            demo.Choose(number);
        }
    }
}