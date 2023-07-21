# Creating Raw Ethereum Transactions with NEthereum and Submitting Via Altura API for User Signature 

## Overview
In this guide, we will demonstrate how to create raw transactions on the Ethereum network using the NEthereum library in C#, and then submit those transactions via Altura's API for user signature. 

The example code will call the `approve` function on a deployed contract. For context, the `approve` function is part of the ERC20 token standard and it gives a certain address (the `spender`) allowance to spend tokens on behalf of the token holder.

## Libraries Used
The code uses several libraries, which you need to add as dependencies:

- Nethereum.ABI
- Nethereum.Hex.HexConvertors.Extensions
- Nethereum.Util
- Newtonsoft.Json
- System.Net.Http

Most of these can be installed via NuGet. 

## Getting Started
1. Create a new .NET Console Application.
2. Install the necessary NuGet packages: `Nethereum.ABI`, `Nethereum.Hex.HexConvertors.Extensions`, `Nethereum.Util`, and `Newtonsoft.Json`.
3. Copy the provided code into the `Program.cs` file in your project.

## Understanding the Code
The code begins by defining some constant strings for the Altura API key, API endpoint, user wallet address, and contract address. 

The `Main` method contains the logic for creating the transaction, submitting it to Altura API, and then polling the API for a response.

1. **Creating the Transaction**

   The raw transaction is created by encoding the smart contract function and its parameters. In this case, we're using the `approve` function, which requires a `spender` address and an `amount`. The method name is hashed using the `Sha3Keccack` function to get the function signature that the Ethereum Virtual Machine (EVM) uses to identify the function to call. 

   We then use the `ABIEncode` class to encode the function parameters. Note that the amount is converted to Wei (the smallest denomination of ether) before encoding. 

   The function call data is then assembled by concatenating the function signature with the encoded parameters.

2. **Submitting the Request to Altura API**

   We create an anonymous object representing the request payload, which includes the API token, request parameters (including the `from` and `to` addresses, the function call data, and the value of ether to send), and the chain id. 

   We then serialize this object to JSON and send it to the Altura API via a POST request.

3. **Polling for the Response**

   After submitting the transaction, we periodically poll the Altura API for a response. If the HTTP status code indicates that there is no content (i.e., the user has not yet signed the transaction), we wait for 10 seconds before sending another request. When a response is finally received, it is printed to the console.

The `ApproveFunction` class represents the `approve` function in the ERC20 standard. It is annotated with the `Function` attribute, and its properties are annotated with the `Parameter` attribute, which specifies the type and order of the function parameters. 

## Running the Code
Before running the code, make sure to replace the placeholders in the code with actual values:

- Replace the `AlturaAPIKey` string with your actual API key.
- Ensure you have already created a connection with the user for the token
- Replace the `userAddress` string with the wallet address of the user.
- Replace the `contractAddress` string with the address of the contract you are interacting with.

You can then run the code in your development environment. It will send the transaction to the Altura API, which will return a response when the user has signed or rejected the transaction.
