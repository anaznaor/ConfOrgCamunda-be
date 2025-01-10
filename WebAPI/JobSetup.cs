using AutoMapper;
using WebAPI.Interface;
using WebAPI.Models;
using WebAPI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
//using Storage.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI
{
    public class JobSetup
    {
        private readonly IMapper _mapper;
        private readonly ILogger<JobSetup> _logger;
        private readonly IServiceProvider _serviceProvider;

        public JobSetup(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _mapper = serviceProvider.GetRequiredService<IMapper>();
            _logger = serviceProvider.GetRequiredService<ILogger<JobSetup>>();
        }
    }
}