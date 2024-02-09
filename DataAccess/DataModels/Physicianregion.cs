﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project_HalloDoc.DataModels;

[Table("physicianregion")]
public partial class Physicianregion
{
    [Key]
    [Column("physicianregionid")]
    public int Physicianregionid { get; set; }

    [Column("physicianid")]
    public int Physicianid { get; set; }

    [Column("regionid")]
    public int Regionid { get; set; }
}
