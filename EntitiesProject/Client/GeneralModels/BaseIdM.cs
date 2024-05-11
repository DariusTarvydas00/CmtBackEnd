using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntitiesProject.Client.GeneralModels;

public class BaseIdM
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Guid { get; set; } = Guid.NewGuid();
    public int MsgId { get; set; }
}