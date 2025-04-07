using UnityEngine;

/// <summary>
/// This scriptable object contains the color settings for the table.
/// </summary>
[CreateAssetMenu(fileName = "TableColorSettings", menuName = "Table/Color Settings")]
public class TableColorSettings : ScriptableObject
{
    public Color normalColor = new Color(0.8f, 0.8f, 0.8f, 0f);
    public Color hoverColor = new Color(0.8f, 0.8f, 0.2f, 0.5f);
    public Color winningColor = new Color(0f, 0.8f, 0f, 0.7f);

    public float colorTransitionSpeed = 5f;

} 