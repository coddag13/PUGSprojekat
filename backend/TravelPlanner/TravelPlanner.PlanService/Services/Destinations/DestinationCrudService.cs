using Microsoft.EntityFrameworkCore;
using TravelPlanner.Common;
using TravelPlanner.Common.Models;
using TravelPlanner.Infrastructure.Entities;
using TravelPlanner.PlanService.Mappings;
using TravelPlanner.PlanService.Services.Common;
using TravelPlanner.PlanService.Validation;

namespace TravelPlanner.PlanService.Services.Destinations
{
    internal sealed class DestinationCrudService : IDestinationCrudService
    {
        private readonly IPlanServiceDbContextFactory _dbFactory;
        private readonly IDestinationValidationService _validationService;

        public DestinationCrudService(
            IPlanServiceDbContextFactory dbFactory,
            IDestinationValidationService validationService)
        {
            _dbFactory = dbFactory;
            _validationService = validationService;
        }

        public async Task<List<DestinationData>> GetDestinationsAsync(Guid travelPlanId)
        {
            await using var db = _dbFactory.CreateDbContext();

            var destinations = await db.Destinations
                .AsNoTracking()
                .Where(d => d.TravelPlanId == travelPlanId)
                .ToListAsync();

            return destinations.Select(DestinationMapper.Map).ToList();
        }

        public async Task<DestinationData?> GetDestinationByIdAsync(Guid travelPlanId, Guid id)
        {
            await using var db = _dbFactory.CreateDbContext();

            var destination = await db.Destinations
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id && d.TravelPlanId == travelPlanId);

            return destination is null ? null : DestinationMapper.Map(destination);
        }

        public async Task<ServiceResponse<DestinationData>> CreateDestinationAsync(
            Guid travelPlanId,
            string name,
            string location,
            DateTime arrivalDate,
            DateTime departureDate,
            string description)
        {
            await using var db = _dbFactory.CreateDbContext();

            name = name?.Trim() ?? string.Empty;
            location = location?.Trim() ?? string.Empty;
            description = description?.Trim() ?? string.Empty;

            var fieldError = _validationService.ValidateFields(name, location, arrivalDate, departureDate);
            if (fieldError is not null)
                return ServiceResponse<DestinationData>.Fail(fieldError);

            var planError = await _validationService.ValidateForPlanAsync(db, travelPlanId, arrivalDate, departureDate);
            if (planError is not null)
                return ServiceResponse<DestinationData>.Fail(planError);

            var destination = new Destination
            {
                Id = Guid.NewGuid(),
                TravelPlanId = travelPlanId,
                Name = name,
                Location = location,
                ArrivalDate = arrivalDate,
                DepartureDate = departureDate,
                Description = description
            };

            db.Destinations.Add(destination);
            await db.SaveChangesAsync();

            return ServiceResponse<DestinationData>.Ok(DestinationMapper.Map(destination));
        }

        public async Task<bool> UpdateDestinationAsync(
            Guid travelPlanId,
            Guid id,
            string name,
            string location,
            DateTime arrivalDate,
            DateTime departureDate,
            string description)
        {
            await using var db = _dbFactory.CreateDbContext();

            var destination = await db.Destinations
                .FirstOrDefaultAsync(d => d.Id == id && d.TravelPlanId == travelPlanId);

            if (destination is null)
                return false;

            name = name?.Trim() ?? string.Empty;
            location = location?.Trim() ?? string.Empty;
            description = description?.Trim() ?? string.Empty;

            if (_validationService.ValidateFields(name, location, arrivalDate, departureDate) is not null)
                return false;

            var planError = await _validationService.ValidateForPlanAsync(db, travelPlanId, arrivalDate, departureDate, id);
            if (planError is not null)
                return false;

            destination.Name = name;
            destination.Location = location;
            destination.ArrivalDate = arrivalDate;
            destination.DepartureDate = departureDate;
            destination.Description = description;

            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteDestinationAsync(Guid travelPlanId, Guid id)
        {
            await using var db = _dbFactory.CreateDbContext();

            var destination = await db.Destinations
                .FirstOrDefaultAsync(d => d.Id == id && d.TravelPlanId == travelPlanId);

            if (destination is null)
                return false;

            var relatedActivities = await db.PlanActivities
                .Where(a => a.TravelPlanId == travelPlanId && a.DestinationId == id)
                .ToListAsync();

            if (relatedActivities.Count > 0)
            {
                db.PlanActivities.RemoveRange(relatedActivities);
            }

            db.Destinations.Remove(destination);
            await db.SaveChangesAsync();
            return true;
        }
    }
}
