using FastEndpoints;
using RestSharp;

public class AddressRequest
{
    public string Street { get; set; }
    public int Number { get; set; }
    public string City { get; set; }
    public string Neighborhood { get; set; }
    public string State { get; set; }
}

public class AddressResponse
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class AddressEndpoint : Endpoint<AddressRequest, AddressResponse>
{
    public override void Configure()
    {
        Post("/addresses");
        AllowAnonymous();
    }

    public override async Task HandleAsync(AddressRequest req, CancellationToken ct)
    {
        var client = new RestClient("https://maps.googleapis.com/maps/api/geocode/json");
        var request = new RestRequest();
        
        request.AddParameter("address", $"{req.Street}, {req.Number}, {req.Neighborhood}, {req.City} - {req.State}");
        request.AddParameter("key", "YOUR_GOOGLE_MAPS_API_KEY");

        var response = await client.ExecuteGetAsync<GeocodeResponse>(request);

        if (response.IsSuccessful && response.Data.Results.Any())
        {
            var location = response.Data.Results.First().Geometry.Location;

            await SendAsync(new AddressResponse
            {
                Latitude = location.Lat,
                Longitude = location.Lng
            });
        }
        else
        {
            await SendNotFoundAsync();
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