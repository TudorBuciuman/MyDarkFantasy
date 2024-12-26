using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BookManager : MonoBehaviour
{
    public Button nextPageButton;
    public Button previousPageButton;
    public Text Title;
    public Text pagenr;
    public Text Page1;
    public Text Page2;

    private Book currentBook;
    private int currentPageIndex;

    public void InteractWithBook(Book book)
    {
        OpenBook(book);
    }
    public void OpenBook(Book book)
    {
        currentBook = book;
        currentPageIndex = 0;

        // Optional: Display title and author
        Title.text = book.title;
        //authorText.text = "by " + book.author;

        UpdatePage();
    }
    public Book LoadBook(string fileName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<Book>(json);
        }
        Debug.LogError("Book file not found: " + fileName);
        return null;
    }
    public EditableBook CreateNewBook(string title, string author)
    {
        EditableBook newBook = ScriptableObject.CreateInstance<EditableBook>();
        newBook.title = title;
        newBook.author = author;
        newBook.pages.Add("Start writing your book here...");
        return newBook;
    }
    public void NextPage()
    {
        if (currentPageIndex < currentBook.pages.Count - 1)
        {
            currentPageIndex++;
            UpdatePage();
        }
    }

    public void PreviousPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            UpdatePage();
        }
    }

    private void UpdatePage()
    {
        pagenr.text = currentBook.pages[currentPageIndex];
        previousPageButton.interactable = currentPageIndex > 0;
        nextPageButton.interactable = currentPageIndex < currentBook.pages.Count - 1;
    }

}
[System.Serializable]
public class Book
{
    public string title;
    public string author;
    public List<string> pages; 
}

[CreateAssetMenu(fileName = "NewBook", menuName = "Book")]
public class EditableBook : ScriptableObject
{
    public string title;
    public string author;
    public List<string> pages = new List<string>();
}