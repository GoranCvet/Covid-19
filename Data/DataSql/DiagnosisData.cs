﻿using Data.IServices;
using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data.DataSql
{
    public class DiagnosisData : IDiagnosisService
    {
        private readonly AppDbContext dbContext;

        public DiagnosisData(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public int Commit()
        {
            return dbContext.SaveChanges();
        }

        public Diagnosis CreateDiagnosis(Diagnosis diagnosis)
        {
            dbContext.Diagnoses.Add(diagnosis);
            return diagnosis;
        }

        public IEnumerable<Diagnosis> GetDiagnoses()
        {
            return dbContext.Diagnoses
                .ToList();
        }

        public Diagnosis GetDiagnosisById(int id)
        {
            return dbContext.Diagnoses
                .SingleOrDefault(x => x.Id == id);
        }

        public Diagnosis UpdateDiagnosis(Diagnosis diagnosis)
        {
            dbContext.Entry(diagnosis).State = EntityState.Modified;
            return diagnosis;
        }
    }
}
