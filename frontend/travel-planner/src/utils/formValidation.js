function formatFieldValue(value) {
  return value ? ` ${value}.` : '.'
}

function getLocalizedValidationMessage(field) {
  const { validity, type, min, max, minLength, maxLength, title } = field

  if (validity.valueMissing) {
    return 'Ovo polje je obavezno.'
  }

  if (validity.typeMismatch) {
    if (type === 'email') {
      return 'Unesite ispravnu email adresu.'
    }

    if (type === 'url') {
      return 'Unesite ispravnu web adresu.'
    }

    return 'Unesite ispravnu vrijednost.'
  }

  if (validity.badInput) {
    if (type === 'number') {
      return 'Unesite ispravan broj.'
    }

    return 'Unesite ispravnu vrijednost.'
  }

  if (validity.rangeUnderflow) {
    if (type === 'date' || type === 'datetime-local') {
      return `Vrijednost ne može biti prije${formatFieldValue(min)}`
    }

    return `Vrijednost mora biti najmanje${formatFieldValue(min)}`
  }

  if (validity.rangeOverflow) {
    if (type === 'date' || type === 'datetime-local') {
      return `Vrijednost ne može biti poslije${formatFieldValue(max)}`
    }

    return `Vrijednost može biti najviše${formatFieldValue(max)}`
  }

  if (validity.tooShort) {
    return `Unesite najmanje ${minLength} karaktera.`
  }

  if (validity.tooLong) {
    return `Unesite najviše ${maxLength} karaktera.`
  }

  if (validity.stepMismatch) {
    return 'Unesite dozvoljenu vrijednost za ovo polje.'
  }

  if (validity.patternMismatch) {
    return title || 'Unesena vrijednost nije u ispravnom formatu.'
  }

  return field.validationMessage || 'Unesite ispravnu vrijednost.'
}

export function createFormValidationHandlers() {
  const handleInvalid = (event) => {
    const field = event.target

    if (typeof field?.setCustomValidity !== 'function') {
      return
    }

    field.setCustomValidity(getLocalizedValidationMessage(field))
  }

  const handleInput = (event) => {
    const field = event.target

    if (typeof field?.setCustomValidity !== 'function') {
      return
    }

    field.setCustomValidity('')
  }

  return {
    onInvalidCapture: handleInvalid,
    onInputCapture: handleInput,
  }
}
