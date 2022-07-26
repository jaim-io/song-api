using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Song.API.Models;

namespace Song.API.IntergrationTests
{
  public class SongControllerTests
      : IClassFixture<CustomWebApplicationFactory<Program>>
  {
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions()
    {
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public SongControllerTests(CustomWebApplicationFactory<Program> factory)
    {
      _factory = factory;
      _client = factory.CreateClient(new WebApplicationFactoryClientOptions
      {
        AllowAutoRedirect = false,
      });
    }

    [Fact]
    public async void GetSongs()
    {
      // Arrange 

      // Act
      var response = await _client.GetAsync("/api/song/");
      var responseString = await response.Content.ReadAsStringAsync();
      var songs = JsonSerializer.Deserialize<List<Models.Song>>(responseString, _jsonSerializerOptions);

      // Assert
      response.EnsureSuccessStatusCode();
      Assert.Equal(HttpStatusCode.OK, response.StatusCode);
      Assert.NotNull(songs);
      Assert.Equal(4, songs?.Count);
    }

    [Fact]
    public async void GetSong_GivenID_ReturnsSong()
    {
      // Arrange 
      var id = 1;

      // Act
      var response = await _client.GetAsync($"/api/song/{id}");
      var responseString = await response.Content.ReadAsStringAsync();
      var song = JsonSerializer.Deserialize<Models.Song>(responseString, _jsonSerializerOptions);

      // Assert
      response.EnsureSuccessStatusCode();
      Assert.Equal(HttpStatusCode.OK, response.StatusCode);
      Assert.NotNull(song);
      Assert.Equal(id, song?.Id);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(213)]
    public async void TheoryGetSong_GivenNonExistingID_ReturnsNotFound(int id)
    {
      // Arrange 

      // Act
      var response = await _client.GetAsync($"/api/song/{id}");

      // Assert
      Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async void PutSong_GivenSongAndMatchingID_ReturnsNoContent()
    {
      // Arrange
      var id = 1;
      var song = new Models.Song() { Id = 1, Name = "A Place For My Head", Artist = "Linkin Park", ImageUrl = "https://picsum.photos/200/300" };
      var jsonStr = JsonSerializer.Serialize(song);
      var content = new StringContent(jsonStr, Encoding.UTF8, "application/json");

      // Act
      var response = await _client.PutAsync($"/api/song/{id}", content);

      // Assert
      // response.EnsureSuccessStatusCode();
      Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async void PutSong_GivenSongAndNotMatchingID_ReturnsBadRequest()
    {
      // Arrange
      var id = -1;
      var song = new Models.Song() { Id = 1, Name = "A Place For My Head", Artist = "Linkin Park", ImageUrl = "https://picsum.photos/200/300" };
      var jsonStr = JsonSerializer.Serialize(song);
      var content = new StringContent(jsonStr, Encoding.UTF8, "application/json");

      // Act
      var response = await _client.PutAsync($"/api/song/{id}", content);

      // Assert
      Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async void PutSong_NotExistingSongAndID_ReturnsNotFound()
    {
      // Arrange
      var id = -1;
      var song = new Models.Song() { Id = -1, Name = "A Place For My Head", Artist = "Linkin Park", ImageUrl = "https://picsum.photos/200/300" };
      var jsonStr = JsonSerializer.Serialize(song);
      var content = new StringContent(jsonStr, Encoding.UTF8, "application/json");

      // Act
      var response = await _client.PutAsync($"/api/song/{id}", content);

      // Assert
      Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }


    [Fact]
    public async void PostSong_GivenAlbum_ReturnsAlbum()
    {
      // Arrange 
      var song = new Models.Song() { Name = "X Gon' Give It To Ya", Artist = "DMX", ImageUrl = "https://picsum.photos/200/300" };
      var jsonStr = JsonSerializer.Serialize(song);
      var content = new StringContent(jsonStr, Encoding.UTF8, "application/json");

      // Act
      var response = await _client.PostAsync($"/api/song/", content);

      // Assert
      response.EnsureSuccessStatusCode();
      Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async void DeleteSong_GivenExistingID_ReturnsNoContent()
    {
      // Arrange 
      var id = 4;

      // Act
      var response = await _client.DeleteAsync($"/api/song/{id}");

      // Assert
      response.EnsureSuccessStatusCode();
      Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }


    [Fact]
    public async void DeleteSong_GivenNonExistingID_ReturnsNotFound()
    {
      // Arrange 
      var id = -1;

      // Act
      var response = await _client.DeleteAsync($"/api/song/{id}");

      // Assert
      Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
  }
}
