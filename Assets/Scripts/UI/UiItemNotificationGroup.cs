using System.Collections;
using System.Collections.Generic;
using Inventory.SO;
using Inventory.Structs;
using UnityEngine;
using Utilr;

namespace UI
{
    public class UiItemNotificationGroup : MonoBehaviour
    {
        [SerializeField] private int m_popUpMaxCount = 16;
        [SerializeField] private GameObject m_popUpPrefab = null;
        
        [SerializeField] private ObtainItemEvent m_obtainItemEvent = null;
        [SerializeField] private int m_maxOnScreen = 8;
        [SerializeField] private float m_durBetweenTriggers = 0.1f;
        [SerializeField] private float m_onScreenDuration = 3.5f;

        private readonly Queue<(UiItemCollectedPopup PopUp, int Index)> m_activePopUps = 
            new Queue<(UiItemCollectedPopup PopUp, int Index)>();
        
        private ObjectPool<UiItemCollectedPopup> m_pool = null;

        private void Start()
        {
            m_pool = new ObjectPool<UiItemCollectedPopup>(m_popUpPrefab, m_popUpMaxCount, transform);
            m_obtainItemEvent.Event.AddListener(OnItemCollected);

            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                child.gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            m_obtainItemEvent.Event.RemoveListener(OnItemCollected);
        }

        private void OnItemCollected(ItemCount itemCount)
        {
            StartCoroutine(DisplayPopUpHelper(itemCount));
        }

        private IEnumerator DisplayPopUpHelper(ItemCount itemCount)
        {
            for (int i = 0; i < itemCount.Count; i++)
            {
                (var popUp, int index) = m_pool.GetNextAvailable();
                popUp.gameObject.SetActive(true);
                popUp.transform.SetSiblingIndex(0);
                foreach ((var popUpInQueue, int _) in m_activePopUps)
                {
                    popUpInQueue.MoveUp();
                }

                popUp.Display(itemCount.Id, m_onScreenDuration, () => {
                    m_pool.SetAvailable(index);
                    var front = m_activePopUps.Dequeue();
                    if (front.PopUp != popUp)
                    {
                        Debug.LogError($"Dequeued pop-up {front.PopUp.gameObject} index {front.Index}," +
                            $" expected: {popUp.gameObject} index {index}");
                    }
                    popUp.gameObject.SetActive(false);
                });
                m_activePopUps.Enqueue((popUp, index));
                if (m_activePopUps.Count > m_maxOnScreen)
                {
                    (var earlyExitPopUp, int earlyExitIndex) = m_activePopUps.Dequeue();
                    m_pool.SetAvailable(earlyExitIndex);
                    earlyExitPopUp.EarlyExit();
                    earlyExitPopUp.gameObject.SetActive(false);
                }
                
                yield return new WaitForSeconds(m_durBetweenTriggers);
            }
        }
    }
}
