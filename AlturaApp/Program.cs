
using System.Numerics;
using Nethereum.ABI;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Util;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Util.Keccak;

class Program
{
    private static readonly string AlturaAPIKey = ""; // Add your API key
    private static readonly string AlturaAPI = "https://api.alturanft.com";
    private static readonly HttpClient httpClient = new HttpClient();
    private static readonly string userAddress = "0xc0016f4AE265f7311B4B6991a7aafc4052A8d3E7"; // the address of the user the tx is from, its hardcoded here
    private static readonly string contractAddress = "0x78867BbEeF44f2326bF8DDd1941a4439382EF2A7"; // the address of the contract we are interacting with

    static async Task Main(string[] args)
    {
        try
        {
    var functionSignature = "approve(address,uint256)";
            var sha3 = new Sha3Keccack();
            var functionHash = sha3.CalculateHash(functionSignature);
            var functionSelector = functionHash.Substring(0, 8); // First 4 bytes of the hash

            // Create the function call data
            var abiEncode = new ABIEncode();
            var amount =  Nethereum.Util.UnitConversion.Convert.ToWei(0.1m);
            var functionParametersData = abiEncode.GetABIParamsEncoded(new ApproveFunction()
            {
                Spender = userAddress,
                Amount = amount
            }).ToHex();

            var functionCallData = functionSelector + functionParametersData;
            // Create the request payload
            var payload = new
            {
                token = "d8f9eb28-bb06-4c32-8048-32a49cb8dca5",
                reqParameters = new object[] {
                    "transaction",
                    new {
                        from = userAddress,
                        to = contractAddress,
                        data = "0x"+functionCallData,
                        value = "0x0"
                    },
                    97 // chain id
                }
            };

            // Send the request
            var requestContent = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            Console.WriteLine(JsonConvert.SerializeObject(payload));
            var response = await httpClient.PostAsync($"{AlturaAPI}/api/alturaguard/request", requestContent);
            var responseData = await response.Content.ReadAsStringAsync();
            var requestId = JsonConvert.DeserializeObject<dynamic>(responseData).requestId;

            // Poll for the response
            var result = await PollForResponse(requestId.ToString());
            Console.WriteLine(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static async Task<string> PollForResponse(string requestId)
    {
        while (true)
        {
            var payload = new
            {
                token = "d8f9eb28-bb06-4c32-8048-32a49cb8dca5",
                requestId
            };
            var requestContent = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync($"{AlturaAPI}/api/alturaguard/getResponse", requestContent);
            if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                return responseData;
            }
            await Task.Delay(10000); // Wait for 10 seconds before the next request
        }
    }
}

[Function("approve", "bool")]
public class ApproveFunction
{
    [Parameter("address", "spender", 1)]
    public string Spender { get; set; }

    [Parameter("uint256", "amount", 2)]
    public BigInteger Amount { get; set; }
}