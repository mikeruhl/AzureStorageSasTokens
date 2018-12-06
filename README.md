# AzureStorageSasTokens
Test Sas Tokens on Blobs, Containers, and Access Policies

## Overview
This is a console app adapted from a tutorial located [here](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-dotnet-shared-access-signature-part-2#part-2-create-a-console-application-to-test-the-shared-access-signatures) from Microsoft that explains the use of Share Access Signatures in Azure Storage.  This console application creates and tests four scenarios as outlined in the original tutorial:
- Container With Sas
- Blob With Sas
- Container With Access Policy
- Blob With Access Policy

The purpose of this application was to refamiliarize myself with the concepts and isolate the different aspects of the tutorial into separate classes to better read and understand it.

## Installation
All that's required for this source code to run is a valid [storage account](https://docs.microsoft.com/en-us/azure/storage/common/storage-account-overview) in Azure or run the [Storage Emulator](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-emulator).

The current appsettings.json is configured to use a storage emulator installed locally.  This is the cheapest option for testing this code as it does not require any subscription to Azure.

## Security Risks
Please be mindful of exposing storage account connection strings in your source code.  If the preference is to run this in Azure, please create a test storage account and immediately delete it after running the program and studying the examples.  This will mitigate any unknown changes the code will have on your existing accounts.
