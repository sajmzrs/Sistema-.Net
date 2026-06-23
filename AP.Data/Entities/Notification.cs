using AP.Data.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace AP.Data
{
    public partial class Notification : IEntity
    {
        [NotMapped]
        public string UniqueIdentifier { get; set; }
    }
}
