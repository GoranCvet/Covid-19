﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.IServices;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp
{
    public class DiagnosisEditModel : PageModel
    {
        private readonly IDiagnosisService diagnosisService;
        private readonly IVirusService virusService;
        private readonly IPatientService patientService;

        public DiagnosisEditModel(IDiagnosisService diagnosisService, IVirusService virusService, IPatientService patientService)
        {
            this.diagnosisService = diagnosisService;
            this.virusService = virusService;
            this.patientService = patientService;
        }

        [BindProperty]
        public Diagnosis Diagnosis { get; set; }
        [BindProperty]
        public List<Virus> Viruses { get; set; }
        [BindProperty]
        public Patient Patient { get; set; }
        public IActionResult OnGet(int? id, int patientId)
        {
            if (id.HasValue)
            {
                Diagnosis = diagnosisService.GetDiagnosisById(id.Value);
                Patient = patientService.GetPatientById(patientId);
                
                if (Diagnosis == null)
                {
                    return RedirectToPage("NotFound");
                }
                Viruses = virusService.GetViruses();

                foreach (var item in Viruses)
                {
                    foreach(var vd in Diagnosis.DiagnosisViruses)
                    {
                        if(item.Id == vd.VirusId)
                        {
                            item.IsSelected = true;
                        }
                    }
                }   // Pri edit na diagnoza Virusite koi vekje gi ima pacientot da bida stiklirani

            }
            else
            {
                Diagnosis = new Diagnosis();
                Patient = patientService.GetPatientById(patientId);
                if(Patient.Diagnosis.Any(d => d.Death == true))
                {
                    TempData["Message"] = "Can not Create new Diagnose because Patient is Dead!";
                    return RedirectToPage("DiagnosisList", new { id = Patient.Id });
                }
            }
            Viruses = virusService.GetViruses();
            return Page();
        }
        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {

                foreach (var virus in Viruses)
                {
                    if (virus.IsSelected == true)
                    {
                        if (virus.Name == "Covid-19")
                        {
                            Diagnosis.IsPositive = true;
                        }
                        var diagnosisVirus = new DiagnosisVirus();
                        diagnosisVirus.Virus = virusService.GetVirusById(virus.Id);
                        Diagnosis.DiagnosisViruses.Add(diagnosisVirus);
                    }
                }
                if (Diagnosis.Id == 0)
                {
                    Diagnosis.PatientId = Patient.Id;
                    Diagnosis = diagnosisService.CreateDiagnosis(Diagnosis);
                }
                else
                {
                    Patient = patientService.GetPatientById(Patient.Id);
                    var diagnose = diagnosisService.GetDiagnosisById(Diagnosis.Id);
                    Patient.Diagnosis.Remove(diagnose);
                    Patient.Diagnosis.Add(Diagnosis);
                }
                diagnosisService.Commit();
                return RedirectToPage("./DiagnosisList", new { id = Diagnosis.PatientId });
            }
            return Page();
        }
    }
}