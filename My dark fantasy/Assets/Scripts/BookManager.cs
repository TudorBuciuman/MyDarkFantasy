using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BookManager : MonoBehaviour
{
    public Text Title,authorText;
    public static bool readingBook = false;
    public static BookManager instance;
    public Text pagenr;
    public Text Page1;
    public Text Page2;
    public GameObject BookObj;
    public GameObject Cover,pages;
    private Book currentBook;
    private int currentPageIndex;
    public void Start()
    {
        instance = this;
    }
    public void InteractWithBook(Book book)
    {
        OpenBook(book);
    }
    public void OpenBook(Book book)
    {
        currentBook = book;
        currentPageIndex = 0;
        readingBook = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Toolbar.instance.openedInv = false;
        Title.text = book.title;
        authorText.text =book.author;
        BookObj.gameObject.SetActive(true);
        StartCoroutine(GetInput());
        UpdatePage();
    }
    public void OpenABook()
    {
        string name = "Book1";
        currentBook=LoadBook(name);
        OpenBook(currentBook);
    }
    public IEnumerator GetInput()
    {
        while (currentBook != null)
        {
            if(Input.GetKeyUp(KeyCode.LeftArrow))
            {
                PreviousPage();
            }
            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                NextPage();
            }
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                CloseBook();
            }
            yield return new WaitForSeconds(0.01f);
        }
    }
    public void CloseBook()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        BookObj.gameObject.SetActive(false);
        currentBook = null;
        readingBook = false;
        Toolbar.instance.openedInv = false;
    }
    public Book LoadBook(string fileName)
    {
        EditableBook bookSO = Resources.Load<EditableBook>("Books/"+fileName);
        Book book=new();
        book.author =bookSO.author;
        book.title = bookSO.title;
        book.pages = bookSO.pages;
        return book;
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
        if (currentPageIndex < currentBook.pages.Count)
        {
            currentPageIndex++;
            UpdatePage();
        }
    }

    public void PreviousPage()
    {
        if (currentPageIndex >= 0)
        {
            currentPageIndex--;
            UpdatePage();
        }
    }

    private void UpdatePage()
    {
        if (currentPageIndex > 0)
        {
            Page1.text = currentBook.pages[currentPageIndex-1];
            pagenr.text = (currentPageIndex + 1).ToString();
            Cover.gameObject.SetActive(false);
            pages.SetActive(true);
        }
        else
        {
            Cover.gameObject.SetActive(true);
            pages.SetActive(false);
        }
    }

}
[System.Serializable]
public class Book
{
    public string title;
    public string author;
    public List<string> pages; 
}

