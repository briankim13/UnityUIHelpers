using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

[ExecuteAlways]
public class AutoLayoutContents : MonoBehaviour
{
    public int Rows = 0;
    public float PaddingLeft = 0f;
    public float PaddingRight = 0f;
    public float PaddingTop = 0f;

    public float SpacingHorizontal = 1f;
    public float SpacingVertical = 1f;

    public bool ConstantlyUpdate = false;
    protected float lastUpdateTime; 

    void OnEnable()
    {
        LayoutChildrenObjects(gameObject);
    }

    private void Update()
    {
        if (ConstantlyUpdate && Time.time > lastUpdateTime + 1f)
        {
            LayoutChildrenObjects(gameObject);
            lastUpdateTime = Time.time;
        }
    }

    public void LayoutChildrenObjects(GameObject parent = null)
    {
        LayoutChildrenObjectsAsync(parent);
    }

    public async Task<bool> LayoutChildrenObjectsAsync(GameObject parent = null)
    {
        await Task.Yield();
        // TODO: take variable height scenario into consideration
        if (parent == null)
        {
            parent = gameObject;
        }
        RectTransform parentRectTransform = parent.GetComponent<RectTransform>();
        Rows = 0;
        if (parent.transform.childCount < 1)
        {
            return true;
        }
        RectTransform rectTransform = parent.transform.GetChild(0).gameObject.GetComponent<RectTransform>();
        RectTransform prevRectTransform = rectTransform;
        bool isFirst = true;
        Rows = 1;
        Vector2 firstPos = new Vector2(-parentRectTransform.rect.width / 2f + rectTransform.rect.width / 2f + PaddingLeft,
            parentRectTransform.rect.height / 2f - rectTransform.rect.height / 2f - PaddingTop);
        UIPositionHelper.SetAbsoluteAnchoredCenterPosition(rectTransform, firstPos);

        // Start iterating through children
        Vector2 pos = UIPositionHelper.GetAbsoluteAnchoredCenterPosition(rectTransform);
        Vector2 offSet; // how much further from the previous object
        foreach (Transform child in parent.transform)
        {
            rectTransform = child.gameObject.GetComponent<RectTransform>();
            if (isFirst)
            {
                isFirst = false;
                continue;
            }
            offSet.x = prevRectTransform.rect.width / 2f + rectTransform.rect.width / 2f + SpacingHorizontal;
            offSet.y = 0f;

            // When overflowing to the right, go to next row
            if (pos.x + offSet.x + rectTransform.rect.width / 2f + PaddingRight > parentRectTransform.rect.width / 2f)
            {
                offSet.y = -prevRectTransform.rect.height - SpacingVertical;
                pos = pos + offSet;
                pos.x = -parentRectTransform.rect.width / 2f + rectTransform.rect.width / 2f + PaddingLeft;
                Rows++;
            }
            else
            {
                pos = pos + offSet;
            }

            UIPositionHelper.SetAbsoluteAnchoredCenterPosition(rectTransform, pos);

            prevRectTransform = rectTransform;
        }
        return true;
    }

}
