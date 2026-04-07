using UnityEngine;

public class ShowVideoButtonInTrigger : MonoBehaviour
{
    [SerializeField] private GameObject playerObject;

    [SerializeField] private GameObject videoButtonObject;

    [SerializeField] private VideoLessonPlayer videoLessonPlayer;

    [SerializeField] private VideoButtonHoverPress videoButtonHoverPress;

    private void Start()
    {
        HideButton();

        if (videoLessonPlayer != null)
            videoLessonPlayer.SetPlayerInZone(false);

        if (videoButtonHoverPress != null)
            videoButtonHoverPress.ResetToNormal();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject != playerObject) return;

        if (videoLessonPlayer != null)
            videoLessonPlayer.SetPlayerInZone(true);

        if (videoLessonPlayer != null && videoLessonPlayer.IsVideoOpen())
            return;

        if (videoButtonHoverPress != null)
            videoButtonHoverPress.ResetToNormal();

        ShowButton();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject != playerObject) return;

        if (videoLessonPlayer != null)
            videoLessonPlayer.SetPlayerInZone(false);

        HideButton();

        if (videoLessonPlayer != null)
            videoLessonPlayer.CloseVideo();
    }

    private void ShowButton()
    {
        if (videoButtonObject == null) return;
        videoButtonObject.SetActive(true);
    }

    private void HideButton()
    {
        if (videoButtonObject == null) return;
        videoButtonObject.SetActive(false);
    }
}