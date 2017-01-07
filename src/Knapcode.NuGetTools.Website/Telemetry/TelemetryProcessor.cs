using System.Collections.Generic;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Knapcode.NuGetTools.Website
{
    public class RequestSuccessInitializer : ITelemetryInitializer
    {
        private const string PropertyKey = "RequestSuccessInitializer";
        private const string PropertyValue = "true";

        private static readonly HashSet<int> SuccessResponseCodes = new HashSet<int>
        {
            404
        };

        public void Initialize(ITelemetry telemetry)
        {
            var requestTelemetry = telemetry as RequestTelemetry;
            if (requestTelemetry == null)
            {
                return;
            }

            int responseCode;
            if (!int.TryParse(requestTelemetry.ResponseCode, out responseCode))
            {
                return;
            }

            if (SuccessResponseCodes.Contains(responseCode))
            {
                requestTelemetry.Success = true;
                requestTelemetry.Context.Properties[PropertyKey] = PropertyValue;
            }
        }
    }
}
