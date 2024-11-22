using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using PROG6212POE.Areas.Identity.Data;
using PROG6212POE.Models;
using System.Security.Claims;

namespace PROG6212POE.Controllers
{
    public class ClaimsController : Controller
    {
        //varibales for db and hosting
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ClaimsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            this.webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult AddClaim()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddClaim(Claims claims)
        {
            if (ModelState.IsValid)
            {
                // Save supporting documents if present
                if (claims.SupportingDocuments != null && claims.SupportingDocuments.Any())
                {
                    // Loop through all files and upload each one
                    foreach (var file in claims.SupportingDocuments)
                    {
                        // Generate unique file name
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var uniqueFileName = Path.GetFileName(file.FileName);
                        
                        var uploadedFile = await SingleFileUpload(file, claims.ID,uniqueFileName); // Call SingleFileUpload

                        if (uploadedFile != null)
                        {
                            // Add the file to the claim's Files collection if uploaded successfully
                            claims.Files.Add(uploadedFile);
                        }
                    }
                }

                // Initialize claim status and calculate total amount
                claims.ClaimStatus = "Pending";
                claims.TotalAmount = claims.HoursWorked * claims.RatePerHour;

                // Save the claim to the database
                _context.Claims.Add(claims);
                await _context.SaveChangesAsync();

                // Add any uploaded files to the database
                if (claims.Files != null && claims.Files.Any())
                {
                    await _context.SaveChangesAsync();
                }

                TempData["message"] = $"Your Total Amount is: R{claims.TotalAmount:F2}";
                return RedirectToAction(nameof(ClaimDetails), new { id = claims.ID });
            }

            // Handle validation errors
            var errorMessages = ModelState.Values.SelectMany(v => v.Errors)
                                                 .Select(e => e.ErrorMessage)
                                                 .ToList();
            TempData["errorMessages"] = errorMessages;

            return View(claims);
        }


        public async Task<Files> SingleFileUpload(IFormFile file, int claimId, string uniquename)
        {
            if (file == null || file.Length == 0)
            {
                return null; // No file selected
            }

            // Validate file extension
            var permittedExtensions = new[] { ".jpg", ".png", ".gif", ".pdf", ".docx", ".xlsx" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(extension) || !permittedExtensions.Contains(extension))
            {
                return null; // Invalid file extension
            }

            // Validate MIME type
            var mimeType = file.ContentType;
            var permittedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif", "application/pdf", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" };
            if (!permittedMimeTypes.Contains(mimeType))
            {
                return null; // Invalid MIME type
            }

            // Ensure the uploads directory exists
            var uploadsFolderPath = Path.Combine(webHostEnvironment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath); // Create the directory if it doesn't exist
            }

