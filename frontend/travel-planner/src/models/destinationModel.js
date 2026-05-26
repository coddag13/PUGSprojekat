export function createEmptyDestinationForm() {
  return {
    name: '',
    location: '',
    arrivalDate: '',
    departureDate: '',
    description: '',
  }
}

export function createDestinationPayload(form) {
  return {
    name: form.name.trim(),
    location: form.location.trim(),
    arrivalDate: `${form.arrivalDate}T00:00:00`,
    departureDate: `${form.departureDate}T00:00:00`,
    description: form.description.trim(),
  }
}

export function createDestinationFormModel(destination) {
  return {
    name: destination.name ?? '',
    location: destination.location ?? '',
    arrivalDate: destination.arrivalDate?.slice(0, 10) ?? '',
    departureDate: destination.departureDate?.slice(0, 10) ?? '',
    description: destination.description ?? '',
  }
}

export function normalizeDestination(destination) {
  return {
    id: destination.id ?? '',
    travelPlanId: destination.travelPlanId ?? '',
    name: destination.name ?? '',
    location: destination.location ?? '',
    arrivalDate: destination.arrivalDate ?? '',
    departureDate: destination.departureDate ?? '',
    description: destination.description ?? '',
  }
}

export function normalizeDestinations(destinations) {
  return destinations.map(normalizeDestination)
}
