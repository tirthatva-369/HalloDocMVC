﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project_HalloDoc.DataModels;

[Table("shift")]
public partial class Shift
{
    [Key]
    [Column("shiftid")]
    public int Shiftid { get; set; }

    [Column("physicianid")]
    public int Physicianid { get; set; }

    [Column("startdate")]
    public DateOnly Startdate { get; set; }

    [Column("isrepeat", TypeName = "bit(1)")]
    public BitArray Isrepeat { get; set; } = null!;

    [Column("weekdays")]
    [StringLength(7)]
    public string? Weekdays { get; set; }

    [Column("repeatupto")]
    public int? Repeatupto { get; set; }

    [Column("createdby")]
    [StringLength(128)]
    public string Createdby { get; set; } = null!;

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime Createddate { get; set; }

    [Column("ip")]
    [StringLength(20)]
    public string? Ip { get; set; }
}