            // Generate unique file name
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolderPath, fileName);
            

            
            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await file.CopyToAsync(stream);
            }

            
            var fileModel = new Files
            {
                fileName = fileName,
                UniqueFileName = uniquename,
                Length = file.Length,
                ContentType = mimeType,
                Data = ConvertToByteArray(filePath),
                ClaimId = claimId 
            };
            _context.SaveChanges();

            return fileModel;
        }

        // Method to handle file download -- part 3
        public IActionResult DownloadFile(int id)
        {
            var file = _context.Files.FirstOrDefault(f => f.fileId == id);

            if (file == null)
            {
                return NotFound();
            }

            return File(file.Data, file.ContentType, file.fileName);
        }


        // This method will convert the uploaded file into a byte array --part 3
        private byte[] ConvertToByteArray(string filePath)
        {
            byte[] fileData;

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    fileData = reader.ReadBytes((int)fs.Length);
                }
            }

            return fileData;
        }

        [Authorize(Roles = "Programme Coordinator,Academic Manager")]
        public async Task<IActionResult> ClaimsList()
        {
            var claims = await _context.Claims
                .Include(c => c.Files) 
                .ToListAsync();

            return View(claims);
        }

        public IActionResult ClaimDetails(int id)
        {
            var claim = _context.Claims
                                .Include(c => c.Files)  
                                .FirstOrDefault(c => c.ID == id);

            if (claim == null)
            {
                return NotFound();
            }

            ViewData["Title"] = "Claim Details";
            return View(claim);
        }

        //approve method
        public IActionResult ApproveClaim(int id)
        {
            var claim = _context.Claims.Find(id);
            if (claim == null)
            {
                return RedirectToAction(nameof(ApproveClaim));
            }
            claim.ClaimStatus = "Approved";
            _context.SaveChanges();
            //toastr build for approved claim
            TempData["Approved"] = $"The claim for {claim.FirstName} {claim.LastName} has been approved.";

            return RedirectToAction("HRView");
        }

        //Reject method
        public IActionResult RejectClaim(int id)
        {
            var claim = _context.Claims.Find(id);
            if (claim == null)
            {
                return RedirectToAction(nameof(RejectClaim));
            }
            claim.ClaimStatus = "Rejected";
            _context.SaveChanges();
            //toastr build for rejected claim
            TempData["Rejected"] = $"The claim for {claim.FirstName} {claim.LastName} has been rejected.";

            return RedirectToAction("HRView");
        }


        //ability  to edit claims
        [HttpGet]
        public IActionResult EditClaims(int id)
        {
            var claim = _context.Claims.Include(c => c.Files).FirstOrDefault(c => c.ID == id);

            if (claim == null)
            {
                return NotFound();
            }

            ViewData["Title"] = "Edit Claim";
            return View(claim);  
        }
        [HttpPost]
        public async Task<IActionResult> EditClaims(int claimId, Claims updatedClaim, IFormFile newFile)
        {

            // Retrieve the existing claim
            var existingClaim = await _context.Claims.Include(c => c.Files)
                                                      .FirstOrDefaultAsync(c => c.ID == claimId);

            if (existingClaim == null)
            {
                Console.WriteLine("Claim not found in the database.");
                return NotFound(); // Handle case where claim is not found
            }

            // file deletion
            if (existingClaim.Files != null && existingClaim.Files.Any())
            {
                var oldFile = existingClaim.Files.First();
                if (oldFile != null)
                {
                    Console.WriteLine($"Deleting old file with ID: {oldFile.fileId}");
                    await DeleteFile(oldFile.fileId);
                }
            }

            // file upload
            if (newFile != null && newFile.Length > 0)
            {
                Console.WriteLine($"Uploading new file: {newFile.FileName}");
                var newfileName = Guid.NewGuid().ToString() + Path.GetExtension(newFile.FileName);
                var newUniqueFileName = Path.GetFileName(newfileName);
                updatedClaim.SupDocName = newUniqueFileName;
                var fileModel = await SingleFileUpload(newFile, claimId, newUniqueFileName);
                if (fileModel != null)
                {
                    existingClaim.Files = new List<Files> { fileModel };
                }
            }

            // Updating claim properties
            existingClaim.DescriptionOfWork = updatedClaim.DescriptionOfWork;
            existingClaim.TotalAmount = updatedClaim.HoursWorked * updatedClaim.RatePerHour;
            existingClaim.ClaimStatus = updatedClaim.ClaimStatus;

            // save changes
            _context.Attach(existingClaim).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            Console.WriteLine("Claim successfully updated.");
            return RedirectToAction(nameof(ClaimDetails), new { id = claimId });
        }

        //delete file method
        public async Task<IActionResult> DeleteFile(int id)
        {
            // Find the file in the database by the file ID
            var file = _context.Files.FirstOrDefault(f => f.fileId == id);

            if (file == null)
            {
                return NotFound(); // Return a 404 error if the file is not found
            }

            
            var filePath = Path.Combine(webHostEnvironment.WebRootPath, "uploads", file.UniqueFileName);

            // Delete the file from the file system if it exists
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // Remove the file entry from the database
            _context.Files.Remove(file);
            _context.SaveChanges();

            // success message
            TempData["message"] = "File deleted successfully.";
            return RedirectToAction("ClaimsList");
        }

        // GET: /Claims/YourClaims
        [HttpGet]
        public IActionResult YourClaims(string lecturerId)
        {
            if (string.IsNullOrEmpty(lecturerId))
            {
                return View(new List<Claims>()); 
            }

           
            var claims = _context.Claims
                                 .Where(c => c.LecturerID == lecturerId) 
                                 .ToList();

            return View(claims); 
        }


        [HttpPost]
        public async Task<IActionResult> DeleteClaims(int claimId)
        {
            // Retrieve the claim with its associated file
            var claim = await _context.Claims.Include(c => c.Files)
                                             .FirstOrDefaultAsync(c => c.ID == claimId);

            // Check if the claim exists
            if (claim == null)
            {
                return NotFound(); 
            }

            // Call the DeleteFile method for each associated file
            if (claim.Files != null && claim.Files.Any())
            {
                var fileToDelete = claim.Files.First(); 
                if (fileToDelete != null)
                {
                    // Call the DeleteFile method to delete the associated file
                    var deleteFileResult = await DeleteFile(fileToDelete.fileId);
                    if (deleteFileResult is NotFoundResult) // If the file was not found or couldn't be deleted
                    {
                        return NotFound(); 
                    }
                }
            }

            // Remove the claim from the database
            _context.Claims.Remove(claim);

            // Save changes to the database
            await _context.SaveChangesAsync();

            

            // Check the user's role and redirect 
            if (User.IsInRole("Programme Coordinator") || User.IsInRole("Academic Manager"))
            {
                TempData["message"] = "Claim and associated file deleted successfully.";
                return RedirectToAction("ClaimsList");
            }
            else
            {
                TempData["message"] = "Claim Submission was canceled.";
                return RedirectToAction("YourClaims");
            }
        }

        [Authorize(Roles = "Programme Coordinator,Academic Manager")]
        public async Task<IActionResult> HRView()
        {
            var claims = await _context.Claims
                .Include(c => c.Files) 
                .ToListAsync();

            return View(claims);
        }
    }
}
