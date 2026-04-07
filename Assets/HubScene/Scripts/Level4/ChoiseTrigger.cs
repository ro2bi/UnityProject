using UnityEngine;

namespace EquationSystem
{
    public class ChoiceTrigger : MonoBehaviour
    {
        [SerializeField] private int stageIndex;
        [SerializeField] private bool isCorrect;
        [SerializeField] private LevelStagesManager manager;
        [SerializeField] private Transform player;

        public void SetCorrect(bool value) => isCorrect = value;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.transform != player) return;

            if (isCorrect)
                manager.OnCorrectChoice(stageIndex);
            else
                manager.OnWrongChoice();
        }
    }
}