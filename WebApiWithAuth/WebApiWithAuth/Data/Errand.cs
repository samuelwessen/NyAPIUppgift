using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebApiWithAuth.Data;

namespace WebApi.Data
{
    public class Errand
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName ="nvarchar(50)")]
        public string CustomerName { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [Column(TypeName ="datetime")]
        public DateTime Created { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? Changed { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(20)")]
        public string Status { get; set; }

        public virtual User User { get; set; }
    }
}
