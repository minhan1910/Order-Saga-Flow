using MassTransit;
using Microsoft.AspNetCore.Mvc;
using SagaSateMachine.Common.IntegrationEvents;

namespace SagaSateMachine.BasketService.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BasketsController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public BasketsController(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost]
        public async Task<IActionResult> BasketCheckout()
        {
            List<Command.BasketItem> basketItems = new List<Command.BasketItem>()
            {
                new() 
                {
                    CourseId = Guid.NewGuid(),
                    CourseName = "Dotnet Course",
                    Price = 100
                },
                new()
                {
                    CourseId = Guid.NewGuid(),
                    CourseName = "Java Course",
                    Price = 50
                },
            };

            Command.BasketCheckout basketCheckout = new Command.BasketCheckout
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                TimeStamp = DateTime.Now,
                Items = basketItems,
                TotalPrice = basketItems.Sum(x => x.Price),
                TransactionId = Guid.NewGuid()
            };

            using var source = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            await _publishEndpoint.Publish(basketCheckout, source.Token);

            return Accepted();
        }
    }
}
