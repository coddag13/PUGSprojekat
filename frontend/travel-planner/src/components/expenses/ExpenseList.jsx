import ExpenseCard from './ExpenseCard'

function ExpenseList({
  expenses,
  loading,
  onRefresh,
  onEdit,
  onDelete,
  deletingExpenseId,
}) {
  return (
    <section className="rounded-[2rem] bg-white p-6 shadow-lg">
      <div className="mb-5 flex items-center justify-between">
        <h2 className="text-2xl font-bold text-slate-900">Troškovi</h2>

        <button
          className="rounded-2xl border border-slate-300 px-4 py-2 text-sm font-semibold text-slate-700 hover:bg-slate-50"
          type="button"
          onClick={onRefresh}
        >
          Osvježi
        </button>
      </div>

      {loading ? (
        <div className="rounded-3xl border border-dashed border-slate-300 p-10 text-center text-slate-500">
          Učitavanje troškova...
        </div>
      ) : expenses.length === 0 ? (
        <div className="rounded-3xl border border-dashed border-slate-300 p-10 text-center text-slate-500">
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