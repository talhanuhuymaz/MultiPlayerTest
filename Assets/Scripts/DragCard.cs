using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.Netcode;

public class DragCard : NetworkBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    GameObject PrefabToInstantiate;

    [SerializeField]
    RectTransform UIDragElement;

    [SerializeField]
    RectTransform Canvas;

    private Vector2 mOriginalLocalPointerPosition;
    private Vector3 mOriginalPanelLocalPosition;
    private Vector2 mOriginalPosition;

    private bool mIsDragging = false; // Flag to track dragging status

    private NetworkObject spawnedObject; // Reference to the spawned object's NetworkObject

    void Start()
    {
        mOriginalPosition = UIDragElement.localPosition;
    }

    public void OnBeginDrag(PointerEventData data)
    {
        if (!IsOwner)
            return;

        mIsDragging = true;
        mOriginalPanelLocalPosition = UIDragElement.localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(Canvas, data.position, data.pressEventCamera, out mOriginalLocalPointerPosition);
    }

    public void OnDrag(PointerEventData data)
    {
        if (!mIsDragging)
            return;

        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(Canvas, data.position, data.pressEventCamera, out localPointerPosition))
        {
            Vector3 offsetToOriginal = localPointerPosition - mOriginalLocalPointerPosition;
            UIDragElement.localPosition = mOriginalPanelLocalPosition + offsetToOriginal;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        

        mIsDragging = false; // Stop dragging
        StartCoroutine(Coroutine_MoveUIElement(UIDragElement, mOriginalPosition, 0.5f));

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 1000.0f))
        {
            Vector3 worldPoint = hit.point;

            // Call the server-side method to create the object on the server and then synchronize it
            CreateObjectOnServerRpc(worldPoint);
        }
    }

    public IEnumerator Coroutine_MoveUIElement(RectTransform r, Vector2 targetPosition, float duration = 0.1f)
    {
        float elapsedTime = 0;
        Vector2 startingPos = r.localPosition;
        while (elapsedTime < duration)
        {
            r.localPosition = Vector2.Lerp(startingPos, targetPosition, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        r.localPosition = targetPosition;
    }

    // Server-side method to create the object on the server and synchronize it
    [ServerRpc]
    public void CreateObjectOnServerRpc(Vector3 position)
    {
        spawnedObject = Instantiate(PrefabToInstantiate, position + Vector3.up * 0.5f, Quaternion.identity).GetComponent<NetworkObject>();

        // The object is networked and will be synchronized across all clients
    }
}