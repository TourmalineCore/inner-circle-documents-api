namespace Core;

public class Document
{
  public long Id { get; set; }

  public string Name { get; set; }

  public Document(string name)
  {
    Name = name;
  }

  private Document() { }
}
