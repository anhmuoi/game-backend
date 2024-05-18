namespace ERPSystem.DataModel.Category;

public class CategoryAddModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public short Type { get; set; }
    public string Color { get; set; }
}

public class CategoryEditModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public short Type { get; set; }
    public string Color { get; set; }
}

public class CategoryListModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public short Type { get; set; }
    public string Color { get; set; }
}

public class CategoryModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public short Type { get; set; }
    public string Color { get; set; }
}