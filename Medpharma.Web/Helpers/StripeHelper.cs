using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Threading.Tasks;
using Vereyon.Web;

namespace Medpharma.Web.Helpers
{
    public class StripeHelper : IStripeHelper
    {
        private readonly IConfiguration _configuration;
        private readonly IFlashMessage _flashMessage;

        public StripeHelper(IConfiguration configuration, IFlashMessage flashMessage)
        {
            _configuration = configuration;
            _flashMessage = flashMessage;
        }

        public async Task<Response> PayAsync(string cardNumber, string expMonth, string expYear, string cvc, int amountToPay)
        {
            bool result = false;
            string stripeMessage = "";

            try
            {
                StripeConfiguration.ApiKey = _configuration["StripeApiKey:Key"];

                var optionsToken = new TokenCreateOptions
                {
                    Card = new TokenCardOptions
                    {
                        Number = cardNumber,
                        ExpMonth = expMonth,
                        ExpYear = expYear,
                        Cvc = cvc
                    }
                };

                var serviceToken = new TokenService();
                Token stripeToken = await serviceToken.CreateAsync(optionsToken);

                var options = new ChargeCreateOptions
                {
                    Amount = amountToPay,
                    Currency = "eur",
                    Description = "TESTE STRIPE MEDPHARMA",

                    Source = stripeToken.Id
                };

                var service = new ChargeService();

                Charge charge = await service.CreateAsync(options);

                if (charge.Paid)
                {
                    result = true;
                }

            }
            catch (Exception ex)
            {
                stripeMessage = ex.Message;
            }

            return new Response { IsSuccess = result, Message = stripeMessage };
        }
    }
}
