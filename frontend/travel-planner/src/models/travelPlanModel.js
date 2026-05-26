export function createEmptyTravelPlanForm() {
  return {
    title: '',
    description: '',
    startDate: '',
    endDate: '',
    plannedBudget: '',
    notes: '',
  }
}

export function createTravelPlanPayload(form) {
  return {
    title: form.title.trim(),
    description: form.description.trim(),
    startDate: `${form.startDate}T00:00:00`,
    endDate: `${form.endDate}T00:00:00`,
    plannedBudget: Number(form.plannedBudget),
    notes: form.notes.trim(),
  }
}

export function normalizeTravelPlan(plan) {
  return {
    id: plan.id ?? '',
    ownerId: plan.ownerId ?? '',
    title: plan.title ?? '',
    description: plan.description ?? '',
    startDate: plan.startDate ?? '',
    endDate: plan.endDate ?? '',
    plannedBudget: Number(plan.plannedBudget ?? 0),
    notes: plan.notes ?? '',
  }
}

export function normalizeTravelPlans(plans) {
  return plans.map(normalizeTravelPlan)
}
