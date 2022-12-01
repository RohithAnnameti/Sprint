 using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Sprint.Models
{
    [Table("tblPatient")]
    public class Patient
    {
        public static int currId = 0;
        public static int GenerateId(int id)
        {
            currId = id + 1;
            return currId;
        }

        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SerialNo { get; set; }

        [Required]
        [Column(TypeName="nvarchar(250)")]
        public string PatientName { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int Age { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(250)")]
        public string Gender { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(250)")]
        public string Email { get; set; }

        [Column(TypeName = "nvarchar(250)")]
        public string MobileNumber { get; set; }

        [Column(TypeName ="uniqueidentifier")]
        public System.Guid CheckUpCode { get; set; }

        [Column(TypeName = "nvarchar(250)")]
        public double BillAmount { get; set; }

        [Column(TypeName = "nvarchar(250)")]
        public double TotalBillAmount { get; set; }

    }
}
