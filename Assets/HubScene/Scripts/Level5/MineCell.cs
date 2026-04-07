using UnityEngine;
using TMPro;

public class MineCell2D : MonoBehaviour
{
    public int x, y;
    public bool isMine, isRevealed, isFlagged;
    public int neighboringMines;

    [Header("Visuals")]
    public SpriteRenderer sr;
    public TextMeshPro labelText;
    public GameObject flagVisual;
    public GameObject mineVisual;

    private MineGridManager2D manager;

    public void Setup(int x, int y, MineGridManager2D m)
    {
        this.x = x;
        this.y = y;
        manager = m;

        if (labelText != null) labelText.text = "";
        if (flagVisual != null) flagVisual.SetActive(false);
        if (mineVisual != null) mineVisual.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isRevealed && !isFlagged)
        {
            Reveal();
        }
    }

    public void Reveal(bool isCorrectInput = false)
    {
        if (isRevealed) return;

        if (isMine)
        {
            isRevealed = true;
            if (mineVisual != null) mineVisual.SetActive(true);
            if (sr != null) sr.color = Color.red;
            manager.TriggerGameOver();
            return;
        }

        if (neighboringMines > 0 && !isCorrectInput)
        {
            manager.OpenMathPuzzle(this);
        }
        else
        {
            isRevealed = true;
            if (sr != null) sr.color = new Color(0.7f, 0.7f, 0.7f);

            if (neighboringMines > 0)
            {
                if (labelText != null) labelText.text = neighboringMines.ToString();
            }
            else
            {
                manager.RevealEmptyNeighbors(x, y);
            }
        }
    }

    public void ToggleFlag()
    {
        if (isRevealed) return;
        isFlagged = !isFlagged;
        if (flagVisual != null) flagVisual.SetActive(isFlagged);
    }
}