using Flunt.Validations;
using IWantApp.Domain.Orders;

namespace IWantApp.Domain.Products;

public class Product : Entity
{
    public string Name { get; private set; } = null!;
    public Category? Category { get; private set; }
    public Guid CategoryId { get; private set; }
    public string? Description { get; private set; }
    public bool HasStock { get; private set; } = true;
    public bool Active { get; private set; } = true;
    public decimal Price { get; set; }
    public ICollection<Order>? Orders { get; private set; }

    private Product() { }
    public Product( decimal price)
    {
        Price = price;
    }

    public Product(string name, Category? category, string? description,decimal price, bool hasStock, string createdBy)
    {
        Name = name;
        Category = category;
        Description = description;
        Price = price;
        HasStock = hasStock;
        
        CreatedBy = createdBy;
        EditedBy = null;
        CreatedOn = DateTime.Now;
        EditedOn = null;

        Validate( );
    }
    private void Validate()
    {
        var contract = new Contract<Product>( )
            .IsNotNullOrEmpty(Name,"Name","Nome é obrigatório")
            .IsGreaterOrEqualsThan(Name,4,"Name","Mínimo de 4 caracteres")
            .IsNotNull(Category, "Category", "Categoria nao encontrada")
            .IsGreaterOrEqualsThan(Price, 1,"Price", "O produto precisa de um valor de venda")
            .IsLowerOrEqualsThan(Name,25,"Name","Máximo de 80 caracteres")
            .IsNotNullOrEmpty(CreatedBy,"CreatedBy");
        AddNotifications(contract);
    }

    public void EditInfo(string name, string description,bool hasStock,bool active)
    {
        Name = name;
        Description = description;
        HasStock = hasStock;
        Active = active;

        Validate( );
    }
}
