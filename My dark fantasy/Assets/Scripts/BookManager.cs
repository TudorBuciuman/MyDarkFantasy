using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BookManager : MonoBehaviour
{
    public Text Title;
    public Text pagenr;
    public Text Page1;
    public Text Page2;
    public GameObject BookObj;
    private Book currentBook;
    private int currentPageIndex;
    public void FixedUpdate()
    {
        if(Input.GetKeyUp(KeyCode.V)) { 
        OpenABook();
        }
    }
    public void InteractWithBook(Book book)
    {
        OpenBook(book);
    }
    public void OpenBook(Book book)
    {
        currentBook = book;
        currentPageIndex = 0;

        // Optional: Display title and author
        //Title.text = book.title;
        //authorText.text = "by " + book.author;
        BookObj.gameObject.SetActive(true);

        UpdatePage();
    }
    public void OpenABook()
    {
        string name = "Book1";
        currentBook=LoadBook(name);
    }
    public IEnumerator GetInput()
    {
        while (currentBook != null)
        {
            if(Input.GetKeyDown(KeyCode.LeftArrow))
            {
                PreviousPage();
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                NextPage();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseBook();
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
    public void CloseBook()
    {
        BookObj.gameObject.SetActive(false);
        currentBook = null;
    }
    public Book LoadBook(string fileName)
    {
        string path = Path.Combine(Application.dataPath+"/Data/Books/" ,fileName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            Debug.Log(path);
            return JsonUtility.FromJson<Book>(json);
        }
        Debug.LogError("Book file not found: " + path);
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