using System;
using System.Collections.Generic;

namespace PersonalLibraryManagement
{
    // Abstract class for a book
    public abstract class Book
    {
        private string _title;
        private string _author;
        private string _genre;

        public string Title 
        {
            get => _title;
            set => _title = value;
        }

        public string Author 
        {
            get => _author;
            set => _author = value;
        }

        public string Genre 
        {
            get => _genre;
            set => _genre = value;
        }

        protected Book(string title, string author, string genre)
        {
            _title = title;
            _author = author;
            _genre = genre;
        }

        public abstract void DisplayDetails();
    }

    // Interface for borrowable items
    public interface IBorrowable
    {
        void Borrow(string borrower, DateTime dueDate);
        void Return();
    }

    // Concrete class for owned books
    public class OwnedBook : Book
    {
        private DateTime _purchaseDate;
        private string _condition;

        public DateTime PurchaseDate
        {
            get => _purchaseDate;
            set => _purchaseDate = value;
        }

        public string Condition
        {
            get => _condition;
            set => _condition = value;
        }

        public OwnedBook(string title, string author, string genre, DateTime purchaseDate, string condition)
            : base(title, author, genre)
        {
            _purchaseDate = purchaseDate;
            _condition = condition;
        }

        public override void DisplayDetails()
        {
            Console.WriteLine($"Owned Book: {Title} by {Author}, Genre: {Genre}, Purchased on: {PurchaseDate}, Condition: {Condition}");
        }
    }

    // Concrete class for borrowed books
    public class BorrowedBook : Book, IBorrowable
    {
        private string _borrower;
        private DateTime _dueDate;

        public string Borrower
        {
            get => _borrower;
            private set => _borrower = value;
        }

        public DateTime DueDate
        {
            get => _dueDate;
            private set => _dueDate = value;
        }

        public BorrowedBook(string title, string author, string genre)
            : base(title, author, genre)
        {
        }

        public void Borrow(string borrower, DateTime dueDate)
        {
            _borrower = borrower;
            _dueDate = dueDate;
        }

        public void Return()
        {
            _borrower = null;
            _dueDate = DateTime.MinValue;
        }

        public override void DisplayDetails()
        {
            Console.WriteLine($"Borrowed Book: {Title} by {Author}, Genre: {Genre}, Borrowed by: {Borrower}, Due on: {DueDate}");
        }
    }

    // Concrete class for wishlist books
    public class WishlistBook : Book
    {
        private DateTime _desiredDate;
        private int _priority;

        public DateTime DesiredDate
        {
            get => _desiredDate;
            set => _desiredDate = value;
        }

        public int Priority
        {
            get => _priority;
            set => _priority = value;
        }

        public WishlistBook(string title, string author, string genre, DateTime desiredDate, int priority)
            : base(title, author, genre)
        {
            _desiredDate = desiredDate;
            _priority = priority;
        }

        public override void DisplayDetails()
        {
            Console.WriteLine($"Wishlist Book: {Title} by {Author}, Genre: {Genre}, Desired by: {DesiredDate}, Priority: {Priority}");
        }
    }

    // Library class to manage books
    public class Library
    {
        private readonly List<Book> _books = new List<Book>();

        public void AddBook(Book book)
        {
            _books.Add(book);
            Console.WriteLine("Book added.");
        }

        public void RemoveBook(Book book)
        {
            _books.Remove(book);
            Console.WriteLine("Book removed.");
        }

        public List<Book> SearchBooks(string searchTerm)
        {
            return _books.FindAll(b => b.Title.Contains(searchTerm) || b.Author.Contains(searchTerm) || b.Genre.Contains(searchTerm));
        }

        public void DisplayAllBooks()
        {
            foreach (var book in _books)
            {
                book.DisplayDetails();
            }
        }
    }

    // Console UI for interacting with the library
    public class ConsoleUI
    {
        private readonly Library _library = new Library();

        public void Start()
        {
            string command;
            do
            {
                Console.WriteLine("Enter command (add, remove, search, display, exit):");
                command = Console.ReadLine().ToLower();

                switch (command)
                {
                    case "add":
                        AddBook();
                        break;
                    case "remove":
                        RemoveBook();
                        break;
                    case "search":
                        SearchBooks();
                        break;
                    case "display":
                        DisplayAllBooks();
                        break;
                }

            } while (command != "exit");
        }

        private void AddBook()
        {
            Console.WriteLine("Enter book title:");
            string title = Console.ReadLine();
            Console.WriteLine("Enter book author:");
            string author = Console.ReadLine();
            Console.WriteLine("Enter book genre:");
            string genre = Console.ReadLine();
            Console.WriteLine("Enter book type (Owned, Borrowed, Wishlist):");
            string type = Console.ReadLine();

            if (type.Equals("Owned", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Enter purchase date (yyyy-MM-dd):");
                DateTime purchaseDate = DateTime.Parse(Console.ReadLine());
                Console.WriteLine("Enter condition:");
                string condition = Console.ReadLine();
                _library.AddBook(new OwnedBook(title, author, genre, purchaseDate, condition));
            }
            else if (type.Equals("Borrowed", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Enter borrower name:");
                string borrower = Console.ReadLine();
                Console.WriteLine("Enter due date (yyyy-MM-dd):");
                DateTime dueDate = DateTime.Parse(Console.ReadLine());
                BorrowedBook borrowedBook = new BorrowedBook(title, author, genre);
                borrowedBook.Borrow(borrower, dueDate);
                _library.AddBook(borrowedBook);
            }
            else if (type.Equals("Wishlist", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Enter desired date (yyyy-MM-dd):");
                DateTime desiredDate = DateTime.Parse(Console.ReadLine());
                Console.WriteLine("Enter priority (1-5):");
                int priority = int.Parse(Console.ReadLine());
                _library.AddBook(new WishlistBook(title, author, genre, desiredDate, priority));
            }
            else
            {
                Console.WriteLine("Invalid book type. Please try again.");
            }
        }

        private void RemoveBook()
        {
            Console.WriteLine("Enter title of the book to remove:");
            string title = Console.ReadLine();
            var books = _library.SearchBooks(title);

            if (books.Count > 0)
            {
                _library.RemoveBook(books[0]);
                Console.WriteLine("Book removed.");
            }
            else
            {
                Console.WriteLine("Book not found.");
            }
        }

        private void SearchBooks()
        {
            Console.WriteLine("Enter search term:");
            string searchTerm = Console.ReadLine();
            var books = _library.SearchBooks(searchTerm);

            if (books.Count > 0)
            {
                foreach (var book in books)
                {
                    book.DisplayDetails();
                }
            }
            else
            {
                Console.WriteLine("No books found.");
            }
        }

        private void DisplayAllBooks()
        {
            _library.DisplayAllBooks();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var ui = new ConsoleUI();
            ui.Start();
        }
    }
}
