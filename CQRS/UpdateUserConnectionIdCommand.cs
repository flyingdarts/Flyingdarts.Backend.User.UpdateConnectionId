using Amazon.Lambda.APIGatewayEvents;
using MediatR;

public class UpdateUserConnectionIdCommand : IRequest<APIGatewayProxyResponse>
{
    public string UserId { get; set; }
    internal string ConnectionId { get; set; }
}