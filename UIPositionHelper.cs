using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIPositionHelper
{
    static public Vector2 GetAbsoluteAnchoredLeftBottomPosition(RectTransform rectTransform)
    {
        Vector2 absPos = GetAbsoluteAnchoredCenterPosition(rectTransform);
        return absPos - new Vector2(rectTransform.rect.width / 2f, rectTransform.rect.height / 2f);
    }

    static public Vector2 GetAbsoluteAnchoredRightTopPosition(RectTransform rectTransform)
    {
        Vector2 absPos = GetAbsoluteAnchoredCenterPosition(rectTransform);
        return absPos + new Vector2(rectTransform.rect.width / 2f, rectTransform.rect.height / 2f);
    }

    /// <summary>
    /// Use this to find the position of the rect transform regardless of the anchor settings! 
    /// This will return the position w.r.t. center point. So this will return position as if anchor
    /// was set to (0, 0) center middle. 
    /// </summary>
    /// <param name="rectTransform"></param>
    /// <returns></returns>
    static public Vector2 GetAbsoluteAnchoredCenterPosition(RectTransform rectTransform)
    {
        // Get current anchor's position (could be center, left bottom corner, middle right, etc...
        Vector2 anchorPos = GetAnchorPos(rectTransform);
        Matrix4x4 m = GetMatrix(anchorPos);
        // Transform the anchored center position w.r.t. current anchor position to anchored center position w.r.t. (0,0)
        return m.MultiplyPoint(GetAnchoredCenterPosition(rectTransform));
    }

    static public void SetAbsoluteAnchoredCenterPosition(RectTransform rectTransform, Vector2 posToSet)
    {
        Vector2 anchorPos = GetAnchorPos(rectTransform);
        Matrix4x4 m = GetMatrix(anchorPos);
        m = m.inverse;
        SetAnchoredCenterPosition(rectTransform, m.MultiplyPoint(posToSet));
    }

    /// <summary>
    /// Get the anchored center position no matter what the pivot is. This will always return anchored
    /// position of the center point (the point when you set pivot to 0.5 0.5). 
    /// </summary>
    /// <param name="rectTransform"></param>
    /// <returns></returns>
    static public Vector2 GetAnchoredCenterPosition(RectTransform rectTransform)
    {
        Vector2 pivot = rectTransform.pivot;
        Vector2 objectSize = new Vector2(rectTransform.rect.width, rectTransform.rect.height);
        Vector2 anchoredPos = rectTransform.anchoredPosition;

        return new Vector2(anchoredPos.x + objectSize.x * (0.5f - pivot.x), anchoredPos.y + objectSize.y * (0.5f - pivot.y));
    }

    /// <summary>
    /// Set version of the same net getter. This will set the center position of the rect transform as 
    /// the input posToSet. So this is like doing rectTransform.anchoredPosition = posToSet after changing
    /// pivot to (0.5, 0.5)
    /// </summary>
    /// <param name="rectTransform"></param>
    /// <param name="posToSet"> This is where you want the center position (when pivot was set to 0.5 0.5) to be</param>
    static public void SetAnchoredCenterPosition(RectTransform rectTransform, Vector2 posToSet)
    {
        Vector2 pivot = rectTransform.pivot;
        Vector2 objectSize = new Vector2(rectTransform.rect.width, rectTransform.rect.height);
        Vector2 anchorPosToSet;

        anchorPosToSet.x = posToSet.x - objectSize.x * (0.5f - pivot.x);
        anchorPosToSet.y = posToSet.y - objectSize.y * (0.5f - pivot.y);
        rectTransform.anchoredPosition = anchorPosToSet;
    }

    static public Vector2 GetAnchorPosNormalized(GameObject gObject)
    {
        return GetAnchorPosNormalized(gObject.GetComponent<RectTransform>());
    }

    /// <summary>
    /// Getting one vector from unity's anchorMin and anchorMax. This vector
    /// is what unity calculates anchored position based on. 
    /// Anchor left bottom corner will return (0, 0), right top corner (1, 1), top expand (0.5, 1), 
    /// right expand (1, 0.5), both expand (0.5, 0.5)
    /// </summary>
    /// <param name="rectTransform"></param>
    /// <returns></returns>
    static public Vector2 GetAnchorPosNormalized(RectTransform rectTransform)
    {
        return (rectTransform.anchorMin + rectTransform.anchorMax) / 2f;
    }

    static public Vector2 GetAnchorPosNormalizedZeroed(GameObject gObject)
    {
        return GetAnchorPosNormalizedZeroed(gObject.GetComponent<RectTransform>());
    }

    /// <summary>
    /// Slightly different version GetAnchorPosNormalized where
    /// center is (0, 0), so left bottom corner will return (-0.5, -0.5), 
    /// top right corner will return (0.5, 0.5), middle right will return (0.5, 0), ...
    /// </summary>
    /// <param name="rectTransform"></param>
    /// <returns></returns>
    static public Vector2 GetAnchorPosNormalizedZeroed(RectTransform rectTransform)
    {
        Vector2 normalizedAnchorPos = GetAnchorPosNormalized(rectTransform);
        return new Vector2(normalizedAnchorPos.x - 0.5f, normalizedAnchorPos.y - 0.5f);
    }

    static public Vector2 GetAnchorPos(GameObject gObject)
    {
        return GetAnchorPos(gObject.GetComponent<RectTransform>());
    }

    /// <summary>
    /// Get the position of the anchor that rect transform find the position based on
    /// </summary>
    /// <param name="rectTransform"></param>
    /// <returns></returns>
    static public Vector2 GetAnchorPos(RectTransform rectTransform)
    {
        Vector2 normalizedAnchorPos = GetAnchorPosNormalizedZeroed(rectTransform); // (0, 0), (-0.5, -0.5), (-0.5, 0.5), (0, 0.5), ..., (0.5, 0.5) etc.
        Vector2 parentSize = GetParentSize(rectTransform.gameObject); // (100, 400), whatever...
        return new Vector2(normalizedAnchorPos.x * parentSize.x, normalizedAnchorPos.y * parentSize.y);
    }

    static public Vector2 GetParentSize(GameObject gObject)
    {
        Rect rect = gObject.transform.parent.gameObject.GetComponent<RectTransform>().rect;
        return new Vector2(rect.width, rect.height);
    }

    static public Matrix4x4 GetMatrix(Vector3 translation)
    {
        return Matrix4x4.TRS(translation, Quaternion.identity, Vector3.one);
    }
}
