using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[assembly: ContractNamespace("http://PatientDemographics.com/2018/01", ClrNamespace = "RestfulPatientService.Models")]
namespace RestfulPatientService.Models
{
    [DataContract(Namespace = "")]
    public class Patient
    {
        [Column(Order = 0)]
        [DataMember(IsRequired = true, Order = 0)]
        public int PatientID { get; set; }

        [DataMember(IsRequired = true, Order = 1)]
        [Column(Order = 1)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Forename cannot be blank!")]
        [MinLength(3, ErrorMessage = "Forename cannot be less than 3!"), MaxLength(50, ErrorMessage = "ForeName cannot be greater than 50!")]
        public string Forename { get; set; }

        [DataMember(IsRequired = true, Order = 2)]
        [Column(Order = 2)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Surname cannot be blank!")]
        [MinLength(3, ErrorMessage = "Surname cannot be less than 3!"), MaxLength(50, ErrorMessage = "Surname cannot be greater than 50!")]
        public string Surname { get; set; }

        [DataMember(Order = 3)]
        [Column(Order = 3)]
        public DateTime? DateOfBirth { get; set; }

        [DataMember(IsRequired = true, Order = 4)]
        [Column(Order = 4)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Gender cannot be blank!")]
        public string Gender { get; set; }

        // Navigation Property.
        [DataMember(IsRequired = false, Order = 5)]
        //public virtual List<Phone> Phones { get; set; }
        public List<Phone> Phones { get; set; }
    }
}