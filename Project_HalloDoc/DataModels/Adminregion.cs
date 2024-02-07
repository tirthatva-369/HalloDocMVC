using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project_HalloDoc.DataModels;

[Table("adminregion")]
public partial class Adminregion
{
    [Key]
    [Column("adminregionid")]
    public int Adminregionid { get; set; }

    [Column("adminid")]
    public int Adminid { get; set; }

    [Column("regionid")]
    public int Regionid { get; set; }
}
