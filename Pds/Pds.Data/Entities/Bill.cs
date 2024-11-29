using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pds.Core.Enums;

namespace Pds.Data.Entities;

/// <summary>
/// Represents a bill entity in the system
/// </summary>
public class Bill : EntityBase
{
    /// <summary>
    /// Gets or sets the monetary value of the bill
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Value { get; set; }

    /// <summary>
    /// Status of the bill
    /// </summary>
    [Required]
    public BillStatus Status { get; set; }
        
    /// <summary>
    /// Status of the payment
    /// </summary>
    [Required]
    public PaymentStatus PaymentStatus { get; set; }
        
    /// <summary>
    /// Gets or sets the type of the bill
    /// </summary>
    [Required]
    public BillType Type { get; set; }

    /// <summary>
    /// Gets or sets the payment type used for this bill
    /// </summary>
    public PaymentType? PaymentType { get; set; }

    /// <summary>
    /// Gets or sets additional comments or notes about the bill
    /// </summary>
    public string Comment { get; set; }

    /// <summary>
    /// Gets or sets the contact information
    /// </summary>
    [Column(TypeName = "varchar(300)")]
    public string Contact { get; set; }
    
    /// <summary>
    /// Gets or sets the contact email address
    /// </summary>
    [Column(TypeName = "varchar(100)")]
    public string ContactEmail { get; set; }

    /// <summary>
    /// Gets or sets the name of the contact person
    /// </summary>
    [Column(TypeName = "varchar(300)")]
    public string ContactName { get; set; }

    /// <summary>
    /// Gets or sets the type of contact
    /// </summary>
    public ContactType? ContactType { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the contact is an agent
    /// </summary>
    public bool IsContactAgent { get; set; }
        
    /// <summary>
    /// Gets or sets the contract number associated with the bill
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string ContractNumber { get; set; }

    /// <summary>
    /// Gets or sets the date of the contract
    /// </summary>
    public DateTime? ContractDate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether NDS (VAT) needs to be paid
    /// </summary>
    public bool IsNeedPayNds { get; set; }

    /// <summary>
    /// Gets or sets the date when the bill was paid
    /// </summary>
    public DateTime? PaidAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the associated brand
    /// </summary>
    public Guid BrandId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the associated content
    /// </summary>
    public Guid? ContentId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the associated client
    /// </summary>
    public Guid? ClientId { get; set; }
        
    /// <summary>
    /// Gets or sets the associated brand
    /// </summary>
    public virtual Brand Brand { get; set; }

    /// <summary>
    /// Gets or sets the associated content
    /// </summary>
    public virtual Content Content { get; set; }

    /// <summary>
    /// Gets or sets the associated client
    /// </summary>
    public virtual Client Client { get; set; }
}