    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using Sprint.Models;
    using Azure.Storage.Blobs;

    namespace Sprint.Controllers
    {
        public class PatientsController : Controller
        {
            static string conStr = "DefaultEndpointsProtocol=https;AccountName=straccnameaw;AccountKey=zg3r2W8fcdUp3GDYg7v4SLfYBzcjJibYQd0H/uyR5h0uWSAO2D0TCXAMmcmeA/S0bdpunJIeKLSlp6/5pbNO3w==;EndpointSuffix=core.windows.net";
            static BlobServiceClient serviceClient = new BlobServiceClient(conStr);
            static BlobContainerClient containerClient = serviceClient.GetBlobContainerClient("sample1");

            // GET: Patients
            public ActionResult Index()
            {
            List<Patient> patientlist = new List<Patient>();
                for(int i=1; i<= Patient.currId;i++){
                    var fileName = i + ".txt";
                string existingContent = GetContentFromBlob(fileName);
                var patient = Newtonsoft.Json.JsonConvert.DeserializeObject<Patient>(existingContent);
                patientlist.Add(patient);
            }
                return View(patientlist);
            }

            // GET: Patients/Details/5
            public ActionResult Details(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var fileName = id + ".txt";
                string existingContent = GetContentFromBlob(fileName);
                var patient = Newtonsoft.Json.JsonConvert.DeserializeObject<Patient>(existingContent);

                if (patient == null)
                {
                    return NotFound();
                }

                return View(patient);
            }

            // GET: Patients/Create
            public IActionResult Create()
            {
                return View();
            }

            // POST: Patients/Create
            // To protect from overposting attacks, enable the specific properties you want to bind to, for 
            // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Create([Bind("SerialNo,PatientName,Age,Gender,Email,MobileNumber,BillAmount")] Patient patient)
            {
                patient.SerialNo = Patient.GenerateId(Patient.currId);
            if (patient.Age > 65) { 
                    patient.CheckUpCode = Guid.NewGuid();
                    patient.TotalBillAmount = patient.BillAmount / 2.0;
                }
                else
                {
                    patient.TotalBillAmount = patient.BillAmount;
                }
            
                try
                {
                string patientStr = Newtonsoft.Json.JsonConvert.SerializeObject(patient);

                    try
                    {
                        UploadBlob(patientStr, patient.SerialNo);
                        ViewBag.MessageToScreent = "Details Updated to Blob :" + patientStr;
                    }
                    catch (Exception ex)
                    {
                        ViewBag.MessageToScreent = "Failed to update blob " + ex.Message;
                    }
                    return View("Details", patient);
                }
                catch
                {
                    return View(patient);
                }
            }


        
        public static string UploadBlob(string fileContent, int SerialNo)
            {
                string result = "Success";
                try
                {
                    var fileName = SerialNo + ".txt";
                    var blobClient = containerClient.GetBlobClient(fileName);

                    var ms = new MemoryStream();
                    TextWriter tw = new StreamWriter(ms);
                    tw.Write(fileContent);
                    tw.Flush();
                    ms.Position = 0;

                    blobClient.UploadAsync(ms, true);

                }
                catch (Exception ex)
                {
                    result = "Failed";
                    throw ex;
                }
                return result;
            }

            private static string GetContentFromBlob(string fileName)
            {
                BlobClient blobClient = containerClient.GetBlobClient(fileName);
                string line = string.Empty;
                if (blobClient.Exists())
                {
                    var response = blobClient.Download();
                    using (var streamReader = new StreamReader(response.Value.Content))
                    {
                        while (!streamReader.EndOfStream)
                        {
                            line += streamReader.ReadLine() + Environment.NewLine;
                        }
                    }
                }
                return line;
            }

            // GET: Patients/Edit/5
            public ActionResult Edit(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }
                var fileName = id + ".txt";
                string existingContent = GetContentFromBlob(fileName);
                var patient = Newtonsoft.Json.JsonConvert.DeserializeObject<Patient>(existingContent); 

                //var patient = await _context.tblPatient.FindAsync(id);
                if (patient == null)
                {
                    return NotFound();
                }
                return View(patient);
            }


        // POST: Patients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Edit(int id, [Bind("SerialNo,PatientName,Age,Gender,Email,MobileNumber,BillAmount")] Patient patient)
            {
            patient.SerialNo = id;

            try
            {
                string patientStr = Newtonsoft.Json.JsonConvert.SerializeObject(patient);

                try
                {
                    UploadBlob(patientStr, patient.SerialNo);
                    ViewBag.MessageToScreent = "Details Updated to Blob :" + patientStr;
                }
                catch (Exception ex)
                {
                    ViewBag.MessageToScreent = "Failed to update blob " + ex.Message;
                }
                return View("Details",patient);
            }
            catch
            {
                return View(patient);
            }
        }

        // GET: Patients/GetDetails
        public IActionResult GetDetails()
        {
            return View();
        }

        // POST: Patients/GetDetails
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetDetails([Bind("SerialNo")] Patient p)
        {
            int id = p.SerialNo;

            var fileName = id + ".txt";
            string existingContent = GetContentFromBlob(fileName);
            var patient = Newtonsoft.Json.JsonConvert.DeserializeObject<Patient>(existingContent);

            if (patient == null)
            {
                return NotFound();
            }

            return View("Details", patient);
        }

        // GET: Patients/EditDetails
        public IActionResult EditDetails()
        {
            return View();
        }

        // POST: Patients/EditDetails
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDetails([Bind("SerialNo")] Patient p)
        {
            int id = p.SerialNo;

            var fileName = id + ".txt";
            string existingContent = GetContentFromBlob(fileName);
            var patient = Newtonsoft.Json.JsonConvert.DeserializeObject<Patient>(existingContent);

            if (patient == null)
            {
                return NotFound();
            }

            return View("Edit", patient);
        }

        // GET: Patients/Delete/5
        public ActionResult Delete(int? id)
            {
                return View();
            }

            // POST: Patients/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public ActionResult DeleteConfirmed(int id)
            {
                return RedirectToAction(nameof(Index));
            }

        }
    }
