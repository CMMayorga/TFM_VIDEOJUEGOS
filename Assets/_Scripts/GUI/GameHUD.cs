using UnityEngine;
using System.Collections;

public class GameHUD : MonoBehaviour
{

    [SerializeField]
    GUISkin guiSkin;
    [SerializeField]
    float nativeVerticalResolution = 1200.0f;
    [SerializeField]
    Texture2D healthImage;
    [SerializeField]
    Vector2 healthImageOffset = new Vector2(0, 0);

    [SerializeField]
    Texture2D[] healthPieImages;
    [SerializeField]
    Vector2 healthPieImageOffset = new Vector2(10, 147);

    [SerializeField]
    Vector2 livesCountOffset = new Vector2(425, 160);

    [SerializeField]
    Texture2D fuelCellsImage;
    [SerializeField]
    Vector2 fuelCellOffset = new Vector2(0, 0);

    [SerializeField]
    Vector2 fuelCellCountOffset = new Vector2(391, 161);
    ThirdPersonStatus playerInfo;

    void Awake()
    {
        playerInfo = FindObjectOfType<ThirdPersonStatus>();
    }

    public void OnGUI()
    {

        int itemsLeft = playerInfo.GetRemainingItems();
        int healthPieIndex = Mathf.Clamp(playerInfo.health, 0, healthPieImages.Length);

        if (itemsLeft < 0) itemsLeft = 0;
        GUI.skin = guiSkin;
        GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(Screen.height / nativeVerticalResolution, Screen.height / nativeVerticalResolution, 1));
        DrawImageBottomAligned(healthImageOffset, healthImage);

        Texture2D pieImage = healthPieImages[healthPieIndex - 1];
        DrawImageBottomAligned(healthPieImageOffset, pieImage);
        DrawLabelBottomAligned(livesCountOffset, playerInfo.lives.ToString());
        DrawImageBottomRightAligned(fuelCellOffset, fuelCellsImage);
        DrawLabelBottomRightAligned(fuelCellCountOffset, itemsLeft.ToString());
    }

    void DrawImageBottomAligned(Vector2 pos, Texture2D image)
    {
        GUI.Label(new Rect(pos.x, nativeVerticalResolution - image.height - pos.y, image.width, image.height), image);
    }

    void DrawLabelBottomAligned(Vector2 pos, string text)
    {
        GUI.Label(new Rect(pos.x, nativeVerticalResolution - pos.y, 100, 100), text);
    }

    void DrawImageBottomRightAligned(Vector2 pos, Texture2D image)
    {
        float scaledResolutionWidth = nativeVerticalResolution / Screen.height * Screen.width;
        GUI.Label(new Rect(scaledResolutionWidth - pos.x - image.width, nativeVerticalResolution - image.height - pos.y, image.width, image.height), image);
    }

    void DrawLabelBottomRightAligned(Vector2 pos, string text)
    {
        float scaledResolutionWidth = nativeVerticalResolution / Screen.height * Screen.width;
        GUI.Label(new Rect(scaledResolutionWidth - pos.x, nativeVerticalResolution - pos.y, 100, 100), text);
    }
}