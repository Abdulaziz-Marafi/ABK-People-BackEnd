using System.Security.Claims;
using ABK_People_BackEnd.Data;
using ABK_People_BackEnd.DTOs;
using ABK_People_BackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ABK_People_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]


    public class RequestsController : ControllerBase
    {
        // Services
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostenv;

        public RequestsController(ApplicationDbContext context, IWebHostEnvironment hostenv)
        {
            _context = context;
            _hostenv = hostenv;
        }

        [Authorize(Roles = "Employee")]
        [HttpPost("vacation")]
        public async Task<IActionResult> CreateVacationRequest([FromForm] CreateVacationDTO vacationDTO)
        {
            // Extract the user ID from the token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Invalid token: User ID not found.");
            }

            var vacationRequest = new VacationRequest
            {
                StartDate = vacationDTO.StartDate,
                EndDate = vacationDTO.EndDate,
                TypeOfVacation = vacationDTO.TypeOfVacation,
                RequestStatus = VacationRequest.Status.Ongoing,
                CreatedAt = DateTime.UtcNow,
                EmployeeId = userId,
                //TODO: It was not letting me access RequestType directly, so I had to use the subclas, not sure why.
                TypeOfRequest = VacationRequest.RequestType.Vacation
            };

            // Initialize Messages collection if there’s a message or files
            if (vacationDTO.Message != null || (vacationDTO.Message?.Files.Any() == true))
            {
                vacationRequest.Messages = new List<Message>(); // Initialize collection

                var message = new Message
                {
                    DescriptionBody = vacationDTO.Message.DescriptionBody,
                    CreatedAt = DateTime.UtcNow,
                    UserId = userId
                    // let EF handle requestId it via the relationship
                };

                vacationRequest.Messages.Add(message); 

                if (vacationDTO.Message.Files.Any())
                {
                    message.Files = await SaveFiles(vacationDTO.Message.Files, message);
                }
            }
            _context.Requests.Add(vacationRequest);
            await _context.SaveChangesAsync();

            return Ok(new { RequestId = vacationRequest.RequestId });
        }

        [Authorize(Roles = "Employee")]
        [HttpPost("complaint")]
        public async Task<IActionResult> CreateComplaintRequest([FromForm] CreateComplaintDTO complaintDTO)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Invalid token: User ID not found.");
            }
            var complaintRequest = new ComplaintRequest
            {
                TypeOfComplaint = complaintDTO.TypeOfComplaint,
                RequestStatus = ComplaintRequest.Status.Ongoing,
                CreatedAt = DateTime.UtcNow,
                EmployeeId = userId,
                TypeOfRequest = ComplaintRequest.RequestType.Complaint
            };
            if (complaintDTO.Message != null && (!string.IsNullOrEmpty(complaintDTO.Message.DescriptionBody) || complaintDTO.Message.Files?.Any() == true))
            {
                complaintRequest.Messages = new List<Message>();
                var message = new Message
                {
                    DescriptionBody = complaintDTO.Message.DescriptionBody,
                    CreatedAt = DateTime.UtcNow,
                    UserId = userId
                };

                complaintRequest.Messages.Add(message);

                if (complaintDTO.Message.Files?.Any() == true)
                {
                    message.Files = await SaveFiles(complaintDTO.Message.Files, message);
                }
                _context.Requests.Add(complaintRequest);
                await _context.SaveChangesAsync();

                return Ok(new { RequestId = complaintRequest.RequestId });
            }
        }


        // Helper Functions

        // Helper Function used to save files to the server and return messagefiles object
        private async Task<List<MessageFile>> SaveFiles(List<IFormFile> files, Message message)
        {
            List<MessageFile> messageFiles = new List<MessageFile>();
            var filesFolder = Path.Combine(_hostenv.ContentRootPath, "Files");

            // Create Files Folder if it does not exist
            if (!Directory.Exists(filesFolder))
            {
                Directory.CreateDirectory(filesFolder);
            }
            foreach (var file in files)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(filesFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                messageFiles.Add(new MessageFile
                {
                    FileName = fileName,
                    FilePath = filePath,
                    FileType = file.ContentType,
                    //MessageId = message.MessageId Let EF handle it via the relationship
                });
            }
            message.Files = messageFiles;
            return messageFiles;
        }

    }
}
