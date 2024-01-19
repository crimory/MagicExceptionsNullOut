namespace _03_MagicValidation.DomainModel;

public record CustomerOrder(
    [property: System.ComponentModel.DataAnnotations.Required]
    [property: System.ComponentModel.DataAnnotations.Length(3, 50)]
    string Number,
    [property: System.ComponentModel.DataAnnotations.Required]
    [property: System.ComponentModel.DataAnnotations.MinLength(1)]
    CustomerOrderPosition[] Positions,
    [property: System.ComponentModel.DataAnnotations.Required]
    Customer Customer);

public record CustomerOrderPosition(
    [property: System.ComponentModel.DataAnnotations.Required]
    [property: System.ComponentModel.DataAnnotations.Length(3, 50)]
    string ItemName,
    [property: System.ComponentModel.DataAnnotations.Range(5, 1000)]
    uint Quantity);

public record Customer(
    [property: System.ComponentModel.DataAnnotations.Required]
    [property: System.ComponentModel.DataAnnotations.Length(3, 50)]
    string Name);