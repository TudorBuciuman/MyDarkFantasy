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
    public Text pagenr1,pagenr2;
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
        currentPageIndex = -1;
        readingBook = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Toolbar.instance.openedInv = true;

        Title.text = book.title;
        authorText.text =book.author;
        BookObj.SetActive(true);
        StartCoroutine(GetInput());
        UpdatePage();
    }
    public void OpenABook()
    {
        string name = "Book"+(1+Voxeldata.PlayerData.scene).ToString();
        currentBook=LoadBook(name);
        OpenBook(currentBook);
    }
    public IEnumerator GetInput()
    {
        while (currentBook != null)
        {
            if(Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A))
            {
                PreviousPage();
            }
            else if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D))
            {
                NextPage();
            }
            if (Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.X))
            {
                CloseBook();
            }
            yield return null;
        }
    }
    public void CloseBook()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        BookObj.SetActive(false);
        currentBook = null;
        readingBook = false;
        Toolbar.instance.openedInv = false;
    }
    public Book LoadBook(string fileName)
    {
        EditableBook bookSO = Resources.Load<EditableBook>("Books/"+fileName);
        Book book = new()
        {
            author = bookSO.author,
            title = bookSO.title,
            pages = bookSO.pages
        };
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
        if (currentPageIndex < currentBook.pages.Count-1)
        {
            currentPageIndex+=2;
            UpdatePage();
        }

    }

    public void PreviousPage()
    {
        if (currentPageIndex >= 2)
        {
            currentPageIndex-=2;
            UpdatePage();
        }
    }

    private void UpdatePage()
    {
        if (currentPageIndex >= 0)
        {
            Page1.text = currentBook.pages[currentPageIndex-1];
            if (currentPageIndex < currentBook.pages.Count)
            {
                Page2.text = currentBook.pages[currentPageIndex];
                pagenr2.text = (currentPageIndex+1).ToString();
            }
            else
            {
                Page2.text = null;
                pagenr2.text= null;
            }
            pagenr1.text = (currentPageIndex).ToString();
            Cover.SetActive(false);
            pages.SetActive(true);
        }
        else
        {
            Cover.SetActive(true);
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

