using AIS.Application.Interfaces.Repositories;
using AIS.Domain.ImageFiles;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AIS.Persistance.ImageFiles
{
    public class InMemoryImageFileRepository : IImageFileRepository
    {
        private static int _currentMaxId = 0;
        private static object _lockObject = new object();
        private readonly Dictionary<int, ImageFile> _imageFileStore = new Dictionary<int, ImageFile>();

        public Task<ImageFile[]> GetAllImageFiles(CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            return Task.FromResult(_imageFileStore.Values.ToArray());
        }

        public Task<ImageFile> GetByMd5(Md5Info md5Info, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            var imageFile = _imageFileStore.Values.FirstOrDefault(x => x.Md5 == md5Info);
            return Task.FromResult(imageFile);
        }

        public Task<int> SaveImageFile(ImageFile imageFile, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            if (imageFile.IsIdSet())
            {
                _imageFileStore[imageFile.Id] = imageFile;
                return Task.FromResult(imageFile.Id);
            }
            else
            {
                lock (_lockObject)
                {
                    _currentMaxId++;
                    imageFile.SetId(_currentMaxId);
                    _imageFileStore[_currentMaxId] = imageFile;
                    return Task.FromResult(imageFile.Id);
                }
            }
        }
    }
}
