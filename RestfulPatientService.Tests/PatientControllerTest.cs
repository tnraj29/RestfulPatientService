using System;
using System.Web;
using System.Net;

using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;

using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RestfulPatientService.Models;
using RestfulPatientService.Repository;
using RestfulPatientService.Controllers;

namespace RestfulPatientService.Tests
{
    //Unit Tests for the Patient Controller.
    [TestClass]
    public class PatientControllerTest
    {
        private PatientReposiltory m_patientRepo;
        private Patient m_patient;
        private PatientsController m_patientCtrl;

        #region Units Tests

        public PatientControllerTest()
        {
            m_patientRepo = new PatientReposiltory();
            m_patient = new Patient();
            m_patientCtrl = new PatientsController(m_patientRepo);
        }

        [TestInitialize]
        public void InitPatentTest()
        {
            InitPatientTestData();
        }

        [TestMethod]
        public async Task GetReturnsAllPatients()
        {
            //Arrange.
            m_patientCtrl.Request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Patients");
            m_patientCtrl.Configuration = new HttpConfiguration();

            //Act.
            HttpResponseMessage responseAllpatients = m_patientCtrl.GetPatients();
            List<Patient> patients = await responseAllpatients.Content.ReadAsAsync<List<Patient>>();

            //Assert.
            Assert.IsTrue(responseAllpatients.StatusCode.Equals(HttpStatusCode.OK));
            Assert.IsTrue(patients.Count > 0);
        }

        [TestMethod]
        public async Task GetReturnsPatientID()
        {
            //Arrange.
            m_patientCtrl.Request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Patients");
            m_patientCtrl.Configuration = new HttpConfiguration();

            //Act.
            HttpResponseMessage response = m_patientCtrl.GetPatient(1);
            Patient patient = await response.Content.ReadAsAsync<Patient>();

            //Assert.
            Assert.IsTrue(response.StatusCode.Equals(HttpStatusCode.OK));
            Assert.AreEqual(1, patient.PatientID);
        }

        [TestMethod]
        public async Task GetReturnsNotFoundException()
        {
            //Arrange.
            m_patientCtrl.Request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Patients");
            m_patientCtrl.Configuration = new HttpConfiguration();

            //Act.
            HttpResponseMessage response = m_patientCtrl.GetPatient(0);

            //Assert.
            Patient patient = await response.Content.ReadAsAsync<Patient>();
            Assert.IsTrue(response.StatusCode.Equals(HttpStatusCode.NotFound));
            Assert.IsNull(patient);
        }

        [TestMethod]
        public async Task PostReturnsPatient()
        {
            // Arrange.
            var config = new HttpConfiguration();
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/api/Patients");
            var route = config.Routes.MapHttpRoute("GetPatient", "api/{controller}/{id}");
            var routeData = new HttpRouteData(route, new HttpRouteValueDictionary(new { controller = "Patients" }));

            var controller = new PatientsController
            {
                ControllerContext = new HttpControllerContext(config, routeData, request),
                Request = request,
                Url = new UrlHelper(request)
            };
            controller.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;
            controller.Request.Properties[HttpPropertyKeys.HttpRouteDataKey] = routeData;

            //Act.
            HttpResponseMessage response = await controller.CreatePatient(m_patient);

            //Assert.
            Patient patient = await response.Content.ReadAsAsync<Patient>();
            Assert.IsTrue(response.StatusCode.Equals(HttpStatusCode.Created));
        }

        [TestMethod]
        public async Task PutUpdatesPatient()
        {
            //Arrange.
            m_patientCtrl.Request = new HttpRequestMessage(HttpMethod.Put, "http://localhost/api/Patients");
            m_patientCtrl.Configuration = new HttpConfiguration();

            // Get an existing Patient entity to update.
            HttpResponseMessage responseGetPatient = m_patientCtrl.GetPatient(3);
            Patient patient = await responseGetPatient.Content.ReadAsAsync<Patient>();
            patient.Surname = "Test";

            //Act.
            HttpResponseMessage responseUpdatePatient = await m_patientCtrl.UpdatePatient(patient.PatientID, patient);

            //Assert.
            Assert.IsTrue(responseUpdatePatient.StatusCode.Equals(HttpStatusCode.NoContent));
        }

        [TestMethod]
        public async Task DeletePatient()
        {
            //Arrange.
            m_patientCtrl.Request = new HttpRequestMessage(HttpMethod.Delete, "http://localhost/api/Patients");
            m_patientCtrl.Configuration = new HttpConfiguration();

            //Get an existing Patient entity to delete.
            HttpResponseMessage responseGetPatient = m_patientCtrl.GetPatient(3);

            Patient patient = await responseGetPatient.Content.ReadAsAsync<Patient>();

            //Act.
            HttpResponseMessage responseDeletePatient = await m_patientCtrl.DeletePatient(patient.PatientID);

            //Assert.
            Assert.IsTrue(responseDeletePatient.StatusCode.Equals(HttpStatusCode.OK));

            //Confirm deletion.
            HttpResponseMessage responseDeletedPatient = m_patientCtrl.GetPatient(patient.PatientID);
            Assert.IsTrue(responseDeletedPatient.StatusCode.Equals(HttpStatusCode.NotFound));
        }

        #endregion

        #region Unit Test Helper Methods.

        public void InitPatientTestData()
        {
            m_patient = new Patient
            {
                Forename = "Rajendra",
                Surname = "Tadimari",
                Gender = "Male",
                DateOfBirth = new DateTime(1971, 03, 29),
                Phones = new List<Phone>()
                    {
                        new Phone() { PhoneNumber = "777", PhoneType = "Office" },
                        new Phone() { PhoneNumber = "888", PhoneType ="Home" },
                        new Phone() { PhoneNumber = "999", PhoneType = "Mobile" }
                    }
            };
        }

        #endregion
    }
}
