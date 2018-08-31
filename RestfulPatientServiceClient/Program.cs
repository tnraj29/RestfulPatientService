using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using Newtonsoft.Json;
using System.Data.Entity;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

[assembly: ContractNamespace("http://PatientDemographics.com/2018/01", ClrNamespace = "RestfulPatientServiceClient")]
namespace RestfulPatientServiceClient
{
    public class HealthContext : DbContext
    {
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Phone> Phones { get; set; }
    }

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
        public virtual List<Phone> Phones { get; set; }
    }

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
        public virtual Patient Patient { get; set; }
    }


    class Program
    {
        static HttpClient client = new HttpClient();

        static string Serialize<T>(MediaTypeFormatter formatter, T value)
        {
            // Create a dummy HTTP Content.
            Stream stream = new MemoryStream();
            var content = new StreamContent(stream);
            /// Serialize the object.
            formatter.WriteToStreamAsync(typeof(T), value, stream, content, null).Wait();
            // Read the serialized string.
            stream.Position = 0;
            return content.ReadAsStringAsync().Result;
        }

        static T Deserialize<T>(MediaTypeFormatter formatter, string str) where T : class
        {
            // Write the serialized string to a memory stream.
            Stream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            // Deserialize to an object of type T
            return formatter.ReadFromStreamAsync(typeof(T), stream, null, null).Result as T;
        }

        static async Task<Uri> CreatePatientAsync(Patient patient)
        {
            // XML Payload.
            HttpResponseMessage response = await client.PostAsXmlAsync("api/Patients", patient);

            // Json Payload.
            //HttpResponseMessage response = await client.PostAsJsonAsync("api/Patients", patient);

            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Headers.Location;
        }

        static async Task<Patient> GetPatientAsync(string path)
        {
            Patient patient = null;
            HttpResponseMessage response = await client.GetAsync(path);

            if (response.IsSuccessStatusCode)
            {
                patient = await response.Content.ReadAsAsync<Patient>();
            }

            return patient;
        }

        static async Task<Patient> UpdatePatientAsync(Patient patient)
        {
            // Json Payload.
            //HttpResponseMessage response = await client.PutAsJsonAsync($"api/Patients/{patient.PatientID}", patient);

            // Xml Payload.
            HttpResponseMessage response = await client.PutAsXmlAsync($"api/Patients/{patient.PatientID}", patient);

            response.EnsureSuccessStatusCode();

            // Deserialize the updated patient from the response body.
            patient = await response.Content.ReadAsAsync<Patient>();
            return patient;
        }

        static async Task<HttpStatusCode> DeleteProductAsync(int patientId)
        {
            HttpResponseMessage response = await client.DeleteAsync($"api/Patients/{patientId}");

            return response.StatusCode;
        }

        static void ShowPatient(Patient patient)
        {
            string patientHeader = $"Forename" + $"\tSurname" + $"\t\tGender" + $"\t\tPhoneNumber" + $"\tPhoneType";
            Console.WriteLine(patientHeader);

            string patientData = patient.Forename + $"\t\t" + patient.Surname + $"\t\t" + patient.Gender;
            string patientContact = string.Empty;

            foreach (var phone in patient.Phones)
            {
                Console.WriteLine(patientData + $"\t\t" + phone.PhoneNumber + $"\t\t" + phone.PhoneType);
            }
        }

        static async Task<Patient> GetAllPatientAsync()
        {
            Patient patient = null;
            HttpResponseMessage response = await client.GetAsync("api/allpatients");            

            if (response.IsSuccessStatusCode)
            {
                patient = await response.Content.ReadAsAsync<Patient>();
            }

            return patient;
        }

        static void InitPatientService()
        {
            // Update port # in the following line.
            client.BaseAddress = new Uri("http://localhost:51126/");

            //Send Json-Payload.
            //client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

            //Send XML-Payload.
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/xml");

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

            //var p = GetAllPatientAsync().Result;
        }

        static Patient GetPatientTestData()
        {
            Patient patient = new Patient
            {
                Forename = "Rama",
                Surname = "Krishna",
                Gender = "Male",
                DateOfBirth = new DateTime(1971, 03, 29),
                Phones = new List<Phone>()
                    {
                        new Phone() { PhoneNumber = "222", PhoneType = "Office" },
                        new Phone() { PhoneNumber = "333", PhoneType ="Home" },
                        new Phone() { PhoneNumber = "444", PhoneType = "Mobile" }
                    }
            };

            return patient;
        }

        static async Task<Uri> TestCreatePatient(Patient patient)
        {
            //Patient patient = GetPatientTestData();

            var url = await CreatePatientAsync(patient);
            Console.WriteLine($"Created Resource Patient at {url}");

            return url;
        }

        static async Task<Patient> TestGetPatient(string path)
        {
            // Get the patient.
            Patient patient = await GetPatientAsync(path);

            return patient;
        }

        static async Task TestUpdatePatient(Patient patient)
        {
            await UpdatePatientAsync(patient);
        }

        static async Task<HttpStatusCode> TestDeletePatient(int patientID)
        {
            var statuscode = await DeleteProductAsync(patientID);
            return statuscode;
        }

        static string TestXMLSerialization(Patient p)
        {
            // This creates an XML Serializer.
            var xml = new XmlMediaTypeFormatter();

            // This creates a Data Contract Serializer.
            //XmlMediaTypeFormatter xmlDatacontract = new XmlMediaTypeFormatter();
            //xmlDatacontract.SetSerializer<Patient>(new DataContractSerializer(typeof(Patient), new Type[] { typeof(Phone) }));

            string str = Serialize(xml, p);

            return str;
        }

        static Patient TestXMLDeSerialization(string str)
        {
            var xml = new XmlMediaTypeFormatter();
            Patient p = Deserialize<Patient>(xml, str);

            return p;
        }

        static async Task RunAsync()
        {
            try
            {
                Patient testPatient = GetPatientTestData();

                // Create a Patient resource.
                Console.WriteLine("Creating Patient...");
                var Url = await TestCreatePatient(testPatient);

                // Get the Patient.
                testPatient = await TestGetPatient(Url.PathAndQuery);
                ShowPatient(testPatient);

                // Update the Patient.
                Console.WriteLine("Updating Patient Surname...");
                testPatient.Surname = "Test";
                await TestUpdatePatient(testPatient);

                // Get the updated Patient.
                testPatient = await TestGetPatient(Url.PathAndQuery);
                ShowPatient(testPatient);

                // Delete the Patient.
                Console.WriteLine("Deleting Patient...");
                var statusCode = await TestDeletePatient(testPatient.PatientID);
                Console.WriteLine($"Deleted Patient (HTTP Status = {(int) statusCode})");
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }

        static void TestHealthContext()
        {
            using (var db = new HealthContext())
            {
                Patient patient = GetPatientTestData();               

                db.Patients.Add(patient);
                db.SaveChanges();

                patient = db.Patients.Include("Phones").FirstOrDefault(); 

                //var allPatPhones = db.Patients.Include("Phones").ToList();

                ShowPatient(patient);

                //foreach (var patient in allPatPhones)
                //{
                //    Console.WriteLine(patient.Forename);

                //    foreach (var phone in patient.Phones)
                //    {
                //        Console.WriteLine(phone.PhoneNumber);
                //        Console.WriteLine(phone.PhoneType);
                //    }
                //}                
            }
        }
        static void Main(string[] args)
        {
            InitPatientService();

            RunAsync().GetAwaiter().GetResult();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();           
        }
    }
}
