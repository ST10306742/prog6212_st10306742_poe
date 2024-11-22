using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using PROG6212POE.Controllers;
using System.Security.Claims;
using PROG6212POE.Areas.Identity.Data;
using PROG6212POE.Controllers;
using PROG6212POE.Migrations;
using PROG6212POE.Models;
using System.IO.Abstractions.TestingHelpers;

namespace TaskOneDemoTest
{
    public class ClaimsControllerTests
    {
        /*
         * Fact -- sets a method as a unit test
         * AAA --- > ARRANGE , ACT , ASSERT
         */

        //PRE Setup
        private readonly ClaimsController _controller;
        private readonly ApplicationDbContext _context;
        private readonly Mock<IWebHostEnvironment> _mockEnvironment;

        public ClaimsControllerTests()
        {
            //setup the inmemory(package) db
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName: "TaskOneDemo")
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            //mock the ef enviroment
            _mockEnvironment = new Mock<IWebHostEnvironment>();
            _mockEnvironment.Setup(env => env.WebRootPath).Returns("C:\\fakepath");

            //init - the controller with the real context
            _controller = new ClaimsController(_context, _mockEnvironment.Object);

            //setup TempData
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _controller.TempData = tempData;
        }

        //setup the first unit test
        [Fact] // wont pick up as a unit test without it
        public async Task ClaimsPost_ValidModel_RedirectToClaimDetails()
        {
            //AAA --- > ARRANGE , ACT , ASSERT

            //arrange
            var claims = new Claims
            {
                //id to ensure that pk is not matched
                ID = 999,
                LecturerID = "123",
                FirstName = "John",
                LastName = "Smith",
                ClaimsPeriodStart = DateTime.Now.AddDays(-7),
                ClaimsPeriodEnd = DateTime.Now,
                HoursWorked = 5,
                RatePerHour = 100,
                DescriptionOfWork = "testing work descp",
                SupportingDocuments = new List<IFormFile>(),
                ClaimStatus = "APPROVED"
            };

            //act 
            var result = await _controller.AddClaim(claims);

            //assertions
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ClaimDetails", redirectResult.ActionName);
            //CHECKS THAT 1 VALUE GOT ADDED TO THE DB
            Assert.Equal(1, _context.Claims.Count());

        }

