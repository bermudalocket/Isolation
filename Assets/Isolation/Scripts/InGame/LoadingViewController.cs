using TMPro;
using UnityEngine;

namespace Isolation.Scripts {

    public class LoadingViewController: MonoBehaviour {
        
        [SerializeField]
        private TextMeshProUGUI FirstStageText;
        
        [SerializeField]
        private TextMeshProUGUI SecondStageText;
        
        [SerializeField]
        private TextMeshProUGUI ThirdStageText;
        
        [SerializeField]
        private TextMeshProUGUI FourthStageText;
        
        [SerializeField]
        private TextMeshProUGUI FifthStageText;

        public enum Stage {
            First, Second, Third, Fourth, Fifth
        }

        public void Show() {
            GetComponent<Canvas>().enabled = true;
        }

        public void Hide() {
            GetComponent<Canvas>().enabled = false;
        }

        private void Awake() {
            FirstStageText.text = "Locate station";
            SecondStageText.text = "Build schematic";
            ThirdStageText.text = "Train AI";
            FourthStageText.text = "Randomize randomizers";
            FifthStageText.text = "Sync life monitors";
        }

        public void SetStage(Stage stage) {
            switch (stage) {
                case Stage.First:
                    FirstStageText.fontStyle = FontStyles.Strikethrough;
                    break;
                
                case Stage.Second:
                    SecondStageText.fontStyle = FontStyles.Strikethrough;
                    break;
                
                case Stage.Third:
                    ThirdStageText.fontStyle = FontStyles.Strikethrough;
                    break;
                
                case Stage.Fourth:
                    FourthStageText.fontStyle = FontStyles.Strikethrough;
                    break;
                
                case Stage.Fifth:
                    FifthStageText.fontStyle = FontStyles.Strikethrough;
                    break;
            }
        }

    }

}