using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using netploy.rabbit_producer.api.models;
using netploy.rabbit_producer.api.utils;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace netploy.rabbit_producer.api.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IRabbitMQService rabbitMQService;

        public MessageController(IRabbitMQService rabbitMQService)
        {
            this.rabbitMQService = rabbitMQService;
        }

        [HttpPost]
        public string produce([FromBody] SampleMessageType body)
        {
            rabbitMQService.SendMessage(body);
            return "Ok";
        }
    }
}

