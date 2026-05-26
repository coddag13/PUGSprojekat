import ReminderList from '../reminders/ReminderList'

function SharedRemindersSection({ reminders }) {
  return (
    <ReminderList
      reminders={reminders}
      loading={false}
      allowActions={false}
    />
  )
}

export default SharedRemindersSection
