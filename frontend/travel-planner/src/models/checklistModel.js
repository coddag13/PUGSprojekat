export function createEmptyChecklistForm() {
  return {
    text: '',
  }
}

export function createChecklistItemPayload(form) {
  return {
    text: form.text.trim(),
  }
}

export function normalizeChecklistItem(item) {
  return {
    id: item.id ?? '',
    travelPlanId: item.travelPlanId ?? '',
    text: item.text ?? '',
    isCompleted: Boolean(item.isCompleted),
  }
}

export function normalizeChecklistItems(items) {
  return items.map(normalizeChecklistItem)
}
