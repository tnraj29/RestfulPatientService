using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using RestfulPatientService.Models;
using System.Threading.Tasks;
using System.Net.Http.Formatting;

using RestfulPatientService.Repository;

namespace RestfulPatientService.Controllers
{
    //Defines all actions related to "Patient" entity.
    public class PatientsController : ApiController
    {
        private PatientReposiltory m_Repository;        

        public PatientsController(PatientReposiltory patientRepo)
        {
            m_Repository = patientRepo;
        }

        public PatientsController()
        {
            m_Repository = new PatientReposiltory();
        }

        // GET: api/Patients
        [HttpGet]
        [Route("api/Patients")]
        public HttpResponseMessage GetPatients()
        {
            IQueryable<Patient> patients = m_Repository.GetAll();

            if (patients.CountAsync<Patient>().Equals(0))
            {
                return Request.CreateErrorResponse(HttpStatusCode.Gone, "The Patients collection is Empty!");
            }

            return Request.CreateResponse(HttpStatusCode.OK, patients);
        }

        // GET: api/Patients/5
        [HttpGet]
        [Route("api/Patients/{id}", Name = "GetPatient")]
        public HttpResponseMessage GetPatient(int id)
        {
            Patient patient = null;
            try
            {
                patient = m_Repository.GetByID(id);
                
                if (patient == null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Invalid PatientID");
            }
            catch (ArgumentNullException e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, e);          
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, patient);
        }

        // PUT: api/Patients/5
        [ResponseType(typeof(void))]
        [HttpPut]
        [RouteAttribute("api/Patients/{id}", Name = "UpdatePatient")]
        public async Task<HttpResponseMessage> UpdatePatient(int id, Patient patient)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            if (id != patient.PatientID)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            m_Repository.Edit(patient);

            try
            {
                await m_Repository.SaveAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Conflict, e.Message);
            }
            catch (DbUpdateException e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Conflict, e.Message);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message);
            }

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        // POST: api/Patients
        [HttpPost]
        [Route("api/Patients", Name = "CreatePatient")]
        public async Task<HttpResponseMessage> CreatePatient(Patient patient)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            m_Repository.Add(patient);

            try
            {
                await m_Repository.SaveAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Conflict, e.Message);
            }
            catch (DbUpdateException e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Conflict, e.Message);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message);
            }

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, patient);

            string newpatientURI = Url.Link("GetPatient", new { id = patient.PatientID, controller = "Patients" });
            response.Headers.Location = new Uri(newpatientURI);

            return response;             
        }

        // DELETE: api/Patients/5
        [ResponseType(typeof(Patient))]
        [HttpDelete]
        [Route("api/Patients/{id}", Name = "DeletePatient")]
        public async Task<HttpResponseMessage> DeletePatient(int id)
        {
            Patient patient = null;
            try
            {
                patient = m_Repository.GetByID(id);
           
                if (patient == null)
                {
                    if (patient == null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Invalid PatientID");
                }
            }
            catch (ArgumentNullException e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, e);
            }
            
            m_Repository.Remove(patient);

            try
            {
                await m_Repository.SaveAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Conflict, e.Message);
            }
            catch (DbUpdateException e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Conflict, e.Message);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_Repository.Dispose();
            }
            base.Dispose(disposing);
        }        
    }
}