using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagementApiReverb.Entities.Entities;

public class AuditLog
{
    public Guid Id { get; set; }
    [ForeignKey("User")]
    [Column(TypeName = "char(36)")]
    public Guid? UserId { get; set; }
    public string TableName { get; set; }
    public string Action { get; set; }
    
    [Column(TypeName = "longtext")]
    public string? OldValues { get; set; }
    [Column(TypeName = "longtext")]
    public string? NewValues { get; set; }
    public DateTime CreatedAt { get; set; }
    public User? User { get; set; }
    
}