        //invalid claims
        [Fact]
        public async Task ClaimsPost_InvalidModel_ReturnsView()
        {
            //arrange 
            var claims = new Claims
            {
                LecturerID = "",
                FirstName = "",
                LastName = "",
                ClaimsPeriodStart = DateTime.Now.AddDays(-7),
                ClaimsPeriodEnd = DateTime.Now,
                HoursWorked = 10,
                RatePerHour = 50,
                DescriptionOfWork = "testing",
                SupportingDocuments = new List<IFormFile>(),
                ClaimStatus = ""
            };

            _controller.TempData["message"] = "Default message"; // Set a default value

            //act - pull the model - error to make the model invalid 
            _controller.ModelState.AddModelError("FirstName", "Required");
            var result = await _controller.AddClaim(claims);


            //assert

            var ViewResult = Assert.IsType<ViewResult>(result);
            //ensures that model is returned
            Assert.IsType<Claims>(ViewResult.Model);
            Assert.False(_controller.ModelState.IsValid);
        }
        [Fact]
        public void Approve_ClaimFound_RedirectToClaimDetails()
        {
            //arrange
            var claims = new Claims
            {
                LecturerID = "200",
                FirstName = "Test",
                LastName = "Testing",
                ClaimsPeriodStart = DateTime.Now.AddDays(-7),
                ClaimsPeriodEnd = DateTime.Now,
                HoursWorked = 10,
                RatePerHour = 50,
                DescriptionOfWork = "Valid Claim",
                SupportingDocuments = new List<IFormFile>(),
                ClaimStatus = "Pending",
                SupDocName="",
                SupDocPath=""
            };
            _context.Claims.Add(claims);
            _context.SaveChanges();

            //act 
            var result = _controller.ApproveClaim(claims.ID);

            //assert
            var updatedClaims = _context.Claims.Find(claims.ID);//Fect updated claims
            var redirectedResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("HRView", redirectedResult.ActionName); // ensure status change  persisted
            Assert.Equal("Approved", updatedClaims.ClaimStatus);

        }
        [Fact]
        public void Reject_ClaimFound_RedirectToClaimDetails()
        {
            //arrange
            var claims = new Claims
            {
                LecturerID = "200",
                FirstName = "Test",
                LastName = "Testing",
                ClaimsPeriodStart = DateTime.Now.AddDays(-7),
                ClaimsPeriodEnd = DateTime.Now,
                HoursWorked = 10,
                RatePerHour = 50,
                DescriptionOfWork = "Valid Claim",
                SupportingDocuments = new List<IFormFile>(),
                ClaimStatus = "Pending",
                SupDocName = "",
                SupDocPath = ""
            };
            _context.Claims.Add(claims);
            _context.SaveChanges();

            //act 
            var result = _controller.RejectClaim(claims.ID);

            //assert
            var updatedClaims = _context.Claims.Find(claims.ID);//Fect updated claims
            var redirectedResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("HRView", redirectedResult.ActionName); // ensure status change  persisted
            Assert.Equal("Rejected", updatedClaims.ClaimStatus);

        }
        [Fact]
        public async Task DeleteFile_FileExistsInDatabase_DeletesFileAndRedirects()
        {
            // Arrange
            var fileId = 1;
            var file = new Files
            {
                fileId = fileId,
                UniqueFileName = "testfile.txt",
                ContentType = "text/plain", // Set ContentType
                Data = new byte[] { 1, 2, 3 }, // Mock some data
                fileName = "testfile.txt" // Set the required fileName property
            };

            // Add the file to the in-memory database
            _context.Files.Add(file);
            _context.SaveChanges();

            // Set up the mock environment to return a fake file path
            var filePath = Path.Combine("C:\\fakepath", "uploads", file.UniqueFileName);
            _mockEnvironment.Setup(env => env.WebRootPath).Returns("C:\\fakepath");

            // Mock the file system to ensure the file exists
            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddFile(filePath, new MockFileData("File content"));

            // Act
            var result = await _controller.DeleteFile(fileId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ClaimsList", redirectResult.ActionName); // Ensure redirection to ClaimsList
            Assert.Equal(0, _context.Files.Count()); // Ensure the file is deleted from the database
        }


        [Fact]
        public async Task DeleteFile_FileNotFoundInDatabase_ReturnsNotFound()
        {
            // Arrange
            var fileId = 999; // Non-existent file ID

            // Act
            var result = await _controller.DeleteFile(fileId);

            // Assert
            Assert.IsType<NotFoundResult>(result); // Should return NotFound if file is not found in the DB
        }

        [Fact]
        public async Task DeleteFile_FileExistsInDatabaseButNotOnDisk_ReturnsRedirectWithoutDeletingFile()
        {
            // Arrange
            var fileId = 1;
            var file = new Files
            {
                fileId = fileId,
                UniqueFileName = "testfile.txt",
                ContentType = "text/plain", // Set ContentType
                Data = new byte[] { 1, 2, 3 }, // Mock some data
                fileName = "testfile.txt" // Set the required fileName property
            };

            // Add the file to the in-memory database
            _context.Files.Add(file);
            _context.SaveChanges();

            // Set up the mock environment to return a fake file path
            var filePath = Path.Combine("C:\\fakepath", "uploads", file.UniqueFileName);
            _mockEnvironment.Setup(env => env.WebRootPath).Returns("C:\\fakepath");

            // Ensure the file does not exist on disk
            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddFile(filePath, new MockFileData("File content"));
            mockFileSystem.RemoveFile(filePath);  // Simulate file not being on disk

            // Act
            var result = await _controller.DeleteFile(fileId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ClaimsList", redirectResult.ActionName); // Ensure redirection to ClaimsList
            Assert.Equal(0, _context.Files.Count()); // Ensure the file is not deleted from the database (file does not exist on disk)
        }

    }
}