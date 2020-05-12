using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

    public class EnrollStudentRequest
    {
        public string IndexNumber { get; set; }


        [Required(ErrorMessage = "Nie podano imienia")]
        [MaxLength(10)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(255)]
        public string LastName { get; set; }
        public DateTime Birthdate { get; set; }

        [Required]
        public string Studies { get; set; }

    }

