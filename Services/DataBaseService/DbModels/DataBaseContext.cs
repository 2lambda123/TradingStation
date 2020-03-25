﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataBaseService.DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DataBaseService
{
    public class DataBaseContext: DbContext
    {
        public DbSet<DbUser> Users { get; set; }

        private readonly IConfiguration configuration;

        public DataBaseContext(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionStringTradingStation = configuration.GetConnectionString("TradingStationString");
            optionsBuilder.UseSqlServer(connectionStringTradingStation);            
        }

    }
}
