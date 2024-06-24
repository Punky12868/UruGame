using Febucci.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChangeLevelsText : MonoBehaviour
{
    ChapterSelector chapterSelector;
    [SerializeField] private TypewriterByCharacter chapter_typewriter;
    [SerializeField] private TypewriterByCharacter chapterName_typewriter;
    [SerializeField] private TypewriterByCharacter levelName_typewriter;

    [SerializeField] private TMPro.TMP_Text chapter;
    [SerializeField] private TMPro.TMP_Text chapterName;
    [SerializeField] private TMPro.TMP_Text levelID;
    [SerializeField] private TMPro.TMP_Text levelName;

    [TextArea][SerializeField] private string[] chapterTexts;
    [TextArea][SerializeField] private string[] chapterNameTexts;

    private Button[][] chapterButtons;

    private LevelButton selectedLevel;
    private int currentChapter = 0;

    private void Awake()
    {
        chapterSelector = FindObjectOfType<ChapterSelector>();
        chapterButtons = new Button[4][];
        chapterButtons[0] = chapterSelector.GetChaptersButtons(0);
        chapterButtons[1] = chapterSelector.GetChaptersButtons(1);
        chapterButtons[2] = chapterSelector.GetChaptersButtons(2);
        chapterButtons[3] = chapterSelector.GetChaptersButtons(3);

        chapter.text = chapterTexts[0];
        chapterName.text = chapterNameTexts[0];
        levelID.text = "1/" + chapterButtons[currentChapter].Length.ToString();
        levelName.text = chapterButtons[0][0].GetComponent<LevelButton>().GetLevelName();

        chapter_typewriter.onTextShowed.AddListener(ShowChapterNameAfterChapterIndex);
    }

    private void Update()
    {
        if (EventSystem.current == null || !FindObjectOfType<ChapterSelector>().Active)
        {
            currentChapter = 999;
            return;
        }

        if (currentChapter != FindObjectOfType<ChapterSelector>().GetCurrentChapter())
        {
            currentChapter = FindObjectOfType<ChapterSelector>().GetCurrentChapter();

            EraseText(chapter);
            EraseText(chapterName);

            WriteText(chapterTexts[currentChapter], chapter, chapter_typewriter);
            WriteText(chapterNameTexts[currentChapter], chapterName, chapterName_typewriter);
        }

        
        GameObject selectedGameObject = EventSystem.current.currentSelectedGameObject;
        if (selectedGameObject != null && selectedGameObject.GetComponent<LevelButton>() != null)
        {
            if (selectedLevel != null && selectedLevel == selectedGameObject.GetComponent<LevelButton>()) return;

            selectedLevel = selectedGameObject.GetComponent<LevelButton>();
            EraseText(levelName);

            levelID.text = selectedLevel.GetLevelID().ToString() + "/" + chapterButtons[currentChapter].Length.ToString();
            WriteText(selectedLevel.GetLevelName(), levelName, levelName_typewriter);
        }
    }

    public void ShowChapterNameAfterChapterIndex()
    {
        WriteText(chapterNameTexts[currentChapter], chapterName, chapterName_typewriter);
    }

    private void EraseText(TMPro.TMP_Text text)
    {
        text.text = "";
    }

    private void WriteText(string text, TMPro.TMP_Text textComponent, TypewriterByCharacter typewriter)
    {
        if (textComponent.text == text) return;

        //typewriter.ShowText(text);
        //typewriter.StartShowingText();

        textComponent.text = text;
    }
}
