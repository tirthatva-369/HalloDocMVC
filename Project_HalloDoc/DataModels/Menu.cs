using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project_HalloDoc.DataModels;

[Table("menu")]
public partial class Menu
{
    [Key]
    [Column("menuid")]
    public int Menuid { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Column("accounttype")]
    public short Accounttype { get; set; }

    [Column("sortorder")]
    public int? Sortorder { get; set; }
}
