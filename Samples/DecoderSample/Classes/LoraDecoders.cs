// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SensorDecoderModule.Classes
{
    using System;
    using System.Text;
    using Newtonsoft.Json;
    using DecentlabWaterLevelSensorDecoder = Evolution.IoT.LoraWan.Decoders.Decentlab.WaterLevelSensorDecoder;
    using DecentlabWeatherStationSensorDecoder = Evolution.IoT.LoraWan.Decoders.Decentlab.WeatherStationSensorDecoder;
    using LansitecTemperatureHumidityDecoder = Evolution.IoT.LoraWan.Decoders.Lansitec.TemperatureHumidityDecoder;
    using NetvoxTemperatureHumidityDecoder = Evolution.IoT.LoraWan.Decoders.Netvox.TemperatureHumidityDecoder;
    using SensumTemperatureHumidityDecoder = Evolution.IoT.LoraWan.Decoders.Sensum.TemperatureHumidityDecoder;
    using SensumSoilMoistureDecoder = Evolution.IoT.LoraWan.Decoders.Sensum.SoilMoistureDecoder;
    using DigitalMatterOysterDecoder = Evolution.IoT.LoraWan.Decoders.DigitalMatter.OysterDecoder;
    using UsraLinkUC1152Decoder = Evolution.IoT.LoraWan.Decoders.UrsaLink.Uc1152Decoder;
    using UsraLinkUC1114Decoder = Evolution.IoT.LoraWan.Decoders.UrsaLink.Uc1114Decoder;
    using StregaSmartValveDecoder = Evolution.IoT.LoraWan.Decoders.Strega.SmartValveDecoder;
    using Evolution.IoT.LoraWan.Decoders.TTN;

    internal static class LoraDecoders
    {
        private static string DecoderValueSensor(string devEUI, byte[] payload, byte fport)
        {
            // EITHER: Convert a payload containing a string back to string format for further processing
            var result = Encoding.UTF8.GetString(payload);

            // OR: Convert a payload containing binary data to HEX string for further processing
            var result_binary = ConversionHelper.ByteArrayToString(payload);

            // Write code that decodes the payload here.

            // Return a JSON string containing the decoded data
            return JsonConvert.SerializeObject(new { value = result });

        }

        private static string RelayToClassC(string devEUI, byte[] payload, byte fport)
        {
            // EITHER: Convert a payload containing a string back to string format for further processing
            var decodedValue = Encoding.UTF8.GetString(payload);

            // Write code that decodes the payload here.

            // Return a JSON string containing the decoded data
            var resultObject = new 
            {
                value = decodedValue,
                cloudToDeviceMessage = new LoRaCloudToDeviceMessage()
                {
                    DevEUI = "12300000000CCCCC",
                    Payload = "Hello",
                    // RawPayload = "AQIC", // -> Sends 0x01 0x02 0x03 (in base64)
                    Confirmed = false,
                    Fport = fport,
                    MessageId = Guid.NewGuid().ToString(),
                }
            };

            // Return a JSON string containing the decoded data
            return JsonConvert.SerializeObject(resultObject);

        }

              // Add decoders methods here...
        private static string DecoderTheThingsNodeSensor(string devEUI, byte[] payload, uint fport) 
        {
            return ThingsNodeDecoder.Decode(devEUI, payload, fport);
        }

         private static string DecoderLansitecTemperatureHumiditySensor(string devEUI, byte[] payload, uint fport) 
        {
            return LansitecTemperatureHumidityDecoder.Decode(devEUI, payload, fport);
        }

        private static string DecoderDecentlabWaterLevelSensor(string devEUI, byte[] payload, uint fport) 
        {
            return DecentlabWaterLevelSensorDecoder.Decode(devEUI, payload, fport);
        }

        private static string DecoderDecentlabWeatherStationSensor(string devEUI, byte[] payload, uint fport) 
        {
            return DecentlabWeatherStationSensorDecoder.Decode(devEUI, payload, fport);
        }
  

       private static string DecoderNetvoxTemperatureHumidity(string devEUI, byte[] payload, uint fport) 
        {
            return NetvoxTemperatureHumidityDecoder.Decode(devEUI, payload, fport);
        }

        private static string DecodeSensumTemperatureHumidity(string devEUI, byte[] payload, uint fport)
        {
           return SensumTemperatureHumidityDecoder.Decode(devEUI, payload, fport);
        }

        private static string  DecodeSensumSoilMoisture(string devEUI, byte[] payload, uint fport)
        {
            return SensumSoilMoistureDecoder.Decode(devEUI, payload, fport);
        }

        private static string DecodeDigitalMatterOyster(string devEUI, byte[] payload, uint fport)
        {
            return DigitalMatterOysterDecoder.Decode(devEUI, payload, fport);
        }

        private static string DecodeUrsaLinkUc1152(string devEUI, byte[] payload, uint fport)
        {
            return UsraLinkUC1152Decoder.Decode(devEUI, payload, fport);
        }

         private static string DecodeUrsaLinkUc1114(string devEUI, byte[] payload, uint fport)
        {
            return UsraLinkUC1114Decoder.Decode(devEUI, payload, fport);
        }

        private static string DecodeStregaSmartValve(string devEUI, byte[] payload, uint fport)
        {
            return StregaSmartValveDecoder.Decode(devEUI, payload, fport);
        }
    }
}