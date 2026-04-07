using UnityEngine;

namespace EquationSystem
{
    public class LevelStartTrigger1 : MonoBehaviour
    {
        [SerializeField] private LevelStagesManager manager;
        [SerializeField] private Transform player;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.transform == player)
            {
                manager.StartLevelTimer();
                gameObject.SetActive(false);
            }
        }
    }
}