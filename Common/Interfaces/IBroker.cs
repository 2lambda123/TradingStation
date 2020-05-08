﻿using System;
using DTO;
using DTO.MarketBrokerObjects;
using System.Collections.Generic;

namespace Interfaces
{
    public interface IBroker
    {
        IEnumerable<Instrument> GetInstruments(InstrumentType type);

        IEnumerable<Candle> SubscribeOnCandle(string Figi, Action<Candle> SendCandle);

        /// <summary>
        /// Depth of market glass
        /// </summary>
        int Depth { get; set; }
    }
}
