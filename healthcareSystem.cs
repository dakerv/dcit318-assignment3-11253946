using System;
using System.Collections.Generic;
using System.Linq;

namespace DCIT318_Q2
{
    // Generic Repository<T>
    public class Repository<T>
    {
        private List<T> items = new();

        public void Add(T item)
        {
            items.Add(item);
        }

        public List<T> GetAll()
        {
            return new List<T>(items);
        }

        public T? GetById(Func<T, bool> predicate)
        {
            return items.FirstOrDefault(predicate);
        }

        public bool Remove(Func<T, bool> predicate)
        {
            var item = items.FirstOrDefault(predicate);
            if (item == null) return false;
            items.Remove(item);
            return true;
        }
    }

    // Patient class
    public class Patient
    {
        public int Id { get; }
        public string Name { get; }
        public int Age { get; }
        public string Gender { get; }

        public Patient(int id, string name, int age, string gender)
        {
            Id = id;
            Name = name;
            Age = age;
            Gender = gender;
        }
    }

    // Prescription class
    public class Prescription
    {
        public int Id { get; }
        public int PatientId { get; }
        public string MedicationName { get; }
        public DateTime DateIssued { get; }

        public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
        {
            Id = id;
            PatientId = patientId;
            MedicationName = medicationName;
            DateIssued = dateIssued;
        }
    }

    // HealthSystemApp
    public class HealthSystemApp
    {
        public Repository<Patient> _patientRepo = new();
        public Repository<Prescription> _prescriptionRepo = new();
        public Dictionary<int, List<Prescription>> _prescriptionMap = new();

        public void SeedData()
        {
            // Add 2-3 Patient objects
            _patientRepo.Add(new Patient(1, "Alice Smith", 30, "Female"));
            _patientRepo.Add(new Patient(2, "Bob Johnson", 45, "Male"));
            _patientRepo.Add(new Patient(3, "Clara Oswald", 28, "Female"));

            // Add 4-5 Prescription objects with valid PatientIds
            _prescriptionRepo.Add(new Prescription(1, 1, "Amoxicillin", DateTime.Now.AddDays(-10)));
            _prescriptionRepo.Add(new Prescription(2, 1, "Ibuprofen", DateTime.Now.AddDays(-5)));
            _prescriptionRepo.Add(new Prescription(3, 2, "Lisinopril", DateTime.Now.AddDays(-20)));
            _prescriptionRepo.Add(new Prescription(4, 3, "Metformin", DateTime.Now.AddDays(-2)));
            _prescriptionRepo.Add(new Prescription(5, 2, "Atorvastatin", DateTime.Now.AddDays(-1)));
        }

        public void BuildPrescriptionMap()
        {
            _prescriptionMap.Clear();
            var allPrescriptions = _prescriptionRepo.GetAll();
            foreach (var p in allPrescriptions)
            {
                if (!_prescriptionMap.ContainsKey(p.PatientId))
                    _prescriptionMap[p.PatientId] = new List<Prescription>();

                _prescriptionMap[p.PatientId].Add(p);
            }
        }

        public List<Prescription> GetPrescriptionsByPatientId(int patientId)
        {
            if (_prescriptionMap.TryGetValue(patientId, out var list))
                return new List<Prescription>(list);
            return new List<Prescription>();
        }

        public void PrintAllPatients()
        {
            var patients = _patientRepo.GetAll();
            foreach (var p in patients)
            {
                Console.WriteLine($"ID: {p.Id}, Name: {p.Name}, Age: {p.Age}, Gender: {p.Gender}");
            }
        }

        public void PrintPrescriptionsForPatient(int id)
        {
            var prescriptions = GetPrescriptionsByPatientId(id);
            foreach (var pr in prescriptions)
            {
                Console.WriteLine($"Prescription ID: {pr.Id}, Medication: {pr.MedicationName}, DateIssued: {pr.DateIssued}");
            }
        }
    }

    class Program
    {
        static void Main()
        {
            var app = new HealthSystemApp();
            app.SeedData();
            app.BuildPrescriptionMap();
            app.PrintAllPatients();

            // Select one PatientId and display prescriptions
            int selectedPatientId = 2; // per instructions: select one ID
            app.PrintPrescriptionsForPatient(selectedPatientId);
        }
    }
}
