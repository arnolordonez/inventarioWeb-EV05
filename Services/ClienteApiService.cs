using System.Net.Http.Json;
using InventarioWEB.Models.Clientes.Dto;

namespace InventarioWEB.Services
{
    public class ClienteApiService
    {
        private readonly HttpClient _httpClient;

        public ClienteApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // ======================================================
        // Obtener todos los clientes
        // ======================================================
        public async Task<List<ClienteListadoDto>> GetClientesAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<ClienteListadoDto>>("clientes");
            return response ?? new List<ClienteListadoDto>();
        }

        // ======================================================
        // Obtener un cliente por ID
        // ======================================================
        public async Task<ClienteDetalleDto?> GetClienteAsync(int id)
        {
            var response = await _httpClient.GetFromJsonAsync<ClienteDetalleDto>($"clientes/{id}");
            return response;
        }

        // ======================================================
        // Crear cliente
        // ======================================================
        public async Task<bool> CrearClienteAsync(ClienteCrearDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("clientes", dto);
            return response.IsSuccessStatusCode;
        }

        // ======================================================
        // Actualizar cliente
        // ======================================================
        public async Task<bool> ActualizarClienteAsync(int id, ClienteActualizarDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"clientes/{id}", dto);
            return response.IsSuccessStatusCode;
        }

        // ======================================================
        // Eliminar cliente
        // ======================================================
        public async Task<bool> EliminarClienteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"clientes/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
