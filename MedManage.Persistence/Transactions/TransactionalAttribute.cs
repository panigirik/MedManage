using System.Data;

namespace MedManage.Persistence.Transactions;

[AttributeUsage(AttributeTargets.Method, Inherited = true)]
public sealed class TransactionalAttribute : Attribute
{
    public IsolationLevel IsolationLevel { get; init; } = IsolationLevel.RepeatableRead;
}
