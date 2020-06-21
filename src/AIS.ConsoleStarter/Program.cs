using AIS.Application.ImageFiles;
using AIS.Application.Interfaces.Infrastructure;
using AIS.Application.Interfaces.Repositories;
using AIS.Application.PictureSearchers;
using AIS.Application.PictureSearchers.Models;
using AIS.Domain.ImageFiles;
using AIS.Infrastructure.IqdbWebClient;
using AIS.Persistance.ImageFiles;
using FastMember;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace AIS.ConsoleStarter
{
    class Program
    {

        static async Task Main(string[] args)
        {
            IImageFileRepository imageFileRepository = new InMemoryImageFileRepository();
            IFileSystem fileSystem = new FileSystem();
            IDirectory directory = fileSystem.Directory;
            IImagePathFactory imagePathFactory = new ImagePathFactory(fileSystem);
            ImageFileService imageFileService = new ImageFileService(imageFileRepository, directory, imagePathFactory);

            IIqdbResponseParser parser = new IqdbResponseParser();

            // тест парсинга ответа
            //string testResposeFile = "D:\\temp\\missedQuery.txt";
            //using var testResposeStream = new StreamReader(testResposeFile);
            //var response = parser.ParseResponse(testResposeStream);

            // тест запроса в апи + парсинг ответа 

            IIqdbWebClient iqdbWebClient = new IqdbWebClient(new System.Net.Http.HttpClient());
            //string testFile = "D:\\temp\\4w5l5o3.jpg";
            //var imageFile = ImageInfo.Factory.CreateFromFile(fileSystem, testFile, Resolution.Factory.GetZeroed());
            //using (var responseStream = await iqdbWebClient.RequestImageSearch(imageFile))
            //{
            //    string a = responseStream.ReadToEnd();
            //    responseStream.BaseStream.Position = 0;

            //    var testIqdbResponse = parser.ParseResponse(responseStream);
            //}

            // тест чтения файлов из папки

            //string testFolder = "D:\\temp";
            //var imageFilePathArray = await imageFileService.FindImagesInFolder(testFolder);
            //foreach (var imageFilePath in imageFilePathArray)
            //{
            //    var fileId = await imageFileService.SaveNewImageFile(imageFilePath);
            //}

            // тестируем чтение из папки + запрос к iqdb + парсинг ответа 

            string testFolder = "D:\\formerC\\Desktop\\look for";
            var imageFilePathArray = await imageFileService.FindImagesInFolder(testFolder);
            int a = 0;
            foreach (var imageFilePath in imageFilePathArray)
            {
                Console.WriteLine(++a);
                var imageFile = ImageInfo.Factory.CreateFromFile(fileSystem, imageFilePath, Resolution.Factory.GetZeroed());
                using (var responseStream = await iqdbWebClient.RequestImageSearch(imageFile))
                {
                    var testIqdbResponse = parser.ParseResponse(responseStream);
                }
            }


            var savedFiles = await imageFileService.GetKnownImages();
            foreach (var file in savedFiles)
                Console.WriteLine(file.GetPrintableImageFileInfo());

            Console.WriteLine("Hello World!");
        }
    }
}
