using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureUploader;

class Program
{
    private const string connectionString = "DefaultEndpointsProtocol=https;AccountName=emsapplication;AccountKey=RGeqSx0tXX755pErp4+67NOh7vk53ICLsczTlkPKvo4U0COe0DWhiV7g8uXoIf/ueKWskULJ/7yn+AStr0Xf1w==;EndpointSuffix=core.windows.net";
    private const string uploadPath = @"C:\Users\NARADEVA\Desktop\Azure\AzureUploader\SampleFiles";
    private const string downloadPath = @"C:\Users\NARADEVA\Desktop\Azure\AzureUploader\Downloads";

    static async Task Main()
    {
        AzureHelper helper = new AzureHelper(connectionString);

        while (true)
        {
            Console.WriteLine("\nChoose an Operation:");
            Console.WriteLine("1️. Upload a File");
            Console.WriteLine("2️. Retrieve a File");
            Console.WriteLine("3️. Exit");
            Console.Write("Enter your choice: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.WriteLine("\nFetching list of containers...");
                    List<string> uploadContainers = await helper.GetContainersAsync();
                    if (uploadContainers.Count == 0)
                    {
                        Console.WriteLine("No containers found.");
                        break;
                    }

                    Console.WriteLine("\nAvailable Containers:");
                    for (int i = 0; i < uploadContainers.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {uploadContainers[i]}");
                    }

                    Console.Write("\nSelect a container number: ");
                    int uploadContainerIndex = int.Parse(Console.ReadLine()) - 1;
                    if (uploadContainerIndex < 0 || uploadContainerIndex >= uploadContainers.Count)
                    {
                        Console.WriteLine("Invalid selection!");
                        break;
                    }

                    string uploadContainer = uploadContainers[uploadContainerIndex];
                    AzureHelper uploadHelper = new AzureHelper(connectionString, uploadContainer);

                    Console.Write("\nEnter the file name to upload: ");
                    string fileName = Console.ReadLine();
                    string uploadResult = await uploadHelper.UploadFileAsync(fileName, uploadPath);
                    Console.WriteLine(uploadResult);
                    break;

                case "2":
                    Console.WriteLine("\nFetching list of containers...");
                    List<string> retrieveContainers = await helper.GetContainersAsync();
                    if (retrieveContainers.Count == 0)
                    {
                        Console.WriteLine("No containers found.");
                        break;
                    }

                    Console.WriteLine("\nAvailable Containers:");
                    for (int i = 0; i < retrieveContainers.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {retrieveContainers[i]}");
                    }

                    Console.Write("\nSelect a container number: ");
                    int retrieveContainerIndex = int.Parse(Console.ReadLine()) - 1;
                    if (retrieveContainerIndex < 0 || retrieveContainerIndex >= retrieveContainers.Count)
                    {
                        Console.WriteLine("Invalid selection!");
                        break;
                    }

                    string retrieveContainer = retrieveContainers[retrieveContainerIndex];
                    AzureHelper retrieveHelper = new AzureHelper(connectionString, retrieveContainer);

                    Console.Write("\nEnter the file name to retrieve: ");
                    string blobName = Console.ReadLine();
                    string retrieveResult = await retrieveHelper.RetrieveFileAsync(blobName, downloadPath);
                    Console.WriteLine(retrieveResult);
                    break;

                case "3":
                    Console.WriteLine("\nExiting the program.");
                    return;

                default:
                    Console.WriteLine("Invalid choice! Please enter 1, 2, or 3.");
                    break;
            }
        }
    }
}
