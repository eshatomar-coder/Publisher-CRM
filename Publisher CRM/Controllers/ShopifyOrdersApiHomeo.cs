using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Pegasus_Project1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShopifyOrdersApiHomeo : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // --- CONFIGURATION: OAuth Credentials & Store Details ---
        private readonly string storeName = "aeb7f1-0d";
        private readonly string apiVersion = "2024-01";
        private readonly string clientId = "7e5207eb964ff18fe9e8287c8be33139";
        private readonly string clientSecret = "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";

        public ShopifyOrdersApiHomeo(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private async Task<string?> GetFreshShopifyAccessTokenAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var tokenUrl = $"https://{storeName}.myshopify.com/admin/oauth/access_token";

            var requestContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret)
            });

            var response = await client.PostAsync(tokenUrl, requestContent);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var parsedJson = JObject.Parse(jsonString);
                return parsedJson["access_token"]?.ToString();
            }

            return null;
        }

        [HttpGet("orders/by-date")]
        public async Task<IActionResult> GetOrdersForBusinessCentral([FromQuery] string? date)
        {
            var freshAccessToken = await GetFreshShopifyAccessTokenAsync();
            if (string.IsNullOrEmpty(freshAccessToken))
            {
                return StatusCode(500, "Failed to authenticate with Shopify: Unable to retrieve access token.");
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("X-Shopify-Access-Token", freshAccessToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string queryArgument = "";

            // --- DATE LOGIC UPDATED TO ONLY FETCH TODAY BY DEFAULT ---
            if (string.IsNullOrWhiteSpace(date))
            {
                string startDate = DateTime.Now.ToString("yyyy-MM-dd");
                string endDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
                queryArgument = $", query: \"created_at:>={startDate} AND created_at:<{endDate}\"";
            }
            else
            {
                if (!DateTime.TryParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime targetDate))
                {
                    return BadRequest("Please provide a valid Date in the format dd-MM-yyyy.");
                }

                string startDate = targetDate.ToString("yyyy-MM-dd");
                string endDate = targetDate.AddDays(1).ToString("yyyy-MM-dd");
                queryArgument = $", query: \"created_at:>={startDate} AND created_at:<{endDate}\"";
            }

            string graphqlUrl = $"https://{storeName}.myshopify.com/admin/api/{apiVersion}/graphql.json";

            var businessCentralData = new JArray();
            int totalOrdersProcessed = 0;

            bool hasNextPage = true;
            string? cursor = null;

            while (hasNextPage)
            {
                string afterArgument = string.IsNullOrEmpty(cursor) ? "" : $", after: \"{cursor}\"";

                var graphQlQuery = new
                {
                    query = $@"
                    {{
                        orders(first: 250, sortKey: CREATED_AT, reverse: true{queryArgument}{afterArgument}) {{
                            pageInfo {{
                                hasNextPage
                                endCursor
                            }}
                            edges {{
                                node {{
                                    name
                                    tags
                                    currencyCode
                                    shippingAddress {{
                                        firstName
                                        lastName
                                        address1
                                        address2
                                        city
                                        zip
                                        countryCodeV2
                                        phone
                                        provinceCode
                                    }}
                                    billingAddress {{
                                        firstName
                                        lastName
                                        address1
                                        address2
                                        city
                                        zip
                                        countryCodeV2
                                        phone
                                        provinceCode
                                    }}
                                    subtotalPriceSet {{
                                        shopMoney {{
                                            amount
                                        }}
                                    }}
                                    totalDiscountsSet {{
                                        shopMoney {{
                                            amount
                                        }}
                                    }}
                                    lineItems(first: 50) {{
                                        edges {{
                                            node {{
                                                sku
                                                variant {{
                                                    barcode
                                                    price
                                                    compareAtPrice
                                                }}
                                                quantity
                                                originalUnitPriceSet {{
                                                    shopMoney {{
                                                        amount
                                                    }}
                                                }}
                                            }}
                                        }}
                                    }}
                                    shippingLines(first: 5) {{
                                        edges {{
                                            node {{
                                                title
                                                discountedPriceSet {{
                                                    shopMoney {{
                                                        amount
                                                    }}
                                                }}
                                            }}
                                        }}
                                    }}
                                }}
                            }}
                        }}
                    }}"
                };

                var jsonContent = JsonConvert.SerializeObject(graphQlQuery);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(graphqlUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "Failed to connect to Shopify.");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var parsedJson = JObject.Parse(jsonResponse);

                if (parsedJson["errors"] != null)
                {
                    return BadRequest(new
                    {
                        status = false,
                        message = "Shopify returned an error.",
                        shopify_errors = parsedJson["errors"]?.ToString()
                    });
                }

                var ordersData = parsedJson.SelectToken("data.orders");
                var orderEdges = parsedJson.SelectToken("data.orders.edges") as JArray;

                if (orderEdges == null || !orderEdges.Any())
                {
                    if (totalOrdersProcessed == 0 && businessCentralData.Count == 0)
                    {
                        string messageDate = string.IsNullOrWhiteSpace(date) ? "today" : date;
                        return NotFound(new
                        {
                            status = false,
                            message = $"No orders found for {messageDate}.",
                            raw_shopify_response = parsedJson
                        });
                    }
                    break;
                }

                foreach (var orderEdge in orderEdges)
                {
                    var orderNode = orderEdge.SelectToken("node");

                    if (orderNode == null || orderNode.Type == JTokenType.Null)
                    {
                        continue;
                    }

                    var tagsArray = orderNode.SelectToken("tags") as JArray;
                    if (tagsArray != null && tagsArray.Any(t => t.ToString().Equals("PWC", StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }

                    totalOrdersProcessed++;
                    var orderId = orderNode.SelectToken("name")?.ToString()?.TrimStart('#');

                    var addressNode = orderNode.SelectToken("shippingAddress");
                    if (addressNode == null || addressNode.Type == JTokenType.Null)
                    {
                        addressNode = orderNode.SelectToken("billingAddress");
                    }

                    string fName = addressNode?.SelectToken("firstName")?.ToString() ?? "";
                    string lName = addressNode?.SelectToken("lastName")?.ToString() ?? "";
                    string customerName = $"{fName} {lName}".Trim();

                    string add1 = addressNode?.SelectToken("address1")?.ToString() ?? "";
                    string add2 = addressNode?.SelectToken("address2")?.ToString() ?? "";
                    string customerAddress = $"{add1} {add2}".Trim();

                    string city = addressNode?.SelectToken("city")?.ToString() ?? "";
                    string postCode = addressNode?.SelectToken("zip")?.ToString() ?? "";
                    string countryCode = addressNode?.SelectToken("countryCodeV2")?.ToString() ?? "";
                    string phoneNumber = addressNode?.SelectToken("phone")?.ToString() ?? "";
                    string stateCode = addressNode?.SelectToken("provinceCode")?.ToString() ?? "";

                    string currencyCode = orderNode.SelectToken("currencyCode")?.ToString() ?? "";

                    decimal.TryParse(orderNode.SelectToken("subtotalPriceSet.shopMoney.amount")?.ToString(), out decimal orderSubtotal);
                    decimal.TryParse(orderNode.SelectToken("totalDiscountsSet.shopMoney.amount")?.ToString(), out decimal orderTotalDiscount);

                    decimal baseSubtotal = orderSubtotal + orderTotalDiscount;
                    decimal orderDiscountPercentage = 0;

                    if (baseSubtotal > 0 && orderTotalDiscount > 0)
                    {
                        orderDiscountPercentage = (orderTotalDiscount / baseSubtotal);
                    }

                    decimal totalShippingAndCodCost = 0;
                    bool hasShippingOrCod = false;

                    var lineItems = orderNode.SelectToken("lineItems.edges") as JArray;
                    if (lineItems != null)
                    {
                        foreach (var item in lineItems)
                        {
                            var node = item.SelectToken("node");

                            if (node == null || node.Type == JTokenType.Null)
                            {
                                continue;
                            }

                            var isbnNo = node.SelectToken("variant.barcode")?.ToString();
                            if (string.IsNullOrEmpty(isbnNo))
                            {
                                isbnNo = node.SelectToken("sku")?.ToString();
                            }

                            if (isbnNo != null && isbnNo.Equals("COD", StringComparison.OrdinalIgnoreCase))
                            {
                                decimal.TryParse(node.SelectToken("originalUnitPriceSet.shopMoney.amount")?.ToString(), out decimal originalCodPrice);
                                if (originalCodPrice == 0)
                                {
                                    decimal.TryParse(node.SelectToken("variant.price")?.ToString(), out originalCodPrice);
                                }

                                decimal finalCodPaid = originalCodPrice - (originalCodPrice * orderDiscountPercentage);

                                totalShippingAndCodCost += finalCodPaid;
                                hasShippingOrCod = true;
                                continue;
                            }

                            var quantity = node.SelectToken("quantity")?.Value<int>() ?? 0;

                            decimal.TryParse(node.SelectToken("variant.compareAtPrice")?.ToString(), out decimal compareAtPrice);
                            decimal.TryParse(node.SelectToken("variant.price")?.ToString(), out decimal variantPrice);
                            decimal.TryParse(node.SelectToken("originalUnitPriceSet.shopMoney.amount")?.ToString(), out decimal originalLinePrice);

                            decimal actualPrice = compareAtPrice > 0 ? compareAtPrice : (variantPrice > 0 ? variantPrice : originalLinePrice);
                            decimal baseSellingPrice = originalLinePrice > 0 ? originalLinePrice : variantPrice;
                            decimal finalPaidPrice = baseSellingPrice - (baseSellingPrice * orderDiscountPercentage);

                            decimal lineDscPer = 0;
                            if (actualPrice > 0 && actualPrice > finalPaidPrice)
                            {
                                lineDscPer = Math.Round(((actualPrice - finalPaidPrice) / actualPrice) * 100, 2);
                            }

                            if (actualPrice > 0)
                            {
                                businessCentralData.Add(new JObject
                                {
                                    ["CustomerName"] = customerName,
                                    ["CustomerAddress"] = customerAddress,
                                    ["City"] = city,
                                    ["PostCode"] = postCode,
                                    ["CountryCode"] = countryCode,
                                    ["PhoneNumber"] = phoneNumber,
                                    ["StateCode"] = stateCode,
                                    ["CurrencyCode"] = currencyCode,
                                    // CurrencyRate removed
                                    ["OrderId"] = orderId,
                                    ["IsbnNo"] = isbnNo,
                                    ["Quantity"] = quantity,
                                    ["UnitPrice"] = actualPrice,
                                    ["Line_Dsc_Per"] = lineDscPer
                                });
                            }
                        }
                    }

                    var shippingLines = orderNode.SelectToken("shippingLines.edges") as JArray;
                    if (shippingLines != null)
                    {
                        foreach (var item in shippingLines)
                        {
                            var node = item.SelectToken("node");
                            if (node == null || node.Type == JTokenType.Null)
                            {
                                continue;
                            }

                            decimal.TryParse(node.SelectToken("discountedPriceSet.shopMoney.amount")?.ToString(), out decimal shippingPrice);

                            totalShippingAndCodCost += shippingPrice;
                            hasShippingOrCod = true;
                        }
                    }

                    if (hasShippingOrCod && totalShippingAndCodCost > 0)
                    {
                        businessCentralData.Add(new JObject
                        {
                            ["CustomerName"] = customerName,
                            ["CustomerAddress"] = customerAddress,
                            ["City"] = city,
                            ["PostCode"] = postCode,
                            ["CountryCode"] = countryCode,
                            ["PhoneNumber"] = phoneNumber,
                            ["StateCode"] = stateCode,
                            ["CurrencyCode"] = currencyCode,
                            // CurrencyRate removed
                            ["OrderId"] = orderId,
                            ["IsbnNo"] = "COD+Shipping",
                            ["Quantity"] = 1,
                            ["UnitPrice"] = Math.Round(totalShippingAndCodCost, 2),
                            ["Line_Dsc_Per"] = 0
                        });
                    }
                }

                hasNextPage = ordersData?.SelectToken("pageInfo.hasNextPage")?.Value<bool>() ?? false;
                cursor = ordersData?.SelectToken("pageInfo.endCursor")?.ToString();
            }

            var finalResult = new JObject
            {
                ["status"] = true,
                ["totalOrdersProcessed"] = totalOrdersProcessed,
                ["data"] = businessCentralData
            };

            return Content(finalResult.ToString(), "application/json");
        }
    }
}