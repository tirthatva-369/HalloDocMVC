﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project_HalloDoc.DataModels;

[Keyless]
[Table("physicianlocation")]
public partial class Physicianlocation
{
    [Column("locationid")]
    public int Locationid { get; set; }

    [Column("physicianid")]
    public int Physicianid { get; set; }

    [Column("latitude")]
    [Precision(9, 0)]
    public decimal? Latitude { get; set; }

    [Column("longitude")]
    [Precision(9, 0)]
    public decimal? Longitude { get; set; }

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime? Createddate { get; set; }

    [Column("physicianname")]
    [StringLength(50)]
    public string? Physicianname { get; set; }

    [Column("address")]
    [StringLength(500)]
    public string? Address { get; set; }
}