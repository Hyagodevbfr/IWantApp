using Flunt.Validations;

namespace IWantApp.Domain.Products;

public class Category : Entity
{
    public string Name { get; private set; } = null!;
    public bool Active { get; private set; }

    public Category(string name, string createdBy)
    {
        Name = name;
        Active = true;
        CreatedBy = createdBy;
        CreatedOn = DateTime.Now;
        EditedBy = null;
        EditedOn = null;
     
        Validate( );
    }

    private void Validate()
    {
        var contract = new Contract<Category>( )
            .IsNotNullOrEmpty(Name,"Name","Nome é obrigatório")
            .IsGreaterOrEqualsThan(Name,4,"Name","Mínimo de 4 caracteres")
            .IsLowerOrEqualsThan(Name,25,"Name","Máximo de 25 caracteres")
            .IsNotNullOrEmpty(CreatedBy,"CreatedBy");
        AddNotifications(contract);
    }

    public void EditInfo(string name, bool active)
    { 
        Name = name;
        Active = active;

        Validate( );
    }
}
