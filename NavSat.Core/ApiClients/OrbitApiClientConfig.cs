using AutoMapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavSat.Core.ApiClients
{
    public class OrbitApiClientConfig : IOrbitApiClientConfig
    {

        public OrbitApiClientConfig(string baseUrl )
        {
            BaseUrl = baseUrl;
        }
        public string BaseUrl { get;  }
    }
}
