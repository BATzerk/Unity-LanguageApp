using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardView : MonoBehaviour {
    // Components
    [SerializeField] private GameObject go_swipeBannerNo;
    [SerializeField] private GameObject go_swipeBannerYes;
    [SerializeField] private RectTransform myRectTransform;
    [SerializeField] private RectTransform rt_options;
    [SerializeField] private RectTransform rt_sideNative;
    [SerializeField] private RectTransform rt_sideForeign;
    [SerializeField] MoveTermPopup moveTermPopup;
    [SerializeField] TextMeshProUGUI t_native;
    [SerializeField] TextMeshProUGUI t_foreign;
    [SerializeField] TextMeshProUGUI t_phonetic;
    [SerializeField] TMP_InputField if_native;
    [SerializeField] TMP_InputField if_foreign;
    [SerializeField] TMP_InputField if_phonetic;
    // References
    [SerializeField] private PanelStudyFlashcards myPanel;
    public Term MyTerm { get; private set; }
    // Properties
    private bool isNativeSide; // if FALSE, we're showing the foreign side.
    private float timeWhenSetTerm; // Time.time when we last called SetMyTerm.


    // ----------------------------------------------------------------
    //  Setting Visuals
    // ----------------------------------------------------------------
    public void SetMyTerm(Term term) {
        this.MyTerm = term;
        timeWhenSetTerm = Time.time;
        //Debug.Log("Term set: " + MyTerm.mySet);

        // Update visuals!
        UpdateTextFieldsFromTerm();
        HideOptions();
        ShowSideNative(true);

        // Reset swipiness.
        go_swipeBannerNo.SetActive(false);
        go_swipeBannerYes.SetActive(false);
        myRectTransform.anchoredPosition = posNeutral;
        myRectTransform.localEulerAngles = Vector3.zero;
    }

    private void UpdateTextFieldsFromTerm() {
        t_native.text = MyTerm.english;
        t_foreign.text = MyTerm.danish;
        t_phonetic.text = MyTerm.phonetic;
        if_native.text = MyTerm.english;
        if_foreign.text = MyTerm.danish;
        if_phonetic.text = MyTerm.phonetic;
    }

    void ShowSideNative(bool doAnimate=true) {
        isNativeSide = true;
        rt_sideNative.gameObject.SetActive(isNativeSide);
        rt_sideForeign.gameObject.SetActive(!isNativeSide);
    }
    void ShowSideForeign(bool doAnimate=true) {
        isNativeSide = false;
        rt_sideNative.gameObject.SetActive(isNativeSide);
        rt_sideForeign.gameObject.SetActive(!isNativeSide);
    }
    public void FlipCard() {
        if (isNativeSide) ShowSideForeign();
        else ShowSideNative();
    }



    // ----------------------------------------------------------------
    //  Events
    // ----------------------------------------------------------------
    public void OnClickCardFace() {
        // If we're ARE at neutral pos, and haven't JUST set my term, flip the card!
        if (Vector2.Distance(myRectTransform.anchoredPosition, posNeutral) < 1
            && Time.time>timeWhenSetTerm+0.5f) {
            FlipCard();
        }
    }
    public void OnClick_MoveTermButton() {
        moveTermPopup.Show(MyTerm);
    }
    public void HideOptions() {
        rt_options.gameObject.SetActive(false);
    }
    public void ShowOptions() {
        rt_options.gameObject.SetActive(true);
    }
    public void OnEndEditAnyTextField() {
        // Update my actual term now!
        MyTerm.english = if_native.text;
        MyTerm.danish = if_foreign.text;
        MyTerm.phonetic = if_phonetic.text;
        // Save 'em, Joe. :)
        GameManagers.Instance.DataManager.SaveStudySetLibrary();
        // Refresh all my texts now.
        UpdateTextFieldsFromTerm();
    }




    // -------- SWIPING --------
    private bool isSwiping;
    private float swipeRotScale; // randomized every time we click down! For flavor.
    private int swipeOutcomeDir; // -1 no; 0 return to center; 1 yes.
    private Vector2 mouseDownPos;
    private Vector2 posNeutral;

    private Vector2 getMousePos() { return new Vector2(Input.mousePosition.x, Input.mousePosition.y); }

    public void OnCursorDown() {
        isSwiping = true;
        mouseDownPos = getMousePos();// myRectTransform.anchoredPosition - getMousePos();
        // Randomize swipeRotScale.
        swipeRotScale = Random.Range(0.01f, 0.05f);
        if (Random.Range(0, 1f) < 0.5f) swipeRotScale *= -1;
    }
    public void OnCursorUp() {
        isSwiping = false;
        // Far enough to consider this a swipe??
        if (swipeOutcomeDir == -1) {
            myPanel.OnClickNo();
        }
        else if (swipeOutcomeDir == 1) {
            myPanel.OnClickYes();
        }
    }

    private void Update() {
        if (isSwiping) {
            Vector2 posOffset = (getMousePos()-mouseDownPos) / MainCanvas.Canvas.scaleFactor;
            myRectTransform.anchoredPosition = posNeutral + posOffset;
            // Update swipeOutcomeDir!
            if (posOffset.x < -120) swipeOutcomeDir = -1;
            else if (posOffset.x > 120) swipeOutcomeDir = 1;
            else swipeOutcomeDir = 0;
        }
        else {
            myRectTransform.anchoredPosition += (posNeutral-myRectTransform.anchoredPosition) / 4f;
            swipeOutcomeDir = 0;
        }
        float offsetX = posNeutral.x - myRectTransform.anchoredPosition.x;
        // Rotation
        myRectTransform.localEulerAngles = new Vector3(0, 0, offsetX * swipeRotScale);

        // Swipe outcome visuals
        go_swipeBannerNo.SetActive(swipeOutcomeDir == -1);
        go_swipeBannerYes.SetActive(swipeOutcomeDir == 1);
    }




}
