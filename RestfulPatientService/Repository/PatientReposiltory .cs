using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using RestfulPatientService.Models;

namespace RestfulPatientService.Repository
{
    //This is a Concrete Repository class used for Persisting all "Patient" entity types using the "HealthContext" database type.
    //It inherits all the generic CRUD operations from the base class "GenericRepository"and implements its own Interface.
    public class PatientReposiltory : GenericRepository<Patient, HealthContext>, IPatientRepository
    {
        public PatientReposiltory()
        {

        }
        public Patient GetByID(int patientId)
        {
            Patient patient = null;

            try
            {
                patient = this.GetAll().FirstOrDefault<Patient>(x => x.PatientID == patientId);
            }
            catch (ArgumentNullException e)
            {
                throw new ArgumentNullException(e.Message);
            }
            
            return patient;
        }

        public override IQueryable<Patient> GetAll()
        {
            IQueryable<Patient> patientCollection = this.Context.Set<Patient>().Include("Phones");
            return patientCollection;
        }
    }
}