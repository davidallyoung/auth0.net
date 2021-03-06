using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Auth0.Core.Collections;
using Auth0.Core.Http;
using Auth0.ManagementApi.Models;
using Auth0.ManagementApi.Serialization;
using Newtonsoft.Json;

namespace Auth0.ManagementApi.Clients
{
    /// <summary>
    /// Contains all the methods to call the /clients endpoints.
    /// </summary>
    public class ClientsClient : ClientBase, IClientsClient
    {
        /// <summary>
        /// Creates a new instance of the ClientBase class.
        /// </summary>
        /// <param name="connection">The <see cref="IApiConnection" /> which is used to communicate with the API.</param>
        public ClientsClient(IApiConnection connection)
            : base(connection)
        {
        }

        /// <summary>
        /// Creates a new client application.
        /// </summary>
        /// <param name="request">The request containing the properties of the new client.</param>
        /// <returns>Task&lt;Core.Client&gt;.</returns>
        public Task<Client> CreateAsync(ClientCreateRequest request)
        {
            return Connection.PostAsync<Client>("clients", request, null, null, null, null, null);
        }

        /// <summary>
        /// Deletes a client and all its related assets (like rules, connections, etc) given its id.
        /// </summary>
        /// <param name="id">The id of the client to delete.</param>
        /// <returns>Task.</returns>
        public Task DeleteAsync(string id)
        {
            return Connection.DeleteAsync<object>("clients/{id}", new Dictionary<string, string>
            {
                {"id", id}
            }, null);
        }

        public Task<IPagedList<Client>> GetAllAsync(int? page = null, int? perPage = null, bool? includeTotals = null, string fields = null, bool? includeFields = null, 
            bool? isGlobal = null, bool? isFirstParty = null, ClientApplicationType[] appType = null)
        {
            var queryStrings = new Dictionary<string, string>
            {
                {"page", page?.ToString()},
                {"per_page", perPage?.ToString()},
                {"include_totals", includeTotals?.ToString().ToLower()},
                {"fields", fields},
                {"include_fields", includeFields?.ToString().ToLower()},
                {"is_global", isGlobal?.ToString().ToLower()},
                { "is_first_party", isFirstParty?.ToString().ToLower()}
            };

            if (appType != null)
            {
                queryStrings.Add("app_type", string.Join(",", appType.Select(ToEnumString)));
            }
            
            return Connection.GetAsync<IPagedList<Client>>("clients", null, queryStrings, null, new PagedListConverter<Client>("clients"));
        }

        /// <summary>
        /// Retrieves a list of all client applications. Accepts a list of fields to include or exclude.
        /// </summary>
        /// <param name="fields">A comma separated list of fields to include or exclude (depending on includeFields) from the
        /// result, empty to retrieve all fields</param>
        /// <param name="includeFields">true if the fields specified are to be included in the result, false otherwise (defaults to
        /// true)</param>
        /// <returns>Task&lt;IList&lt;Core.Client&gt;&gt;.</returns>
        [Obsolete("Use the paged method overload instead")]
        public Task<IList<Client>> GetAllAsync(string fields = null, bool includeFields = true)
        {
            return Connection.GetAsync<IList<Client>>("clients", null,
                new Dictionary<string, string>
                {
                    {"fields", fields},
                    {"include_fields", includeFields.ToString().ToLower()}
                }, null, null);
        }

        /// <summary>
        /// Retrieves a client by its id.
        /// </summary>
        /// <param name="id">The id of the client to retrieve</param>
        /// <param name="fields">A comma separated list of fields to include or exclude (depending on includeFields) from the
        /// result, empty to retrieve all fields</param>
        /// <param name="includeFields">true if the fields specified are to be included in the result, false otherwise (defaults to
        /// true)</param>
        /// <returns>Task&lt;Core.Client&gt;.</returns>
        public Task<Client> GetAsync(string id, string fields = null, bool includeFields = true)
        {
            return Connection.GetAsync<Client>("clients/{id}",
                new Dictionary<string, string>
                {
                    {"id", id}
                },
                new Dictionary<string, string>
                {
                    {"fields", fields},
                    {"include_fields", includeFields.ToString().ToLower()}
                }, null, null);
        }

        /// <summary>
        /// Rotate a client secret. The generated secret is NOT base64 encoded.
        /// </summary>
        /// <param name="id">The id of the client which secret needs to be rotated</param>
        /// <returns></returns>
        public Task<Client> RotateClientSecret(string id)
        {
            return Connection.PostAsync<Client>("clients/{id}/rotate-secret", null, null, null, new Dictionary<string, string>
            {
                {"id", id}
            }, null, null);
        }

        /// <summary>
        /// Updates a client application.
        /// </summary>
        /// <param name="id">The id of the client you want to update.</param>
        /// <param name="request">The request containing the properties of the client you want to update.</param>
        /// <returns>Task&lt;Core.Client&gt;.</returns>
        public Task<Client> UpdateAsync(string id, ClientUpdateRequest request)
        {
            return Connection.PatchAsync<Client>("clients/{id}", request, new Dictionary<string, string>
            {
                {"id", id}
            });
        }
        
        private string ToEnumString<T>(T type)
        {
            var enumType = typeof(T);
            var name = Enum.GetName(enumType, type);
            var enumMemberAttribute = ((EnumMemberAttribute[])enumType.GetTypeInfo().GetDeclaredField(name).GetCustomAttributes(typeof(EnumMemberAttribute), true)).Single();
            return enumMemberAttribute.Value;
        }

    }
}