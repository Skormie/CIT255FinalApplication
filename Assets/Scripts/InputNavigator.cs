using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputNavigator : MonoBehaviour
{
    EventSystem system;
    Selectable next = null;

    void Start()
    {
        system = EventSystem.current;// EventSystemManager.currentSystem;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown() ?? next;

            if (next != null)
            {

                var pointer = next.GetComponent<InputField> ();

                if (pointer != null)
                    pointer.OnPointerClick(new PointerEventData(system));  //if it's an input field, also set the text caret

                system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
            }

        }
    }
}
