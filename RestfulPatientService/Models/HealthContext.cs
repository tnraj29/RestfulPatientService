using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;

namespace RestfulPatientService.Models
{
    public class HealthContext: DbContext
    {
        public DbSet<Patient> Patients { get;  set; }
        public DbSet<Phone> Phones { get; set; }

        public HealthContext()
        {
            //base.Configuration.ProxyCreationEnabled = false;

        }

        public async Task<Patient> CreatePatient(Patient patient)
        {
            this.Patients.Add(patient);

            try
            {                
                await this.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new DbUpdateConcurrencyException(e.Message);
            }
            return patient;
        }

        public async Task<Patient> GetPatient(int patientID)
        {
            Patient patient = await this.Patients.Where(p => p.PatientID == patientID).Include(p => p.Phones).FirstOrDefaultAsync();

            return patient;
        }

        public async Task UpdatePatient(Patient patient)
        {
            this.Entry<Patient>(patient).State = EntityState.Modified;

            try
            {
                await this.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new DbUpdateConcurrencyException(e.Message);
            }
        }

        public async Task DeletePatient(int patientID)
        {
            Patient patient = await FindPatient(patientID);

            if (patient == null) return;

            this.Patients.Remove(patient);

            try
            {
                await this.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new DbUpdateConcurrencyException(e.Message);
            }
        }

        public async Task<Patient> FindPatient(int patentID)
        {
            return await this.Patients.FindAsync(patentID);
        }
    }    
}