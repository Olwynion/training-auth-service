using System.Data;
using Dapper;
using Training.Auth.Domain.ValueObjects;

namespace Training.Auth.Infrastructure.Mappings;

public class UserIdTypeHandler : SqlMapper.TypeHandler<UserId>
{
    public override UserId Parse(object value)
    {
        return new UserId(Guid.Parse(value.ToString()!));
    }

    public override void SetValue(IDbDataParameter parameter, UserId value)
    {
        parameter.Value = value.Value;
        parameter.DbType = DbType.Guid;
    }
}
