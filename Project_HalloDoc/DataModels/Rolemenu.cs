﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project_HalloDoc.DataModels;

[Table("rolemenu")]
public partial class Rolemenu
{
    [Key]
    [Column("rolemenuid")]
    public int Rolemenuid { get; set; }

    [Column("roleid")]
    public int Roleid { get; set; }

    [Column("menuid")]
    public int Menuid { get; set; }
}