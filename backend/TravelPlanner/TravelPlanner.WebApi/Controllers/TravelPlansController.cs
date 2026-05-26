using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using TravelPlanner.Common.Interfaces;
using TravelPlanner.WebApi.DTOs.TravelPlans;
using TravelPlanner.WebApi.Extensions;
using TravelPlanner.WebApi.Mappings;
using TravelPlanner.WebApi.Reports;

namespace TravelPlanner.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/travel-plans")]
    public class TravelPlansController : ControllerBase
    {
        private static IPlanService PlanService =>
            ServiceProxy.Create<IPlanService>(
                new Uri("fabric:/TravelPlanner/TravelPlanner.PlanService"),
                new ServicePartitionKey(0));

        private static ISharingService SharingService =>
            ServiceProxy.Create<ISharingService>(new Uri("fabric:/TravelPlanner/TravelPlanner.SharingService"));

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var ownerId = User.GetUserId();
            var plans = await PlanService.GetAllPlansByOwnerAsync(ownerId);

            return Ok(plans.Select(plan => plan.ToTravelPlanResponse()));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var ownerId = User.GetUserId();
            var plan = await PlanService.GetPlanByIdAsync(id);

            if (plan is null)
                return NotFound();

            if (plan.OwnerId != ownerId && !User.IsAdminUser())
                return Forbid();

            return Ok(plan.ToTravelPlanResponse());
        }

        [HttpGet("{id:guid}/reports/summary")]
        public async Task<IActionResult> DownloadSummaryReport(Guid id)
        {
            var ownerId = User.GetUserId();
            var plan = await PlanService.GetPlanByIdAsync(id);

            if (plan is null)
                return NotFound();

            if (plan.OwnerId != ownerId && !User.IsAdminUser())
                return Forbid();

            var destinations = await PlanService.GetDestinationsAsync(id);
            var activities = await PlanService.GetActivitiesAsync(id);
            var expenses = await PlanService.GetExpensesAsync(id);
            var checklistItems = await PlanService.GetChecklistItemsAsync(id);
            var reminders = await PlanService.GetRemindersAsync(id);

            var pdfBytes = TravelPlanPdfBuilder.Build(plan, destinations, activities, expenses, checklistItems, reminders);
            var fileName = $"{SanitizeFileName(plan.Title)}-izvjestaj.pdf";

            return File(pdfBytes, "application/pdf", fileName);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTravelPlanDto dto)
        {
            if (dto.EndDate < dto.StartDate)
                return BadRequest("Krajnji datum ne može biti prije početnog datuma.");

            if (dto.PlannedBudget < 0)
                return BadRequest("Budžet ne može biti negativan.");

            var ownerId = User.GetUserId();
            var result = await PlanService.CreatePlanAsync(
                ownerId,
                dto.Title,
                dto.Description,
                dto.StartDate,
                dto.EndDate,
                dto.PlannedBudget,
                dto.Notes);

            if (!result.Success)
                return BadRequest(result.Error);

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data.ToTravelPlanResponse());
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateTravelPlanDto dto)
        {
            if (dto.EndDate < dto.StartDate)
                return BadRequest("Krajnji datum ne može biti prije početnog datuma.");

            if (dto.PlannedBudget < 0)
                return BadRequest("Budžet ne može biti negativan.");

            var ownerId = User.GetUserId();
            var existingPlan = await PlanService.GetPlanByIdAsync(id);

            if (existingPlan is null)
                return NotFound();

            if (existingPlan.OwnerId != ownerId && !User.IsAdminUser())
                return Forbid();

            var updated = await PlanService.UpdatePlanAsync(
                id,
                dto.Title,
                dto.Description,
                dto.StartDate,
                dto.EndDate,
                dto.PlannedBudget,
                dto.Notes);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var ownerId = User.GetUserId();
            var existingPlan = await PlanService.GetPlanByIdAsync(id);

            if (existingPlan is null)
                return NotFound();

            if (existingPlan.OwnerId != ownerId && !User.IsAdminUser())
                return Forbid();

            var deleted = await PlanService.DeletePlanAsync(id);
            if (!deleted)
                return NotFound();

            await SharingService.DeleteTokensByPlanAsync(id);

            return NoContent();
        }

        private static string SanitizeFileName(string value)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            return string.Concat(value.Select(character => invalidChars.Contains(character) ? '-' : character));
        }
    }
}
