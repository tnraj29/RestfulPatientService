using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using RestfulPatientService.Models;

namespace RestfulPatientService.Repository
{
    //This Interface defines operations specific to Patient repository types.
    public interface IPatientRepository
    {
        Patient GetByID(int PatientId);
    }
}