using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DataModels;

[Table("encounterform")]
public partial class Encounterform
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("requestid")]
    public int Requestid { get; set; }

    [Column("firstname")]
    [StringLength(100)]
    public string Firstname { get; set; } = null!;

    [Column("lastname")]
    [StringLength(100)]
    public string? Lastname { get; set; }

    [Column("location")]
    [StringLength(200)]
    public string? Location { get; set; }

    [Column("strmonth")]
    [StringLength(20)]
    public string? Strmonth { get; set; }

    [Column("intyear")]
    public int? Intyear { get; set; }

    [Column("intdate")]
    public int? Intdate { get; set; }

    [Column("servicedate", TypeName = "timestamp without time zone")]
    public DateTime? Servicedate { get; set; }

    [Column("phonenumber")]
    [StringLength(50)]
    public string? Phonenumber { get; set; }

    [Column("email")]
    [StringLength(50)]
    public string? Email { get; set; }

    [Column("illnesshistory")]
    [StringLength(500)]
    public string? Illnesshistory { get; set; }

    [Column("medicalhistory")]
    [StringLength(500)]
    public string? Medicalhistory { get; set; }

    [Column("medications")]
    [StringLength(500)]
    public string? Medications { get; set; }

    [Column("allergies")]
    [StringLength(500)]
    public string? Allergies { get; set; }

    [Column("temperature")]
    [Precision(10, 2)]
    public decimal? Temperature { get; set; }

    [Column("heartrate")]
    [Precision(10, 2)]
    public decimal? Heartrate { get; set; }

    [Column("respirationrate")]
    [Precision(10, 2)]
    public decimal? Respirationrate { get; set; }

    [Column("bloodpressuresystolic")]
    public int? Bloodpressuresystolic { get; set; }

    [Column("bloodpressurediastolic")]
    public int? Bloodpressurediastolic { get; set; }

    [Column("oxygenlevel")]
    [Precision(10, 2)]
    public decimal? Oxygenlevel { get; set; }

    [Column("pain")]
    [StringLength(50)]
    public string? Pain { get; set; }

    [Column("heent")]
    [StringLength(500)]
    public string? Heent { get; set; }

    [Column("cardiovascular")]
    [StringLength(500)]
    public string? Cardiovascular { get; set; }

    [Column("chest")]
    [StringLength(500)]
    public string? Chest { get; set; }

    [Column("abdomen")]
    [StringLength(500)]
    public string? Abdomen { get; set; }

    [Column("extremities")]
    [StringLength(500)]
    public string? Extremities { get; set; }

    [Column("skin")]
    [StringLength(500)]
    public string? Skin { get; set; }

    [Column("neuro")]
    [StringLength(500)]
    public string? Neuro { get; set; }

    [Column("other")]
    [StringLength(500)]
    public string? Other { get; set; }

    [Column("diagnosis", TypeName = "character varying")]
    public string? Diagnosis { get; set; }

    [Column("treatmentplan", TypeName = "character varying")]
    public string? Treatmentplan { get; set; }

    [Column("medicationsdispensed", TypeName = "character varying")]
    public string? Medicationsdispensed { get; set; }

    [Column("procedures", TypeName = "character varying")]
    public string? Procedures { get; set; }

    [Column("followup", TypeName = "character varying")]
    public string? Followup { get; set; }

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime Createddate { get; set; }

    [Column("modifieddate", TypeName = "timestamp without time zone")]
    public DateTime? Modifieddate { get; set; }

    [Column("isfinalized")]
    public bool? Isfinalized { get; set; }

    [Column("finalizeddate", TypeName = "timestamp without time zone")]
    public DateTime? Finalizeddate { get; set; }

    [ForeignKey("Requestid")]
    [InverseProperty("Encounterforms")]
    public virtual Request Request { get; set; } = null!;
}
