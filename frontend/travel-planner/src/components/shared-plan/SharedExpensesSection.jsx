import ExpenseCard from '../expenses/ExpenseCard'

function SharedExpensesSection({ expenses }) {
  return (
    <section className="rounded-[2rem] bg-white p-6 shadow-lg">
      <h2 className="mb-5 text-2xl font-bold text-slate-900">Troskovi</h2>

      {expenses.length === 0 ? (
        <div className="rounded-3xl border border-dashed border-slate-300 p-10 text-center text-slate-500">
          Nema dostupnih troskova.
        </div>
      ) : (
        <div className="grid gap-4">
          {expenses.map((expense) => (
            <ExpenseCard key={expense.id} expense={expense} />
          ))}
        </div>
      )}
    </section>
  )
}

export default SharedExpensesSection
