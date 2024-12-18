using FastEndpoints;
using Microsoft.Extensions.Options;
using RestSharp;

public class Request
{
    public string Street { get; set; }
    public int Number { get; set; }
    public string City { get; set; }
    public string Neighborhood { get; set; }
    public string State { get; set; }
    public int Cep { get; set; }
    public string Country { get; set; }
}

public class Response
{
    public Address Address { get; set; }
    public string Message { get; set; }
}

public class Address
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class Endpoint : Endpoint<Request, Response>
{
    private readonly GoogleMapsConfig _googleMapsConfig;
    private readonly ILogger<Endpoint> _logger;

    public Endpoint(IOptions<GoogleConfig> googleMapsConfig, ILogger<Endpoint> logger)
    {
        _googleMapsConfig = googleMapsConfig.Value.Maps;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/addresses");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        try
        {
            var client = new RestClient(_googleMapsConfig.Url);
            var request = new RestRequest();
            var address = $"{req.Number} {req.Street} {req.Neighborhood}, {req.City}, {req.State} {req.Cep}, {req.Country}";

            request.AddParameter("address", address);
            request.AddParameter("key", _googleMapsConfig.ApiKey);

            var response = await client.ExecuteGetAsync<GeocodeResponse>(request);

            if (response.IsSuccessful && response.Data.Results.Any())
            {
                var location = response.Data.Results.First().Geometry.Location;

                await SendAsync(new Response
                {
                    Address = new Address
                    {
                        Latitude = location.Lat,
                        Longitude = location.Lng
                    }
                });
            }
            else
            {
                await SendNotFoundAsync();
            }
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            await SendAsync(new Response
            {
                Message = "An unexpected error occurred."
            }, 500);
        }
    }
}

public class GeocodeResponse
{
    public List<Result> Results { get; set; }
}

public class Result
{
    public Geometry Geometry { get; set; }
}

public class Geometry
{
    public Location Location { get; set; }
}

public class Location
{
    public double Lat { get; set; }
    public double Lng { get; set; }
}

public class GoogleConfig
{
    public GoogleMapsConfig Maps { get; set; }
}

public class GoogleMapsConfig
{
    public string Url { get; set; }
    public string ApiKey { get; set; }
}