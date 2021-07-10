﻿using System;

namespace Neuroglia.AsyncApi.Samples.StreetLightsApi.Messages
{

    [Message(Name = "LightMeasured")]
    public class LightMeasuredEvent
    {

        public int Id { get; set; }

        public int Lumens { get; set; }

        public DateTime SentAt { get; set; }

    }

}
