Programming6212 - POE  --> Claims Management Contract System MVC application.
README File
------------------------------------------------------------------------------------------------------------------------------------------
Introduction to the Claims Management Contract System MVC application:
A feature-rich web-based program called the Claims Management Contract System (CMCS) was created to simplify the administration and handling of contract claims for educational establishments. The MVC architecture in.NET was used in the construction of CMCS, which offers payroll teams, administrators, and lecturers an easy-to-use platform for managing claims. 

The brief overview of the application :
This  Claims Management Contract System MVC application was designed to manage claims submitted by lecturers in the best possible way, which includes date tracking, uploading of supporting documents and auto-calculation of the total payment necessary by multiplying the H=amount of hours worked by the rate the company pays per hour. Another view is specifically for overseeing claims. This is only for the two administrators of the system, which are the Programme Coordinator and the Academic Manager. 
Since there are 2 separate roles of the system (1 Normal User: Lecturers / 2 Administrator: Programme Coordinator & Academic Manager), a login system will be utilised to aid security and confidentiality in the CMCS. Only administrators can approve or reject claims after viewing the details of each claim from the lecturers. A Lecturer can view all their claims by searching with their lecturer id. An HR view has now been implemented with he sole purpose of Approving and Rejecting claims, while the claims list view are for other functions, like deleting claims. 
------------------------------------------------------------------------------------------------------------------------------------------
Changes made in Final POE:
-An HR view has been made with the only purpose of Approving and Rejecting claims
-The ClaimsList View now allows Admins to Delete claims
-A lecturer can now search for their claims submitted by searching with their Lecturer ID and a table of their claims.
-A lecturer can now edit their claims.
-Admins/HR Staff can download and view supporting Documents from lecturers when approving/rejecting claims.
-3 more unit tests regarding deleting claims have been made, making 7 tests pass successfully.
------------------------------------------------------------------------------------------------------------------------------------------
Automation Changes:
*An HR view has been made with the only purpose of Approving and Rejecting claims
*The ClaimsList View now allows Admins to Delete claims
*A lecturer can now search for their claims submitted by searching with their Lecturer ID and a table of their claims.
*A lecturer can now edit their claims.
------------------------------------------------------------------------------------------------------------------------------------------
Requirements:
You will require a 2019 version 17.5.0 of Visual Studio Community or later and .NET Core 3.2 or later to run this application.
Min. 4GB RAM, 8GB RAM recommended. 
20GB+ hard disk storage space.
Solid State Drive.
------------------------------------------------------------------------------------------------------------------------------------------
Frameworks & plugins used:
Proj file:

<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="6.0.35" />
    <PackageReference Include="Microsoft.AspNetCore.Identity.UI" Version="6.0.35" />
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="6.0.35" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="6.0.35">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="6.0.35" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="6.0.35">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.VisualStudio.Web.CodeGeneration.Design" Version="6.0.18" />
    <PackageReference Include="toastr" Version="2.1.1" />
  </ItemGroup>

</Project>

app json:
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "ApplicationDbContextConnection": "Server=(localdb)\\mssqllocaldb;Database=PROG6212POE;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
------------------------------------------------------------------------------------------------------------------------------------------
How to apply:
1.Clone the repository from GitHub.
2.Open Visual Studio and launch the application.
3. Create and launch the application.
------------------------------------------------------------------------------------------------------------------------------------------
Functionality
The application does the following tasks:
1. The user logs in as either an admin or a lecturer.
	-> If an lecturer were to attempt to log in, they would first have to create an account by clicking 'Register' on the top-right corner of the screen.
	-> To register an account, click 'Register' on the top-right corner of the screen and enter an email and password which will be used for authentication when a user attempts to log in.
	-> To login, click 'Login' on the top-right corner of the screen. Enter your username and password of your account and press the login button or the Enter key on your keyboard.

2. User access
	-> A non-logged in user can only access the Home view and the Privacy View.

