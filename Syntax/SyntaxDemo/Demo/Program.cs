// See https://aka.ms/new-console-template for more information


List<Book> books = new()
{
    new Book("世界观", "巨佬1", 20),
    new Book("博弈论", "巨佬2", 30),
    new Book("编码", "巨佬3", 40),
};

static string PrintTip(Book book) => book switch
{
    { Title: "世界观" } => "这是社科书籍",
    { Price: 30 } => "这本书 30 蚊",
    { Author: "巨佬3" } => "这是巨佬3写的书",
};


foreach (var b in books)
{
    
    Console.WriteLine(b);
    Console.WriteLine(PrintTip(b));
}



Console.ReadLine();


public record Book(string Title, string Author, double Price);

