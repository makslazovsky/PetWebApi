using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Cat
    {
        [Column("CatId")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Cat name is a required field.")]
        [MaxLength(60, ErrorMessage = "Maximum length for the Name is 60 characters.")]
        public string Name { get; set; }

        [ForeignKey(nameof(Employee))]
        public Guid EmployeeId { get; set; }



    }
}

