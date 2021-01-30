using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Exodia
{
    // A basic console window that can:
    // - Be moved on right mouse held down on hover
    // - Be resized on corner hover and left mouse drag

    // Note: Place this script on the console's parent Canvas!
    public class DevConsole : MonoBehaviour
    {
        public static DevConsole instance { get; private set; }

    // Settings
        ///<summary>Whether or not the Console window can be moved outside the Screen viewport.</summary>
        public bool lockInsideScreen = false;
        ///<summary>Whether or not the Console window can be resized smaller or bigger.</summary>
        public bool lockSize = false;
        ///<summary>What gets displayed in the console.</summary>
        public eConsoleLog consoleLog = eConsoleLog.validated;

    // Window and its resizing
        // window moving
        private bool isHover;
        private int margin = 15;
        private bool canResize;
        private Vector2 offPos, offPosClamp, startPos;
        
        // window resizing
        public bool isResize;
        private Vector2 origPos;
        private Vector2 origSize;

        private Vector2 dragPos1;
        public Vector2 dragPos2;

        private float minWidth = 50;
        private float minMeight = 150;

        private Transform window, scrollContent;
        private GameObject Border;
        private GameObject[] Borders;
        private RectTransform rect, scrollContentRect;
        private Canvas canvas;

    // Console Commands
        private TMP_InputField consoleInput;

        private void Start() {
            if (instance != null) {
                Destroy(gameObject);
                return;
            }
            instance = this;

            // Window and its resizing
            canvas = GetComponent<Canvas>();
            window = transform.Find("console-window");
            scrollContent = window.Find("scroll-view/viewport/content");
            scrollContentRect = scrollContent.GetComponent<RectTransform>();
            rect = window.GetComponent<RectTransform>();
            startPos = rect.anchoredPosition;
            
            Border = window.Find("borders").gameObject;
            List<GameObject> _borders = new List<GameObject>();
            foreach (Transform element in Border.transform) _borders.Add(element.gameObject);
            Borders = _borders.ToArray();

            // Console Commands
            consoleInput = window.GetComponentInChildren<TMP_InputField>();
        }

        private void Update() {
            if (!canvas.enabled) return;

        // Window and its resizing
            Vector2 mPos = Input.mousePosition;

            // is cursor inside the window?
            isHover = offPos.x > 0 && offPos.x < rect.sizeDelta.x && offPos.y > 0 && offPos.y < rect.sizeDelta.y;

            bool isLeftBottomCorner = (offPos.x < margin && offPos.x > -margin && offPos.y < margin && offPos.y > -margin);
            bool isLeftTopCorner = (offPos.x < margin && offPos.x > -margin && offPos.y < rect.sizeDelta.y + margin && offPos.y > rect.sizeDelta.y - margin);
            bool isRightTopCorner = (offPos.x < rect.sizeDelta.x + margin && offPos.x > rect.sizeDelta.x - margin && offPos.y < rect.sizeDelta.y + margin && offPos.y > rect.sizeDelta.y - margin);
            bool isRightBottomCorner = (offPos.x < rect.sizeDelta.x + margin && offPos.x > rect.sizeDelta.x - margin && offPos.y < margin && offPos.y > -margin);
            bool[] bs = { isLeftBottomCorner, isLeftTopCorner, isRightTopCorner, isRightBottomCorner };

            // is curson inside any of the corners?
            bool isHoverOnEdge = isLeftBottomCorner || isLeftTopCorner || isRightTopCorner || isRightBottomCorner;

            canResize = !lockSize && isHoverOnEdge && !Input.GetMouseButton(1);
            Border.SetActive(canResize);
            for (int i = 0; i < Borders.Length; i++) Borders[i].SetActive(bs[i]);

            if (Input.GetMouseButtonDown(0)) isResize = canResize;
            if (Input.GetMouseButtonUp(0)) isResize = false;

            // Move the window if right-mouse is pressed
            if (isHover && Input.GetMouseButton(1)) {
                rect.anchoredPosition = mPos - offPosClamp;
            } else if (isResize) { // Resize the window on corner hover and left-mouse

                dragPos2 = mPos - dragPos1;
                float sizeX = origSize.x - dragPos2.x; // How much we have resized from the selected corner in the x-axis
                float sizeY = origSize.x + dragPos2.y; // How much we have resized from the selected corner in the y-axis
                
                // If we are resizing the top-left corner and we can make it smaller
                if (isLeftTopCorner && sizeX > minWidth) {
                    rect.anchoredPosition = new Vector2(origPos.x + dragPos2.x, rect.anchoredPosition.y);
                    rect.sizeDelta = new Vector2(Mathf.Clamp(sizeX, minWidth, sizeX), Mathf.Clamp(sizeY, minMeight, sizeY));
                }

                
                // TODO: ADD OTHER CORNERS' RESIZING

                
                //bottom-left
                
                //top-right
                
                //bottom-right

                else {

                }

            } else {
                if (!consoleInput.isFocused) consoleInput.ActivateInputField();
            
                // window resizing
                dragPos1 = mPos;
                dragPos2 = Vector2.zero;

                origPos = rect.anchoredPosition;
                origSize = rect.sizeDelta;

                // window moving
                float x = -rect.anchoredPosition.x + mPos.x;
                float y = -rect.anchoredPosition.y + mPos.y;
                offPos = new Vector2(x, y);
                offPosClamp = new Vector2(Mathf.Clamp(x, 0, rect.sizeDelta.x), Mathf.Clamp(y, 0, rect.sizeDelta.y));

            // Console Commands
                if (Input.GetKeyDown(KeyCode.Return)) {
                    Process(consoleInput.text);
                    consoleInput.text = "";
                    consoleInput.ActivateInputField();
                }
            }
        }

        private void Process(string input) {
            if (input == "") return;
            
            // convert all characters in the input to lower characters, so we don't get errors because of that
            input = input.ToLower();

            // if the command is not valid, let's say that the input is 'iodjwaidhauwd bvu'. we obviously don't want to log that into the console
            if (consoleLog != eConsoleLog.everything) {
                if (validationPointer(consoleInput.text) == "") return;
                else if (consoleLog == eConsoleLog.validated) AddText(input);
            }
            // if we want to log the proper command we have typed into the console. (creates clutter)
            else AddText(input);

            // then lastly, trigger an event to signal that we have done a command and log its output into the console
            string process = processPointer(consoleInput.text);
            if (process != "") AddText(process);
        }

        private void AddText(string input) {
            float margin = 5;   // the margin for the text from the top and bottom
            float height = 40;  // the height of the text element
            int index = scrollContent.childCount;
            
            scrollContentRect.sizeDelta = new Vector2(0, 2 * margin + (height * (index + 1)));
            RectTransform text = new GameObject($"{index}", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI)).GetComponent<RectTransform>();
            text.SetParent(scrollContent);
            text.localPosition = Vector3.zero;
            text.anchoredPosition = new Vector2(10, -margin + index * -height);
            text.rotation = transform.rotation;
            text.localScale = Vector3.one;
            text.sizeDelta = new Vector2(0, height);
            text.anchorMin = new Vector2(0, 1);
            text.anchorMax = new Vector2(1, 1);
            text.pivot = new Vector2(.5f, 1);

            //todo: add a CommandListener script that checks whether or not the command was valid...
            // if true, have the command line as green, otherwise as red.

            text.GetComponent<TMP_Text>().text = input;

            // automatically scroll the view to the bottom
            scrollContentRect.localPosition = new Vector2(0, index * height - 2 * margin);
        }

        ///<summary>Clears the console completely from all logs.</summary>
        public void Clear() {
            foreach (Transform element in scrollContent) {
                Destroy(element.gameObject);
            }
            scrollContentRect.sizeDelta = new Vector2(scrollContentRect.sizeDelta.x, 0);
        }

        ///<summary>This pointer finds out whether or not the command is valid.</summary>
        public static validation validationPointer;
        public delegate string validation(string input);

        ///<summary>This pointer calls the function.</summary>
        public static process processPointer;
        public delegate string process(string input);

        ///<summary>Toggles the console's visibility.</summary>
        public void Toggle() {
            if (consoleInput.IsActive()) consoleInput.DeactivateInputField();
            canvas.enabled = !canvas.enabled;

            // Press F11 and right ctrl to return the console to the same starting position that gets set on start
            if (canvas.enabled && Input.GetKey(KeyCode.RightControl)) Home();
        }

        ///<summary>Returns the console window back to the starting position.</summary>
        public void Home() {
            if (canvas.enabled) rect.anchoredPosition = startPos;
        }
    }
}