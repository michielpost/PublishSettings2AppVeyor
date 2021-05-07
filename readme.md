# Batch create AppVeyor environments based on .PublishSettings files

This is a useful tool when you want to create a lot of Environments in AppVeyor but you don't want to manually copy and paste data from a PublishSettings file.

Tested with PublishSettings downloaded from Azure.

Works great with this tool to mass download all your Azure Website publishing profiles:
https://github.com/michielpost/DumpAzurePublishProfiles

## Usage
- Run `pub2appveyor` it will ask for your AppVeyor API key and upload all PublishSettings in the current directory  
- Run `pub2appveyor <AppVeyor-key>` to upload all PublishSettings in the current directory  
- Run `pub2appveyor <AppVeyor-key> "C:\path_to_directory_with_PublishSettings"` to upload all PublishSettings in the specified directory

