import ExpenseCard from '../expenses/ExpenseCard'

function SharedExpensesSection({ expenses }) {
  return (
    <section className="glass-panel rounded-[2.2rem] p-6">
      <div className="mb-5">
        <p className="text-sm uppercase tracking-[0.28em] text-slate-500">Dijeljene finansije</p>
        <h2 className="mt-2 text-3xl font-black tracking-tight text-slate-950">Troškovi</h2>
      </div>

      {expenses.length === 0 ? (
        <div className="rounded-[1.8rem] border border-dashed border-slate-300 bg-white/50 p-10 text-center text-slate-500">
          Nema dostupnih troškova.
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
