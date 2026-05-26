export function createEmptyReminderForm() {
  return {
    title: '',
    remindAt: '',
  }
}

export function createReminderPayload(form, isCompleted = false) {
  return {
    title: form.title.trim(),
    remindAt: new Date(form.remindAt).toISOString(),
    isCompleted,
  }
}

export function createReminderFormModel(reminder) {
  return {
    title: reminder.title ?? '',
    remindAt: reminder.remindAt ? reminder.remindAt.slice(0, 16) : '',
  }
}

export function normalizeReminder(reminder) {
  return {
    id: reminder.id ?? '',
    travelPlanId: reminder.travelPlanId ?? '',
    title: reminder.title ?? '',
    remindAt: reminder.remindAt ?? '',
    isCompleted: Boolean(reminder.isCompleted),
  }
}

export function normalizeReminders(reminders) {
  return reminders.map(normalizeReminder)
}
