using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyEmployees.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;

        public WeatherForecastController(IRepositoryManager repository, ILoggerManager logger, IServiceScopeFactory fac) 
        { 
            _repository = repository;
            _logger = logger;
            using (var scope = fac.CreateScope()) 
            { 
                scope.ServiceProvider.GetService<IRepositoryManager>();
            }
                
            
        }
        [HttpGet] 
        public ActionResult<IEnumerable<string>> Get() 
        { 
            _repository.Company.GetType(); //45
            _repository.Employee.GetType();
            _logger.LogDebug("ss");
            return new string[] { "value1", "value2" }; 
        }
    }
}
