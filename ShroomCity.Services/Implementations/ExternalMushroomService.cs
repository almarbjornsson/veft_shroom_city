using System.Net.Http.Json;
using ShroomCity.Models;
using ShroomCity.Models.Dtos;
using ShroomCity.Models.Exceptions;
using ShroomCity.Services.Interfaces;

namespace ShroomCity.Services.Implementations;

public class ExternalMushroomService : IExternalMushroomService
{
    private readonly HttpClient _httpClient;

    public ExternalMushroomService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ExternalMushroomDto?> GetMushroomByName(string name)
    {
        try
        {
            var externalMushroom = await _httpClient.GetFromJsonAsync<ExternalMushroomDto>(name);
            return externalMushroom;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            throw new MushroomNotFoundException($"Mushroom with name {name} was not found.");
        }

    }


    public async Task<Envelope<ExternalMushroomDto>?> GetMushrooms(int pageSize, int pageNumber)
    {
        try
        {
            var queryString = $"?pageSize={pageSize}&pageNumber={pageNumber}";
            var mushies = await _httpClient.GetFromJsonAsync<Envelope<ExternalMushroomDto>>(queryString);
            return mushies;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null; 
        }
    }

}