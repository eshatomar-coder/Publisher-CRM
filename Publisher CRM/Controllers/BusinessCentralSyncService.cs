using Microsoft.Extensions.Configuration; // <-- Add this
using Microsoft.Extensions.DependencyInjection;
using Publisher_CRM.Data;
using Publisher_CRM.Models;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Linq;

namespace Publisher_CRM.Services
{
    public interface IBusinessCentralSyncService
    {
        Task SyncAccountsFromApiAsync();
    }

    public class BusinessCentralSyncService : IBusinessCentralSyncService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config; // <-- Add Configuration

        public BusinessCentralSyncService(
            IServiceScopeFactory scopeFactory,
            IHttpClientFactory httpClientFactory,
            IConfiguration config)
        {
            _scopeFactory = scopeFactory;
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        public async Task SyncAccountsFromApiAsync()
        {
            // 1. Read Settings
            var username = _config["BusinessCentral:Username"]?.Trim();
            var password = _config["BusinessCentral:Password"]?.Trim();

            // Read both URLs from appsettings.json
            var urlBJPL = _config["BusinessCentral:BaseUrlBJPL"]?.Trim();
            var urlLLP = _config["BusinessCentral:BaseUrlLLP"]?.Trim();

            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // 2. Process BJPL data
                if (!string.IsNullOrEmpty(urlBJPL))
                {
                    await FetchAndProcessData(urlBJPL, "bjpl", username, password, context);
                }

                // 3. Process LLP data
                if (!string.IsNullOrEmpty(urlLLP))
                {
                    await FetchAndProcessData(urlLLP, "bjain pegasus llp", username, password, context);
                }

                // 4. Save all changes to the database
                await context.SaveChangesAsync();
            }
        }

        // Helper method to process each API URL so we don't duplicate code
        private async Task FetchAndProcessData(string requestUrl, string targetCompany, string username, string password, ApplicationDbContext context)
        {
            // Setup NTLM/Windows Authentication
            string domain = "";
            string user = username;

            if (!string.IsNullOrEmpty(username) && username.Contains('\\'))
            {
                var authParts = username.Split('\\');
                domain = authParts[0];
                user = authParts[1];
            }

            var handler = new HttpClientHandler
            {
                Credentials = new System.Net.NetworkCredential(user, password, domain),
                PreAuthenticate = true
            };

            using var client = new HttpClient(handler);
            var response = await client.GetAsync(requestUrl);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new System.Exception($"API Failed for {targetCompany}! Status: {response.StatusCode}. URL: {requestUrl} | Details: {errorContent}");
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var bcData = JsonSerializer.Deserialize<BusinessCentralCustomerResponse>(jsonString);

            // Map and Save to Database
            foreach (var bcCustomer in bcData.value)
            {
                var existingAccount = context.Account.FirstOrDefault(a => a.ERPCustomerCode == bcCustomer.No);

                // Map PostCode String to Database PostCodeId
                int? mappedPostcodeId = null;
                if (!string.IsNullOrEmpty(bcCustomer.Post_Code))
                {
                    var matchedPostcode = context.Postcodes.FirstOrDefault(p => p.ZipCode == bcCustomer.Post_Code);
                    if (matchedPostcode != null)
                    {
                        mappedPostcodeId = matchedPostcode.Id;
                    }
                }

                if (existingAccount != null)
                {
                    // Update
                    existingAccount.AccountName = bcCustomer.Name;
                    existingAccount.Email = bcCustomer.E_Mail;
                    existingAccount.Phone = !string.IsNullOrEmpty(bcCustomer.Mobile_Phone_No) ? bcCustomer.Mobile_Phone_No : bcCustomer.Phone_No;
                    existingAccount.Country = bcCustomer.Country_Region_Code;
                    existingAccount.State = bcCustomer.State_Code;
                    existingAccount.City = bcCustomer.City;
                    existingAccount.PostCodeId = mappedPostcodeId;
                    existingAccount.CustomerRelationship = bcCustomer.Customer_Relationship;
                    existingAccount.Address = bcCustomer.Address;
                    existingAccount.Address2 = bcCustomer.Address_2;
                    existingAccount.SalespersonCode = bcCustomer.Salesperson_Code;
                    existingAccount.Location = bcCustomer.Location_Code;
                    existingAccount.Postcode = bcCustomer.Post_Code;

                    // Set the company name dynamically based on which API we are calling
                    existingAccount.CompanyName = targetCompany;
                }
                else
                {
                    // Insert
                    var newAccount = new Account
                    {
                        ERPCustomerCode = bcCustomer.No,
                        AccountName = bcCustomer.Name,
                        Email = bcCustomer.E_Mail,
                        Phone = !string.IsNullOrEmpty(bcCustomer.Mobile_Phone_No) ? bcCustomer.Mobile_Phone_No : bcCustomer.Phone_No,
                        Country = bcCustomer.Country_Region_Code,
                        State = bcCustomer.State_Code,
                        City = bcCustomer.City,
                        PostCodeId = mappedPostcodeId,
                        CustomerRelationship = bcCustomer.Customer_Relationship,
                        Address = bcCustomer.Address,
                        Address2 = bcCustomer.Address_2,
                        SalespersonCode = bcCustomer.Salesperson_Code,
                        Location = bcCustomer.Location_Code,
                        Postcode = bcCustomer.Post_Code,

                        // Set the company name dynamically based on which API we are calling
                        CompanyName = targetCompany,

                        AccountOwner = "zcrm_876952000000348001"
                    };
                    context.Account.Add(newAccount);
                }
            }
        }
    }





    public class BusinessCentralCustomerResponse
    {
        public BCCustomer[] value { get; set; }
    }

    public class BCCustomer
    {
        // These property names MUST exactly match the JSON keys from your BC API
        public string No { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Address_2 { get; set; }
        public string Country_Region_Code { get; set; }
        public string State_Code { get; set; }
        public string City { get; set; }
        public string Post_Code { get; set; }
        public string E_Mail { get; set; }
        public string Customer_Relationship { get; set; }
        public bool CRM { get; set; }
        public string CRM_ID { get; set; }
        public string Phone_No { get; set; }
        public string Mobile_Phone_No { get; set; }
        public string Salesperson_Code { get; set; }
        public string Currency_Code { get; set; }
        public string Location_Code { get; set; }
    }
}
