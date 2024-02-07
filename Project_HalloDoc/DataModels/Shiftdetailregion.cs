﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project_HalloDoc.DataModels;

[Table("shiftdetailregion")]
public partial class Shiftdetailregion
{
    [Key]
    [Column("shiftdetailregionid")]
    public int Shiftdetailregionid { get; set; }

    [Column("shiftdetailid")]
    public int Shiftdetailid { get; set; }

    [Column("regionid")]
    public int Regionid { get; set; }

    [Column("isdeleted", TypeName = "bit(1)")]
    public BitArray? Isdeleted { get; set; }
}
