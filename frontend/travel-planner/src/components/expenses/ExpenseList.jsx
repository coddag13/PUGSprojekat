import ExpenseCard from './ExpenseCard'

function ExpenseList({ expenses, loading, onRefresh, onEdit, onDelete, deletingExpenseId }) {
  return (
    <section className="glass-panel rounded-[2.2rem] p-6">
      <div className="mb-5 flex items-start justify-between gap-4">
        <div>
          <p className="text-sm uppercase tracking-[0.28em] text-slate-500">Pregled troškova</p>
          <h2 className="mt-2 text-3xl font-black tracking-tight text-slate-950">Troškovi</h2>
        </div>

        <button
          className="rounded-[1.25rem] border border-slate-300 bg-white/75 px-4 py-2.5 text-sm font-semibold text-slate-700 transition hover:bg-white"
          type="button"
          onClick={onRefresh}
        >
          Osvježi
        </button>
      </div>

      {loading ? (
        <div className="rounded-[1.8rem] border border-dashed border-slate-300 bg-white/50 p-10 text-center text-slate-500">
          Učitavanje troškova...
        </div>
      ) : expenses.length === 0 ? (
        <div className="rounded-[1.8rem] border border-dashed border-slate-300 bg-white/50 p-10 text-center text-slate-500">
          Još nema unesenih troškova.
        </div>
      ) : (
        <div className="grid gap-4">
          {expenses.map((expense) => (
            <ExpenseCard
              key={expense.id}
              expense={expense}
              onEdit={onEdit}
              onDelete={onDelete}
              isDeleting={deletingExpenseId === expense.id}
            />
          ))}
        </div>
      )}
    </section>
  )
}

export default ExpenseList
