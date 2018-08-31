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

namespace RestfulPatientService.Controllers
{
    public class PatientsController : ApiController
    {
        private HealthContext db = new HealthContext();

        // GET: api/Patients
        [HttpGet]
        [Route("api/Patients")]
        public IQueryable<Patient> GetPatients()
        {
            return db.Patients;//.Include("Phones");
        }

        // GET: api/Patients/5
        [HttpGet]
        [Route("api/Patients/{id}", Name = "GetPatient")]
        public async Task<IHttpActionResult> GetPatient(int id)
        {
            Patient patient = await db.FindPatient(id);

            if (patient == null)
            {
                return NotFound();
            }

            patient = await db.GetPatient(id);
            
            return Ok(patient);
        }

        // PUT: api/Patients/5
        [ResponseType(typeof(void))]
        [HttpPut]
        [RouteAttribute("api/Patients/{id}", Name = "UpdatePatient")]
        public async Task<IHttpActionResult> UpdatePatient(int id, Patient patient)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != patient.PatientID)
            {
                return BadRequest();
            }

            try
            {
                await db.UpdatePatient(patient);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Patients
        [HttpPost]
        [Route("api/Patients", Name = "CreatePatient")]
        public async Task<IHttpActionResult> CreatePatient(Patient patient)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await db.CreatePatient(patient);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return CreatedAtRoute<Patient>("GetPatient", new { id = patient.PatientID }, patient);
            
        }

        // DELETE: api/Patients/5
        [ResponseType(typeof(Patient))]
        [HttpDelete]
        [Route("api/Patients/{id}", Name = "DeletePatient")]        
        public async Task<IHttpActionResult> DeletePatient(int id)
        {
            Patient patient = await db.FindPatient(id);

            if (patient == null)
            {
                return NotFound();
            }

            try
            {
                await db.DeletePatient(id);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok(patient);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PatientExists(int id)
        {
            return db.Patients.Count(e => e.PatientID == id) > 0;
        }
    }
}