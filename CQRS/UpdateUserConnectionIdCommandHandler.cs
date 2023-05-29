using Amazon.Lambda.APIGatewayEvents;
using System.Threading;
using System.Threading.Tasks;
using Flyingdarts.Persistence;
using MediatR;
using Flyingdarts.Shared;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.Options;
using Amazon.DynamoDBv2.DocumentModel;
using System.Linq;

public class UpdateUserConnectionIdCommandHandler : IRequestHandler<UpdateUserConnectionIdCommand, APIGatewayProxyResponse>
{
    private readonly IDynamoDBContext _dbContext;
    private readonly ApplicationOptions _applicationOptions;
    public UpdateUserConnectionIdCommandHandler(IDynamoDBContext dbContext, IOptions<ApplicationOptions> applicationOptions)
    {
        _dbContext = dbContext;
        _applicationOptions = applicationOptions.Value;
    }
    public async Task<APIGatewayProxyResponse> Handle(UpdateUserConnectionIdCommand request, CancellationToken cancellationToken)
    {
        var user = await GetUserAsync(request.UserId, cancellationToken);

        user.ConnectionId = request.ConnectionId;

        var userWrite = _dbContext.CreateBatchWrite<User>(_applicationOptions.ToOperationConfig()); 
        
        userWrite.AddPutItem(user);

        await userWrite.ExecuteAsync(cancellationToken);

        return new APIGatewayProxyResponse { StatusCode = 200 };
    }
    private async Task<User> GetUserAsync(string userId, CancellationToken cancellationToken) 
    {
        var results = await _dbContext.FromQueryAsync<User>(QueryConfig(userId)).GetRemainingAsync(cancellationToken);

        return results.Single();
    }
    private static QueryOperationConfig QueryConfig(string userId) 
    {
        var queryFilter = new QueryFilter("PK", QueryOperator.Equal, Constants.User);
        queryFilter.AddCondition("SK", QueryOperator.BeginsWith, userId);
        return new QueryOperationConfig { Filter = queryFilter };
    }
}