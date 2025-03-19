using System.Security.Claims;
using ABK_People_BackEnd.Data;
using ABK_People_BackEnd.DTOs;
using ABK_People_BackEnd.DTOs.ResponseDTOs;
using ABK_People_BackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ABK_People_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]


    public class RequestsController : ControllerBase
    {
        #region Services
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostenv;

        public RequestsController(ApplicationDbContext context, IWebHostEnvironment hostenv)
        {
            _context = context;
            _hostenv = hostenv;
        }
        #endregion

        #region Employee-Requests
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
                VacationRequestStatus = VacationRequest.Status.Ongoing,
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
                ComplaintRequestStatus = ComplaintRequest.Status.Ongoing,
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
                
            }
            _context.Requests.Add(complaintRequest);
            await _context.SaveChangesAsync();

            return Ok(new { RequestId = complaintRequest.RequestId });
        }

        [Authorize(Roles= "Employee")]
        [HttpPost("add-message")]
        public async Task<IActionResult> AddMessageToRequest([FromForm] AddMessageDTO messageDTO)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Invalid token: User ID not found.");
            }

            var request = await _context.Requests
                .Include(r => r.Messages)
                .FirstOrDefaultAsync(r => r.RequestId == messageDTO.RequestId);

            if (request == null)
            {
                return NotFound("Request not found.");
            }

            if (request.EmployeeId != userId)
            {
                return Forbid("You can only add messages to your own requests.");
            }

            // Check if adding a message is allowed based on status
            if (request is VacationRequest vacationRequest &&
                vacationRequest.VacationRequestStatus != VacationRequest.Status.RequestingDocuments)
            {
                return BadRequest("Cannot add message unless more information is requested.");
            }
            else if (request is ComplaintRequest complaintRequest &&
                complaintRequest.ComplaintRequestStatus != ComplaintRequest.Status.ReturedForResponse)
            {
                return BadRequest("Cannot add message unless response is requested.");
            }

            var message = new Message
            {
                DescriptionBody = messageDTO.DescriptionBody,
                CreatedAt = DateTime.UtcNow,
                UserId = userId
            };

            if (messageDTO.Files?.Any() == true)
            {
                message.Files = await SaveFiles(messageDTO.Files, message);
            }

            request.Messages ??= new List<Message>();
            request.Messages.Add(message);

            // Update request status to Ongoing
            if (request is VacationRequest vr)
            {
                vr.VacationRequestStatus = VacationRequest.Status.Ongoing;
            }
            else if (request is ComplaintRequest cr)
            {
                cr.ComplaintRequestStatus = ComplaintRequest.Status.Ongoing;
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [Authorize(Roles= "Employee")]
        [HttpGet("my-requests")]
        public async Task<IActionResult> GetMyRequests()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Invalid token: User ID not found.");
            }

            var requests = await _context.Requests
                .Include(r => r.Messages)
                    .ThenInclude(m => m.Files)
                .Where(r => r.EmployeeId == userId)
                .ToListAsync();

            var requestDTOs = requests.Select(r => new RequestResponseDTO
            {
                RequestId = r.RequestId,
                IsClicked = r.IsClicked,
                TypeOfRequest = r.TypeOfRequest,
                CreatedAt = r.CreatedAt,
                EmployeeId = r.EmployeeId,
                StartDate = r is VacationRequest vr ? vr.StartDate : null,
                EndDate = r is VacationRequest vr2 ? vr2.EndDate : null,
                RequestStatus = r is VacationRequest vr3 ? (VacationRequest.Status?)vr3.VacationRequestStatus : null,
                TypeOfVacation = r is VacationRequest vr4 ? vr4.TypeOfVacation : null,
                ComplaintStatus = r is ComplaintRequest cr ? (ComplaintRequest.Status?)cr.ComplaintRequestStatus : null,
                TypeOfComplaint = r is ComplaintRequest cr2 ? cr2.TypeOfComplaint : null,
                Messages = r.Messages?.Select(m => new MessageResponseDTO
                {
                    MessageId = m.MessageId,
                    CreatedAt = m.CreatedAt,
                    DescriptionBody = m.DescriptionBody,
                    UserId = m.UserId,
                    RequestId = m.RequestId,
                    Files = m.Files?.Select(f => new MessageFileResponseDTO
                    {
                        MessageFileId = f.MessageFileId,
                        FileName = f.FileName,
                        FilePath = f.FilePath,
                        FileType = f.FileType,
                        MessageId = f.MessageId
                    }).ToList() ?? new List<MessageFileResponseDTO>()
                }).ToList() ?? new List<MessageResponseDTO>()
            }).ToList();

            return Ok(requestDTOs);
        }

        #endregion

        #region Admin-Requests

        [Authorize(Roles = "Admin")]
        [HttpPost("respond")]
        public async Task<IActionResult> RespondToRequest([FromForm] AdminResponseDTO responseDTO)
        {
            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminId))
            {
                return Unauthorized("Invalid token: Admin ID not found.");
            }

            var request = await _context.Requests
                .Include(r => r.Messages)
                .FirstOrDefaultAsync(r => r.RequestId == responseDTO.RequestId);

            if (request == null)
            {
                return NotFound("Request not found.");
            }

            // Update status based on request type
            if (request is VacationRequest vacationRequest)
            {
                if (!Enum.TryParse<VacationRequest.Status>(responseDTO.Status, true, out var status))
                {
                    return BadRequest("Invalid status for vacation request.");
                }
                vacationRequest.VacationRequestStatus = status;
            }
            else if (request is ComplaintRequest complaintRequest)
            {
                if (!Enum.TryParse<ComplaintRequest.Status>(responseDTO.Status, true, out var status))
                {
                    return BadRequest("Invalid status for complaint request.");
                }
                complaintRequest.ComplaintRequestStatus = status;
            }
            else
            {
                return BadRequest("Unknown request type.");
            }

            if (responseDTO.Message != null && (!string.IsNullOrEmpty(responseDTO.Message.DescriptionBody) || responseDTO.Message.Files?.Any() == true))
            {
                var message = new Message
                {
                    DescriptionBody = responseDTO.Message.DescriptionBody,
                    CreatedAt = DateTime.UtcNow,
                    UserId = adminId
                };

                request.Messages ??= new List<Message>();
                request.Messages.Add(message);

                if (responseDTO.Message.Files?.Any() == true)
                {
                    message.Files = await SaveFiles(responseDTO.Message.Files, message);
                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("all-requests")]
        public async Task<IActionResult> GetAllRequests()
        {
            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminId))
            {
                return Unauthorized("Invalid token: Admin ID not found.");
            }

            var requests = await _context.Requests
                .Include(r => r.Messages)
                    .ThenInclude(m => m.Files)
                .AsNoTracking() // No tracking since we’re only reading
                .ToListAsync();

            var requestDTOs = requests.Select(r => new RequestResponseDTO
            {
                RequestId = r.RequestId,
                IsClicked = r.IsClicked,
                TypeOfRequest = r.TypeOfRequest,
                CreatedAt = r.CreatedAt,
                EmployeeId = r.EmployeeId,
                StartDate = r is VacationRequest vr ? vr.StartDate : null,
                EndDate = r is VacationRequest vr2 ? vr2.EndDate : null,
                RequestStatus = r is VacationRequest vr3 ? (VacationRequest.Status?)vr3.VacationRequestStatus : null, // Use the helper property
                TypeOfVacation = r is VacationRequest vr4 ? vr4.TypeOfVacation : null,
                ComplaintStatus = r is ComplaintRequest cr ? (ComplaintRequest.Status?)cr.ComplaintRequestStatus : null, // Use the helper property
                TypeOfComplaint = r is ComplaintRequest cr2 ? cr2.TypeOfComplaint : null,
                Messages = r.Messages?.Select(m => new MessageResponseDTO
                {
                    MessageId = m.MessageId,
                    CreatedAt = m.CreatedAt,
                    DescriptionBody = m.DescriptionBody,
                    UserId = m.UserId,
                    RequestId = m.RequestId,
                    Files = m.Files?.Select(f => new MessageFileResponseDTO
                    {
                        MessageFileId = f.MessageFileId,
                        FileName = f.FileName,
                        FilePath = f.FilePath,
                        FileType = f.FileType,
                        MessageId = f.MessageId
                    }).ToList() ?? new List<MessageFileResponseDTO>()
                }).ToList() ?? new List<MessageResponseDTO>()
            }).ToList();

            return Ok(requestDTOs);
        }

        #endregion

        #region Helper-Functions
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

        #endregion
    }
}
