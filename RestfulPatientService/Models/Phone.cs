using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[assembly: ContractNamespace("http://PatientDemographics.com/2018/01", ClrNamespace = "RestfulPatientService.Models")]
namespace RestfulPatientService.Models
{
    //Defines data Entity "Phone", related to the "Patient" entity, 
    //with all the data and Serialization contraints.
    [DataContract(Namespace = "")]
    public class Phone
    {
        [Column(Order = 0)]
        [DataMember(Order = 0)]
        public int PhoneID { get; set; }

        [DataMember(Order = 1)]
        [Column(Order = 1)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Phone Number cannot be blank!")]
        public string PhoneNumber { get; set; }

        [DataMember(Order = 2)]
        [Column(Order = 2)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Phone Type cannot be blank!")]
        public string PhoneType { get; set; }

        [DataMember]
        public int PatientID { get; set; }

        [ForeignKey("PatientID")]
        //public virtual Patient Patient { get; set; }
        public Patient Patient { get; set; }
    }
}