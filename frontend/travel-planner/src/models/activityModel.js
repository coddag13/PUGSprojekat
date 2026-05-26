export function createEmptyActivityForm() {
  return {
    destinationId: '',
    name: '',
    date: '',
    time: '',
    location: '',
    description: '',
    estimatedCost: '',
    status: '0',
  }
}

export function createActivityPayload(form) {
  return {
    destinationId: form.destinationId || null,
    name: form.name.trim(),
    date: `${form.date}T00:00:00`,
    time: `${form.time}:00`,
    location: form.location.trim(),
    description: form.description.trim(),
    estimatedCost: Number(form.estimatedCost),
    status: Number(form.status),
  }
}

export function createActivityFormModel(activity) {
  return {
    destinationId: activity.destinationId ?? '',
    name: activity.name ?? '',
    date: activity.date?.slice(0, 10) ?? '',
    time: activity.time?.slice(0, 5) ?? '',
    location: activity.location ?? '',
    description: activity.description ?? '',
    estimatedCost: String(activity.estimatedCost ?? ''),
    status: String(activity.status ?? 0),
  }
}

export function normalizeActivity(activity) {
  return {
    id: activity.id ?? '',
    travelPlanId: activity.travelPlanId ?? '',
    destinationId: activity.destinationId ?? null,
    name: activity.name ?? '',
    date: activity.date ?? '',
    time: activity.time ?? '',
    location: activity.location ?? '',
    description: activity.description ?? '',
    estimatedCost: Number(activity.estimatedCost ?? 0),
    status: Number(activity.status ?? 0),
  }
}

export function normalizeActivities(activities) {
  return activities.map(normalizeActivity)
}
