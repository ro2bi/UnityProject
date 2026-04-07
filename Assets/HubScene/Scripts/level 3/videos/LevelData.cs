using UnityEngine;
using UnityEngine.Video;

public enum EquationType
{
    A_plus_B_minus_C,
    A_minusminus_B_plus_C,
    A_mul_B_plus_C,
    A_del_scob1_B_min_C_scob2,
    A_mul_scob1_B_minus_C_scob2,
}

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    public int targetResult;
    public EquationType formulaType;

    [Header("Videos")]
    public VideoClip introVideo;
    public VideoClip winVideo;
    public VideoClip tooLowVideo;
    public VideoClip tooHighVideo;
}