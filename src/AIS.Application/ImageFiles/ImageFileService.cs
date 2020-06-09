using AIS.Application.Interfaces.Repositories;
using AIS.Domain.ImageFiles;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AIS.Application.ImageFiles
{
    public class ImageFileService
    {
        private readonly IImageFileRepository _imageFileRepository;
        private readonly IImagePathFactory _imagePathFactory;
        private readonly IDirectory _directory;
        
        public ImageFileService(IImageFileRepository imageFileRepository,
            IDirectory directory,
            IImagePathFactory imagePathFactory)
        {
            _imageFileRepository = imageFileRepository ?? throw new ArgumentNullException(nameof(imageFileRepository));
            _directory = directory;
            _imagePathFactory = imagePathFactory;
        }

        public async Task<int> SaveNewImageFile(ImageFilePath filePath, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            var newFile = ImageFile.Factory.CreateFromFileImagePath(filePath);

            var existingFile = await _imageFileRepository.GetByMd5(newFile.Md5, token);
            if (!(existingFile is null))
                return existingFile.Id;

            var fileId = await _imageFileRepository.SaveImageFile(newFile, token);
            return fileId;
        }

        public async Task<ImageFile> UpdateImageFile(ImageFile imageFile, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var existingFile = await _imageFileRepository.GetByMd5(imageFile.Md5, token);
            if (existingFile is null)
                throw new ArgumentException($"Image file with id {imageFile.Id} not found");

            var fileId = await _imageFileRepository.SaveImageFile(imageFile, token);
            return imageFile;
        }

        public async Task<ImageFile[]> GetKnownImages(CancellationToken token = default)
        {
            var imageFiles = await _imageFileRepository.GetAllImageFiles(token);
            return imageFiles;
        }

        public Task<ImageFilePath[]> FindImagesInFolder(string folderPath, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var imageFolderPath = _imagePathFactory.CreateFolderPath(folderPath);
            //ищем же мы только картинки, сооружаем паттерн для поиска
            var stringImageExtensions = Enum.GetNames(typeof(ImageFileExtenstion)).Select(x => $"*.{x}");
            var files = stringImageExtensions.SelectMany(x => _directory.GetFiles(imageFolderPath.Directory, x, SearchOption.TopDirectoryOnly))
                .ToArray();
            if (!files.Any())
                return Task.FromResult(Enumerable.Empty<ImageFilePath>().ToArray());

            var imageFilePathArray = files.Select(x => _imagePathFactory.CreateFilePathFromStringPath(x)).ToArray();
            return Task.FromResult(imageFilePathArray);
        }
    }
}