3. Lecturer access
	-> If a lecturer successfully logs in, they would be able to access the submit claim view, which allows a user to enter their respective details to submit a claim. NOTE: a claim will not be submitted 	until all required information have passed validation. Toastr notfications will be provided to guide user for successful claim submission. The user will have to provide the following data:
		# Lecturer Id (integer)
		# First Name (string)
		# Last Name (string)
		# Claims Period Start (date)
		# Claims Period End (date)
		# Hours worked (int)
		# Rate per Hour (double/decimal)
		# Supporting Document (file)
	-> Once a user clicks 'Submit', an auto-calculation will be done to determine the total payment required for the claim, and the file name and path will be retrieved. Once all details 	have been 	   	   validated and accurate, the data will be saved to the database. The claim will be officially be submitted and will be awaited to be overseen by the admins.
	-> Lecturers can now view all their claims. By searching their LecturerID, they can view all claims belonging to them in a table form.
	-> Lectures have the ability to edit their claims after searching by clicking the edit button and editing all information in a different view. Afterwards, pthey will press save to update the information in the database.
4. Admin access
	-> There are only two admins of the system: Programme Coordinator & Academic Manager. These roles have been back-end coded to the database to ensure that they are the only admins of the system. They 	have their specific email and password in order to login to the system. Once logged in, they will have access to the claims list view, which displays a table with all claims 	submitted by 	lecturers. They have three buttons on the right end of each column: Approve, Reject and View.
	   	*Approve: Once clicked on, it updates the claim status to approved for further actions to be done. A toastr notification will be provided if a claim is approved.
	   	*Reject: Once clicked on, it updates the claim status to rejected so that no further actions are necessary.A toastr notification will be provided if a claim is rejected.
		*View: Opens a view of all the details of the specific lecturer's claim. The user then has the option to approve or reject the claim, which will then be taken back to the claim list view or 				return straight back to the claim list view.

	->Admins now have the ability to delete claims by pressing the delete button when viewing a claims details.
------------------------------------------------------------------------------------------------------------------------------------------
Default user login details:
Admins:
Academic Manager:
email : acm@admin.com
password : Admin987$acm

Programme Coordinator:
email : pgc@admin.com
password : Admin123@pgc

Lecturer example:
email : tester@test.com
password Tetser@123
------------------------------------------------------------------------------------------------------------------------------------------
Frequently asked Questions (Faq's):
1. What is CMCS?
The Claims Management Contract System (CMCS) is an online tool created to assist educational establishments in handling contract claims that instructors make. It makes submitting, reviewing, approving, and tracking payments for claims easier.

2. For whom is CMCS appropriate?
Three user roles are intended for CMCS:

Lecturers: You may claim the hours you put in.
Claims can be reviewed, approved, or rejected by program coordinators.
Academic managers can supervise claims and system data and are endowed with administrative rights.

3. How do instructors submit their claims?
Lecturers access the system, log in, complete a form detailing their hourly rate and the number of hours worked, and submit the claim for approval.

4. Can claims be changed after they are submitted?
Editing a claim is allowed prior to approval or rejection. A claim cannot be changed once it has been processed.

5. How is the approval procedure carried out?
A claim is sent to the program coordinator for consideration when it is submitted by a professor. The claim may be accepted or denied by the coordinator. The claim is sent on for payment processing if it is accepted.

6. How do people find out how their claims are progressing?
Toastr messages allow users to receive real-time notifications when their claims are accepted or denied. These alerts show up right away following a transaction.

------------------------------------------------------------------------------------------------------------------------------------------
Credits
A Mahabeer, student at Varsity College with student Number ST10306742 email st10306742@vcconnect.edu.za, was the author of this application.
------------------------------------------------------------------------------------------------------------------------------------------
GitHub Link:
https://github.com/VCWVL/prog6212-poe-ST10306742.git
------------------------------------------------------------------------------------------------------------------------------------------
Acknlodgements/References:
Bro code: https://www.youtube.com/watch?v=HGTJBPNC-Gw
Bootswatch: https://bootswatch.com/minty/
Toastr by Code Seven: https://codeseven.github.io/toastr/
