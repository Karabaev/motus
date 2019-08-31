namespace NotificationService.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using ViewModels.Email;
    using Shared.Mail; 

    [ApiController]
    public class EmailController : ControllerBase
    {
        public EmailController(IMailClient client)
        {
            this.mailClient = client;
        }

        
        [HttpPost("email/send")]
        public async Task<IActionResult> SendMessageAsync([FromBody] SendMessageViewModel model)
        {
            if (!ModelState.IsValid)
                return new JsonResult(new { error = "Одно из полей невалидно" });

            try
            {
                await this.mailClient.SendMessageToManyDestinationsAsync(model.Destinations, model.Caption, model.Body);
                return Ok(new { success = "Сообщение отправлено" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = ex.ToString() });
            }
        }

        private readonly IMailClient mailClient;
    }
}