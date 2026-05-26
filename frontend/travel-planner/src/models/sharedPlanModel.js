import { normalizeActivities } from './activityModel'
import { normalizeChecklistItems } from './checklistModel'
import { normalizeDestinations } from './destinationModel'
import { normalizeExpenses } from './expenseModel'
import { normalizeReminders } from './reminderModel'
import { normalizeTravelPlan } from './travelPlanModel'

export function normalizeSharedPlan(shared) {
  return {
    accessType: Number(shared.accessType ?? 0),
    plan: normalizeTravelPlan(shared.plan ?? {}),
    destinations: normalizeDestinations(shared.destinations ?? []),
    activities: normalizeActivities(shared.activities ?? []),
    expenses: normalizeExpenses(shared.expenses ?? []),
    checklistItems: normalizeChecklistItems(shared.checklistItems ?? []),
    reminders: normalizeReminders(shared.reminders ?? []),
  }
}
