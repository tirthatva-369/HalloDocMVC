﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project_HalloDoc.DataModels;

[Table("request")]
public partial class Request
{
    [Key]
    [Column("requestid")]
    public int Requestid { get; set; }

    [Column("requesttypeid")]
    public int Requesttypeid { get; set; }

    [Column("userid")]
    public int? Userid { get; set; }

    [Column("firstname")]
    [StringLength(100)]
    public string? Firstname { get; set; }

    [Column("lastname")]
    [StringLength(100)]
    public string? Lastname { get; set; }

    [Column("phonenumber")]
    [StringLength(23)]
    public string? Phonenumber { get; set; }

    [Column("email")]
    [StringLength(50)]
    public string? Email { get; set; }

    [Column("status")]
    public short Status { get; set; }

    [Column("physicianid")]
    public int? Physicianid { get; set; }

    [Column("confirmationnumber")]
    [StringLength(20)]
    public string? Confirmationnumber { get; set; }

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime Createddate { get; set; }

    [Column("isdeleted", TypeName = "bit(1)")]
    public BitArray? Isdeleted { get; set; }

    [Column("modifieddate", TypeName = "timestamp without time zone")]
    public DateTime? Modifieddate { get; set; }

    [Column("declinedby")]
    [StringLength(250)]
    public string? Declinedby { get; set; }

    [Column("isurgentemailsent", TypeName = "bit(1)")]
    public BitArray? Isurgentemailsent { get; set; }

    [Column("lastwellnessdate", TypeName = "timestamp without time zone")]
    public DateTime? Lastwellnessdate { get; set; }

    [Column("ismobile", TypeName = "bit(1)")]
    public BitArray? Ismobile { get; set; }

    [Column("calltype")]
    public short? Calltype { get; set; }

    [Column("completedbyphysician", TypeName = "bit(1)")]
    public BitArray? Completedbyphysician { get; set; }

    [Column("lastreservationdate", TypeName = "timestamp without time zone")]
    public DateTime? Lastreservationdate { get; set; }

    [Column("accepteddate", TypeName = "timestamp without time zone")]
    public DateTime? Accepteddate { get; set; }

    [Column("relationname")]
    [StringLength(100)]
    public string? Relationname { get; set; }

    [Column("casenumber")]
    [StringLength(50)]
    public string? Casenumber { get; set; }

    [Column("ip")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [Column("casetag")]
    [StringLength(50)]
    public string? Casetag { get; set; }

    [Column("casetagphysician")]
    [StringLength(50)]
    public string? Casetagphysician { get; set; }

    [Column("patientaccountid")]
    [StringLength(128)]
    public string? Patientaccountid { get; set; }

    [Column("createduserid")]
    public int? Createduserid { get; set; }
}
